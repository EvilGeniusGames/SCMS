using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Services.Template
{
    public class TemplateParser
    {
        private const int MaxRecursionDepth = 25;

        public string Parse(string template, object model)
        {
            return Parse(template, model, 0);
        }

        private string Parse(string template, object model, int depth)
        {
            if (depth > MaxRecursionDepth || string.IsNullOrWhiteSpace(template) || model == null)
                return template;

            var output = new StringBuilder();
            for (int i = 0; i < template.Length;)
            {
                if (MatchAt(template, i, "{{#each ", out int eachStart))
                {
                    int start = i + 8;
                    int end = template.IndexOf("}}", start);
                    var collectionName = template.Substring(start, end - start).Trim();

                    int blockStart = end + 2;
                    int blockEnd = FindBlockEnd(template, blockStart, "each");
                    var blockContent = template.Substring(blockStart, blockEnd - blockStart);

                    var prop = model.GetType().GetProperty(collectionName);
                    var items = prop?.GetValue(model) as IEnumerable;

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            output.Append(Parse(blockContent, item, depth + 1));
                        }
                    }

                    i = blockEnd + 9; // skip {{/each}}
                }
                else if (MatchAt(template, i, "{{#if ", out int ifStart))
                {
                    int start = i + 6;
                    int end = template.IndexOf("}}", start);
                    var propName = template.Substring(start, end - start).Trim();

                    int blockStart = end + 2;
                    int blockEnd = FindBlockEnd(template, blockStart, "if");
                    string trueBlock, falseBlock = "";

                    int elseIndex = FindElse(template, blockStart, blockEnd);
                    if (elseIndex != -1)
                    {
                        trueBlock = template.Substring(blockStart, elseIndex - blockStart);
                        falseBlock = template.Substring(elseIndex + 8, blockEnd - (elseIndex + 8));
                    }
                    else
                    {
                        trueBlock = template.Substring(blockStart, blockEnd - blockStart);
                    }

                    var prop = model.GetType().GetProperty(propName);
                    var value = prop?.GetValue(model);

                    bool condition = value switch
                    {
                        null => false,
                        bool b => b,
                        int iVal => iVal != 0,
                        string s => !string.IsNullOrWhiteSpace(s),
                        IEnumerable e => e.Cast<object>().Any(),
                        _ => true
                    };

                    output.Append(Parse(condition ? trueBlock : falseBlock, model, depth + 1));
                    i = blockEnd + 6; // skip {{/if}}
                }
                else if (MatchAt(template, i, "{{", out int varStart))
                {
                    int end = template.IndexOf("}}", varStart + 2);
                    var key = template.Substring(varStart + 2, end - varStart - 2).Trim();

                    var prop = model.GetType().GetProperty(key);
                    var value = prop?.GetValue(model)?.ToString() ?? "";

                    output.Append(value);
                    i = end + 2;
                }
                else
                {
                    output.Append(template[i]);
                    i++;
                }
            }

            return output.ToString();
        }

        private static bool MatchAt(string src, int index, string match, out int matchStart)
        {
            matchStart = index;
            return src.Length >= index + match.Length && src.Substring(index, match.Length) == match;
        }

        private static int FindBlockEnd(string template, int start, string type)
        {
            int nesting = 1;
            int i = start;

            while (i < template.Length)
            {
                if (template.Substring(i).StartsWith("{{#" + type))
                    nesting++;
                else if (template.Substring(i).StartsWith("{{/" + type + "}}"))
                    nesting--;

                if (nesting == 0)
                    return i;

                i++;
            }

            throw new Exception($"Unmatched {{#{type}}} block");
        }

        private static int FindElse(string template, int start, int end)
        {
            int i = start;
            while (i < end - 8)
            {
                if (template.Substring(i, 8) == "{{else}}")
                    return i;
                i++;
            }
            return -1;
        }
    }
}
