using System.Linq;
using System.Text;

namespace Domain.Extensions;

public static class StringExtensions
{
    //characters '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!'
    //must be escaped with the preceding character '\'.
    public static string Escape(this string input)
    {
        var charsToEscape = new[]
            { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

        var sb = new StringBuilder();

        foreach (var ch in input)
        {
            if (!IsMetachar(ch))
            {
                sb.Append(ch);
            }
            else
            {
                sb.Append($"\\{ch}");
            }
        }

        return sb.ToString();

        bool IsMetachar(char ch) => charsToEscape.Contains(ch);
    }
}