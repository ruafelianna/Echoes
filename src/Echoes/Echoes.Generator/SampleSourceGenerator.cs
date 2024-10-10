using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Tommy;

namespace Echoes.Generator;

[Generator]
public class SampleSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this generator.
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var translationFiles = FindRelevantFiles(context.AdditionalFiles);

        foreach (var file in translationFiles)
        {
            GenerateKeysFile(file, context);
        }
    }

    private static ImmutableArray<AdditionalText> FindRelevantFiles(ImmutableArray<AdditionalText> additionalFiles)
    {
        var translationFiles = new List<AdditionalText>();

        foreach (var additionalFile in additionalFiles)
        {
            if (additionalFile == null)
                continue;

            if (!additionalFile.Path.EndsWith(".toml"))
                continue;

            var text = additionalFile.GetText();

            if (text == null)
                continue;

            var stringText = text.ToString();

            if (stringText.Contains("[echoes_config]"))
                translationFiles.Add(additionalFile);

        }

        return translationFiles.ToImmutableArray();
    }

    private static ImmutableArray<string>? ExtractTranslationKeys(AdditionalText translationFile, GeneratorExecutionContext context)
    {
        var keys = new List<string>();

        var text = translationFile.GetText()?.ToString() ?? string.Empty;
        var reader = new StringReader(text);
        var parser = new TOMLParser(reader);
        var root = parser.Parse();

        if (!root.RawTable.TryGetValue("echoes_config", out var echoesConfig))
            return null;

        if (!root.RawTable.TryGetValue("translations", out var translations))
            return null;

        foreach (var pair in translations.AsTable.RawTable)
        {
            if (pair.Value.IsString)
            {
                  keys.Add(pair.Key);
            }
        }

        return keys.ToImmutableArray();
    }

    private static void GenerateKeysFile (AdditionalText translationFile, GeneratorExecutionContext context)
    {
        var keys = ExtractTranslationKeys(translationFile, context);
        var className =
            Path
                .GetFileNameWithoutExtension(translationFile.Path)
                .Replace(".", "_");

        var projectFolder = context.GetCallingPath();
        var sourceFile = translationFile.Path;

        var trimmedSourceFile = sourceFile;

        if (sourceFile.StartsWith(projectFolder))
        {
            trimmedSourceFile = sourceFile.Substring(projectFolder.Length);
        }

        var sb = new StringBuilder();

        sb.AppendLine($"using Echoes;");
        sb.AppendLine($"using System;");
        sb.AppendLine($"using System.Reflection;");
        sb.AppendLine($"");
        sb.AppendLine($"namespace Echoes;");
        sb.AppendLine("");
        sb.AppendLine($"// {projectFolder}");
        sb.AppendLine($"// {sourceFile}");
        sb.AppendLine($"// <auto-generated/>");
        sb.AppendLine($"public static class {className}");
        sb.AppendLine("{");

        sb.AppendLine($"\tprivate static readonly string _file = \"{trimmedSourceFile}\";");
        sb.AppendLine($"\tprivate static readonly Assembly _assembly = typeof({className}).Assembly;");

        foreach (var key in keys)
        {
            sb.AppendLine($"\tpublic static TranslationUnit {key} => new TranslationUnit(_assembly, _file, \"{key}\");");
        }

        sb.AppendLine("}");

        var text = sb.ToString();

        context.AddSource(className + ".g.cs", text);
    }
}