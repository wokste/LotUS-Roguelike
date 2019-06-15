using System.Collections.Generic;
using System.Text;

namespace HackConsole.Algo
{
    public static class StringExt
    {
        public static IEnumerable<string> Prefix(this IEnumerable<string> input, string prefix)
        {
            var noPrefix = new string(' ', prefix.Length);
            bool first = true;

            foreach (var s in input)
            {
                yield return (first ? prefix : noPrefix) + s;
                first = false;
            }
        }

        public static string CleanUp(this string source)
        {
            var sb = new StringBuilder(source.Length);

            bool firstChar = true;
            bool space = false;

            foreach (var c in source)
            {
                // Max 1 space to be added
                if (c == ' ')
                {
                    space = true;
                    continue;
                }
                if (space)
                {
                    space = false;
                    sb.Append(' ');
                }
                
                // ToUpper all first characters
                sb.Append(firstChar ? char.ToUpper(c) : c);
                if (c == '\t' || c == ' ' || c == '\n' || c == '\r')
                    continue;

                firstChar = (c == '.' || c == '?' || c == '!');
            }
            return sb.ToString();
        }
    }
}
