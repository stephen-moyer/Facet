﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Facet.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class GenerateDtosGenerator : IIncrementalGenerator
{
	private const string GenerateDtosAttributeName = "Facet.GenerateDtosAttribute";
	private const string GenerateAuditableDtosAttributeName = "Facet.GenerateAuditableDtosAttribute";

	// Common audit field patterns
	private static readonly HashSet<string> DefaultAuditFields = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
	{
		"CreatedDate", "UpdatedDate", "CreatedAt", "UpdatedAt",
		"CreatedBy", "UpdatedBy", "CreatedById", "UpdatedById"
	};

	// Common ID field patterns
	private static readonly HashSet<string> IdFieldPatterns = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
	{
		"Id"
	};

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var generateDtosTargets = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				GenerateDtosAttributeName,
				predicate: static (node, _) => node is TypeDeclarationSyntax,
				transform: static (ctx, token) => GetGenerateDtosModels(ctx, token))
			.Where(static m => m is not null)
			.SelectMany(static (models, _) => models!);

		var generateAuditableDtosTargets = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				GenerateAuditableDtosAttributeName,
				predicate: static (node, _) => node is TypeDeclarationSyntax,
				transform: static (ctx, token) => GetGenerateAuditableDtosModels(ctx, token))
			.Where(static m => m is not null)
			.SelectMany(static (models, _) => models!);

		var allTargets = generateDtosTargets.Collect().Combine(generateAuditableDtosTargets.Collect())
			.Select(static (combined, _) => combined.Left.Concat(combined.Right));

		context.RegisterSourceOutput(allTargets, static (spc, models) =>
		{
			foreach (var model in models)
			{
				if (model != null)
				{
					spc.CancellationToken.ThrowIfCancellationRequested();
					GenerateDtosForModel(spc, model);
				}
			}
		});
	}

	private static IEnumerable<GenerateDtosTargetModel>? GetGenerateDtosModels(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		return GetDtosModels(context, token, isAuditable: false);
	}

	private static IEnumerable<GenerateDtosTargetModel>? GetGenerateAuditableDtosModels(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		return GetDtosModels(context, token, isAuditable: true);
	}

	private static IEnumerable<GenerateDtosTargetModel>? GetDtosModels(GeneratorAttributeSyntaxContext context, CancellationToken token, bool isAuditable)
	{
		token.ThrowIfCancellationRequested();
		if (context.TargetSymbol is not INamedTypeSymbol sourceSymbol) return null;
		if (context.Attributes.Length == 0) return null;

		var models = new List<GenerateDtosTargetModel>();

		// Process each attribute separately to support AllowMultiple
		foreach (var attribute in context.Attributes)
		{
			token.ThrowIfCancellationRequested();

			var model = GetDtosModel(context, attribute, sourceSymbol, isAuditable, token);
			if (model != null)
			{
				models.Add(model);
			}
		}

		return models.Count > 0 ? models : null;
	}

	private static GenerateDtosTargetModel? GetDtosModel(GeneratorAttributeSyntaxContext context, AttributeData attribute, INamedTypeSymbol sourceSymbol, bool isAuditable, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		try
		{
			// Extract attribute properties with proper enum handling
			var types = GetNamedArg(attribute.NamedArguments, "Types", DtoTypes.All);
			var outputType = GetNamedArg(attribute.NamedArguments, "OutputType", OutputType.Record);
			var targetNamespace = GetNamedArg<string?>(attribute.NamedArguments, "Namespace", null);
			var prefix = GetNamedArg<string?>(attribute.NamedArguments, "Prefix", null);
			var suffix = GetNamedArg<string?>(attribute.NamedArguments, "Suffix", null);
			var includeFields = GetNamedArg(attribute.NamedArguments, "IncludeFields", false);
			var generateConstructors = GetNamedArg(attribute.NamedArguments, "GenerateConstructors", true);
			var generateProjections = GetNamedArg(attribute.NamedArguments, "GenerateProjections", true);
			var useFullName = GetNamedArg(attribute.NamedArguments, "UseFullName", false);

			// Fix the ExcludeProperties handling
			var userExcludeProperties = new List<string>();
			var excludePropertiesArg = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == "ExcludeProperties");
			if (excludePropertiesArg.Value.Kind != TypedConstantKind.Error && !excludePropertiesArg.Value.IsNull)
			{
				if (excludePropertiesArg.Value.Kind == TypedConstantKind.Array)
				{
					userExcludeProperties.AddRange(
						excludePropertiesArg.Value.Values
							.Where(v => v.Value?.ToString() != null)
							.Select(v => v.Value!.ToString()!));
				}
			}

			// Build exclusion list
			var excludeProperties = new HashSet<string>(userExcludeProperties, System.StringComparer.OrdinalIgnoreCase);

			if (isAuditable)
			{
				foreach (var field in DefaultAuditFields)
				{
					excludeProperties.Add(field);
				}
			}

			var members = new List<FacetMember>();
			var addedMembers = new HashSet<string>();

			var allMembersWithModifiers = GeneratorUtilities.GetAllMembersWithModifiers(sourceSymbol);

			foreach (var (member, isInitOnly, isRequired) in allMembersWithModifiers)
			{
				token.ThrowIfCancellationRequested();
				if (excludeProperties.Contains(member.Name)) continue;
				if (addedMembers.Contains(member.Name)) continue;

				if (member is IPropertySymbol { DeclaredAccessibility: Accessibility.Public } p)
				{
					members.Add(new FacetMember(
						p.Name,
						GeneratorUtilities.GetTypeNameWithNullability(p.Type),
						FacetMemberKind.Property,
						p.Type.IsValueType,
						isInitOnly,
						isRequired,
						false, // Properties are not readonly in the field sense
						null)); // No XML documentation for GenerateDtos
					addedMembers.Add(p.Name);
				}
				else if (includeFields && member is IFieldSymbol { DeclaredAccessibility: Accessibility.Public } f)
				{
					var isReadOnly = f.IsReadOnly;
					members.Add(new FacetMember(
						f.Name,
						GeneratorUtilities.GetTypeNameWithNullability(f.Type),
						FacetMemberKind.Field,
						f.Type.IsValueType,
						false, // Fields don't have init-only
						isRequired,
						isReadOnly,
						null)); // No XML documentation for GenerateDtos
					addedMembers.Add(f.Name);
				}
			}

			var sourceNamespace = sourceSymbol.ContainingNamespace.IsGlobalNamespace
				? null
				: sourceSymbol.ContainingNamespace.ToDisplayString();

			return new GenerateDtosTargetModel(
				sourceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				sourceNamespace,
				targetNamespace ?? sourceNamespace,
				types,
				outputType,
				prefix,
				suffix,
				includeFields,
				generateConstructors,
				generateProjections,
				excludeProperties.ToImmutableArray(),
				members.ToImmutableArray(),
				useFullName);
		}
		catch (Exception)
		{
			// Swallow exceptions to prevent generator crashes
			// In a real scenario, you might want to emit a diagnostic instead
			return null;
		}
	}

	private static void GenerateDtosForModel(SourceProductionContext context, GenerateDtosTargetModel model)
	{
		context.CancellationToken.ThrowIfCancellationRequested();

		var sourceTypeName = GetSimpleTypeName(model.SourceTypeName);

		// Generate Create DTO
		if ((model.Types & DtoTypes.Create) != 0)
		{
			var createExclusions = new HashSet<string>(model.ExcludeProperties, System.StringComparer.OrdinalIgnoreCase);
			foreach (var idField in IdFieldPatterns)
			{
				createExclusions.Add(idField);
			}

			var createMembers = model.Members.Where(m => !createExclusions.Contains(m.Name)).ToImmutableArray();
			var createDtoName = BuildDtoName(sourceTypeName, "Create", "Request", model.Prefix, model.Suffix);

			var createCode = GenerateDtoCode(model, createDtoName, createMembers, "Create");

			context.AddSource($"{GenerateFileDtoFullName(model, createDtoName)}", SourceText.From(createCode, Encoding.UTF8));
		}

		// Generate Update DTO
		if ((model.Types & DtoTypes.Update) != 0)
		{
			var updateExclusions = new HashSet<string>(model.ExcludeProperties, System.StringComparer.OrdinalIgnoreCase);
			// Don't exclude ID from Update DTOs (needed for identification)

			var updateMembers = model.Members.Where(m => !updateExclusions.Contains(m.Name)).ToImmutableArray();
			var updateDtoName = BuildDtoName(sourceTypeName, "Update", "Request", model.Prefix, model.Suffix);

			var updateCode = GenerateDtoCode(model, updateDtoName, updateMembers, "Update");
			context.AddSource($"{GenerateFileDtoFullName(model, updateDtoName)}", SourceText.From(updateCode, Encoding.UTF8));
		}

		// Generate Upsert DTO
		if ((model.Types & DtoTypes.Upsert) != 0)
		{
			var upsertExclusions = new HashSet<string>(model.ExcludeProperties, System.StringComparer.OrdinalIgnoreCase);
			// Include ID in Upsert DTOs (can be null for create, populated for update)

			var upsertMembers = model.Members.Where(m => !upsertExclusions.Contains(m.Name)).ToImmutableArray();
			var upsertDtoName = BuildDtoName(sourceTypeName, "Upsert", "Request", model.Prefix, model.Suffix);

			var upsertCode = GenerateDtoCode(model, upsertDtoName, upsertMembers, "Upsert");
			context.AddSource($"{GenerateFileDtoFullName(model, upsertDtoName)}", SourceText.From(upsertCode, Encoding.UTF8));
		}

		// Generate Response DTO
		if ((model.Types & DtoTypes.Response) != 0)
		{
			var responseExclusions = new HashSet<string>(model.ExcludeProperties, System.StringComparer.OrdinalIgnoreCase);
			// Include all non-excluded properties for Response DTOs

			var responseMembers = model.Members.Where(m => !responseExclusions.Contains(m.Name)).ToImmutableArray();
			var responseDtoName = BuildDtoName(sourceTypeName, "", "Response", model.Prefix, model.Suffix);

			var responseCode = GenerateDtoCode(model, responseDtoName, responseMembers, "Response");
			context.AddSource($"{GenerateFileDtoFullName(model, responseDtoName)}", SourceText.From(responseCode, Encoding.UTF8));
		}

		// Generate Query DTO
		if ((model.Types & DtoTypes.Query) != 0)
		{
			var queryMembers = model.Members.Select(m => new FacetMember(
				m.Name,
				GeneratorUtilities.MakeNullable(m.TypeName),
				m.Kind,
				m.IsInitOnly,
				false)) // Make all properties optional in Query DTOs
				.ToImmutableArray();

			var queryDtoName = BuildDtoName(sourceTypeName, "", "Query", model.Prefix, model.Suffix);

			var queryCode = GenerateDtoCode(model, queryDtoName, queryMembers, "Query");
			context.AddSource($"{GenerateFileDtoFullName(model, queryDtoName)}", SourceText.From(queryCode, Encoding.UTF8));
		}
	}

	private static string BuildDtoName(string sourceTypeName, string prefix, string suffix, string? customPrefix, string? customSuffix)
	{
		var name = sourceTypeName;

		if (!string.IsNullOrWhiteSpace(customPrefix))
			name = customPrefix + name;

		if (!string.IsNullOrWhiteSpace(prefix))
			name = prefix + name;

		if (!string.IsNullOrWhiteSpace(suffix))
			name = name + suffix;

		if (!string.IsNullOrWhiteSpace(customSuffix))
			name = name + customSuffix;

		return name;
	}

	private static string GenerateFileDtoFullName(GenerateDtosTargetModel model, string dtoName)
	{
		if (!model.UseFullName)
		{
			return $"{dtoName}.g.cs";
		}

		var ns = string.IsNullOrEmpty(model.SourceNamespace) ? "Global" : model.SourceNamespace;
		var baseName = $"{ns}.{dtoName}";
		var safeName = baseName.GetSafeName();

		return $"{safeName}.g.cs";
	}

	private static string GetSimpleTypeName(string fullyQualifiedName)
	{
		var parts = fullyQualifiedName.Split('.');
		return parts[parts.Length - 1];
	}


	private static string GenerateDtoCode(GenerateDtosTargetModel model, string dtoName, ImmutableArray<FacetMember> members, string purpose)
	{
		var sb = new StringBuilder();
		GenerateFileHeader(sb);
		sb.AppendLine("using System;");
		sb.AppendLine("using System.Linq.Expressions;");
		sb.AppendLine();

		// Nullable must be enabled in generated code with a directive
		var hasNullableRefTypeMembers = model.Members.Any(m => !m.IsValueType && m.TypeName.EndsWith("?"));
		if (hasNullableRefTypeMembers)
		{
			sb.AppendLine("#nullable enable");
			sb.AppendLine();
		}

		if (!string.IsNullOrWhiteSpace(model.TargetNamespace))
		{
			sb.AppendLine($"namespace {model.TargetNamespace};");
			sb.AppendLine();
		}

		var keyword = model.OutputType switch
		{
			OutputType.Class => "class",
			OutputType.Record => "record",
			OutputType.RecordStruct => "record struct",
			OutputType.Struct => "struct",
			_ => "record"
		};

		var sourceTypeName = GetSimpleTypeName(model.SourceTypeName);

		sb.AppendLine($"/// <summary>");
		sb.AppendLine($"/// Generated {purpose} DTO for {sourceTypeName}.");
		sb.AppendLine($"/// </summary>");

		// Add [Facet] attribute to make it work with extension methods
		sb.AppendLine($"[Facet.Facet(typeof({model.SourceTypeName}))]");

		var hasInitOnlyProperties = members.Any(m => m.IsInitOnly);

		sb.AppendLine($"public {keyword} {dtoName}");
		sb.AppendLine("{");

		foreach (var member in members)
		{
			if (member.Kind == FacetMemberKind.Property)
			{
				var propDef = $"public {member.TypeName} {member.Name}";

				if (member.IsInitOnly)
				{
					propDef += " { get; init; }";
				}
				else
				{
					propDef += " { get; set; }";
				}

				if (member.IsRequired)
				{
					propDef = $"required {propDef}";
				}

				sb.AppendLine($"    {propDef}");
			}
			else
			{
				var fieldDef = $"public";
				if (member.IsReadOnly)
				{
					fieldDef += " readonly";
				}
				fieldDef += $" {member.TypeName} {member.Name}";

				// For readonly fields, we need to provide a default value since they can't be assigned in constructor
				if (member.IsReadOnly)
				{
					var defaultValue = GeneratorUtilities.GetDefaultValueForType(member.TypeName);
					fieldDef += $" = {defaultValue}";
				}

				fieldDef += ";";

				if (member.IsRequired && !member.IsReadOnly)
				{
					fieldDef = $"required {fieldDef}";
				}

				sb.AppendLine($"    {fieldDef}");
			}
		}

		// Generate constructor if requested
		if (model.GenerateConstructors)
		{
			sb.AppendLine();
			sb.AppendLine($"    /// <summary>");
			sb.AppendLine($"    /// Initializes a new instance of the <see cref=\"{dtoName}\"/> class from the specified <see cref=\"{sourceTypeName}\"/>.");
			sb.AppendLine($"    /// </summary>");
			sb.AppendLine($"    /// <param name=\"source\">The source <see cref=\"{sourceTypeName}\"/> object to copy data from.</param>");

			var hasRequiredProperties = model.Members.Any(m => m.IsRequired);
			if (hasRequiredProperties)
			{
				sb.AppendLine("    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]");
			}

			sb.AppendLine($"    public {dtoName}({model.SourceTypeName} source)");
			sb.AppendLine("    {");

			// Only assign to non-init-only properties and non-readonly fields
			var assignableMembers = members.Where(x => !x.IsInitOnly && !x.IsReadOnly).ToList();

			if (assignableMembers.Any())
			{
				foreach (var member in assignableMembers)
				{
					sb.AppendLine($"        this.{member.Name} = source.{member.Name};");
				}
			}
			else
			{
				// If there are no assignable members, add a comment to explain
				sb.AppendLine("        // No assignable members to initialize from source");
				sb.AppendLine("        // (all members are either init-only properties or readonly fields with default values)");
			}

			sb.AppendLine("    }");

			// Add parameterless constructor
			sb.AppendLine();
			sb.AppendLine($"    /// <summary>");
			sb.AppendLine($"    /// Initializes a new instance of the <see cref=\"{dtoName}\"/> class with default values.");
			sb.AppendLine($"    /// </summary>");
			sb.AppendLine($"    public {dtoName}()");
			sb.AppendLine("    {");
			sb.AppendLine("    }");

			// Add static factory method for types with init-only properties or readonly fields
			if (hasInitOnlyProperties || members.Any(m => m.IsReadOnly))
			{
				sb.AppendLine();
				sb.AppendLine($"    /// <summary>");
				sb.AppendLine($"    /// Creates a new instance of <see cref=\"{dtoName}\"/> from the specified <see cref=\"{sourceTypeName}\"/> with init-only properties.");
				sb.AppendLine($"    /// </summary>");
				sb.AppendLine($"    /// <param name=\"source\">The source <see cref=\"{sourceTypeName}\"/> object to copy data from.</param>");
				sb.AppendLine($"    /// <returns>A new <see cref=\"{dtoName}\"/> instance with all properties initialized from the source.</returns>");

				if (members.Any(m => m.IsReadOnly))
				{
					sb.AppendLine($"    /// <remarks>");
					sb.AppendLine($"    /// Note: Readonly fields will use their default values and cannot be copied from the source.");
					sb.AppendLine($"    /// </remarks>");
				}

				sb.AppendLine($"    public static {dtoName} FromSource({model.SourceTypeName} source)");
				sb.AppendLine("    {");
				sb.AppendLine($"        return new {dtoName}");
				sb.AppendLine("        {");

				// Only include non-readonly fields in the object initializer
				var initializableMembers = members.Where(m => !m.IsReadOnly).ToList();
				for (var i = 0; i < initializableMembers.Count; i++)
				{
					var member = initializableMembers[i];
					var comma = i == initializableMembers.Count - 1 ? "" : ",";
					sb.AppendLine($"            {member.Name} = source.{member.Name}{comma}");
				}

				sb.AppendLine("        };");
				sb.AppendLine("    }");
			}
		}

		// Generate projection if requested
		if (model.GenerateProjections)
		{
			sb.AppendLine();
			sb.AppendLine($"    /// <summary>");
			sb.AppendLine($"    /// Gets the projection expression for converting <see cref=\"{sourceTypeName}\"/> to <see cref=\"{dtoName}\"/>.");
			sb.AppendLine($"    /// Use this for LINQ and Entity Framework query projections.");
			sb.AppendLine($"    /// </summary>");
			sb.AppendLine($"    /// <value>An expression tree that can be used in LINQ queries for efficient database projections.</value>");
			sb.AppendLine($"    /// <example>");
			sb.AppendLine($"    /// <code>");
			sb.AppendLine($"    /// var dtos = context.{sourceTypeName}s");
			sb.AppendLine($"    ///     .Where(x => x.IsActive)");
			sb.AppendLine($"    ///     .Select({dtoName}.Projection)");
			sb.AppendLine($"    ///     .ToList();");
			sb.AppendLine($"    /// </code>");
			sb.AppendLine($"    /// </example>");
			sb.AppendLine($"    public static Expression<Func<{model.SourceTypeName}, {dtoName}>> Projection =>");

			if (hasInitOnlyProperties || members.Any(m => m.IsReadOnly))
			{
				sb.AppendLine($"        source => new {dtoName}");
				sb.AppendLine("        {");

				// Only include non-readonly fields in the object initializer for projections too
				var initializableMembers = members.Where(m => !m.IsReadOnly).ToList();
				for (var i = 0; i < initializableMembers.Count; i++)
				{
					var member = initializableMembers[i];
					var comma = i == initializableMembers.Count - 1 ? "" : ",";
					sb.AppendLine($"            {member.Name} = source.{member.Name}{comma}");
				}

				sb.AppendLine("        };");
			}
			else
			{
				sb.AppendLine($"        source => new {dtoName}(source);");
			}
		}

		sb.AppendLine("}");

		return sb.ToString();
	}

	private static void GenerateFileHeader(StringBuilder sb)
	{
		sb.AppendLine("// <auto-generated>");
		sb.AppendLine("//     This code was generated by the Facet GenerateDtos source generator.");
		sb.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
		sb.AppendLine("//     the code is regenerated.");
		sb.AppendLine("// </auto-generated>");
		sb.AppendLine();
		sb.AppendLine("#nullable enable");
		sb.AppendLine();
	}


	private static T GetNamedArg<T>(
		ImmutableArray<KeyValuePair<string, TypedConstant>> args,
		string name,
		T defaultValue)
	{
		var arg = args.FirstOrDefault(kv => kv.Key == name);
		if (arg.Key == null) return defaultValue;

		var value = arg.Value.Value;
		if (value == null) return defaultValue;

		if (typeof(T).IsEnum && value is int intValue)
		{
			return (T) Enum.ToObject(typeof(T), intValue);
		}

		if (value is T t) return t;

		try
		{
			return (T) Convert.ChangeType(value, typeof(T));
		}
		catch
		{
			return defaultValue;
		}
	}

}