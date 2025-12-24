using System.Text;
using System.Text.RegularExpressions;

namespace LogInterpreter.CLI.Customizations;

/// <summary>
/// Experimental class
/// USAGE:
/// string template = "*Id:{id},*Name:{name},*";
/// string line = "Akármi Id: 42, Version: 3, Name: Name1, stb";
/// var result = TemplateParser.ParseFromTemplate(line, template);
/// foreach (var kvp in result)
///     Console.WriteLine($"{kvp.Key} = {kvp.Value}");
/// </summary>
internal static class TemplateParser
{
    public static Dictionary<string, string> ParseFromTemplate(string line, string template)
    {
        var result = new Dictionary<string, string>();

        // Regex sablon építése a template alapján
        var regexPattern = new StringBuilder();
        int i = 0;
        while (i < template.Length)
        {
            if (template[i] == '*')
            {
                regexPattern.Append(".*?");
                i++;
            }
            else if (template[i] == '{')
            {
                int end = template.IndexOf('}', i);
                if (end == -1) throw new FormatException("Hiányzó '}' a template-ben.");

                string name = template.Substring(i + 1, end - i - 1);
                regexPattern.Append($@"(?<{name}>.*?)");
                i = end + 1;
            }
            else
            {
                // Fix szöveg ? escape-eljük
                regexPattern.Append(Regex.Escape(template[i].ToString()));
                i++;
            }
        }

        var regex = new Regex($"^{regexPattern}$");
        var match = regex.Match(line);

        if (!match.Success) return result;

        foreach (var name in regex.GetGroupNames())
        {
            if (int.TryParse(name, out _)) continue;
            result[name] = match.Groups[name].Value;
        }

        return result;
    }
}
