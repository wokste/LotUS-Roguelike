using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Obsolete]
        public static IEnumerable<string> Wrap(this string msg, int maxWidth)// string prefix)
        {
            int FirstChar(int pos)
            {
                for (int i = pos; i < msg.Length; i++)
                    if (!(msg[i] == ' ' || msg[i] == '\n'))
                        return i;
                return msg.Length;
            }

            var lineStart = 0;
            var lastSpace = 0;

            for (var i = 0; i < msg.Length; i++)
            {
                var c = msg[i];

                switch (msg[i])
                {
                    case ' ':
                        lastSpace = i;
                        break;
                    case '\n':
                        {
                            var lineEnd = i;
                            yield return msg.Substring(lineStart, Math.Max(0, lineEnd - lineStart));
                            lineStart = FirstChar(lineEnd);
                        }
                        break;
                    default:
                        if (i - lineStart >= maxWidth)
                        {
                            var lineEnd = (lastSpace > lineStart) ? lastSpace : i;
                            yield return msg.Substring(lineStart, Math.Max(0, lineEnd - lineStart));
                            lineStart = FirstChar(lineEnd);
                        }
                        break;
                }
            }

            if (msg.Length != lineStart)
            {
                yield return msg.Substring(lineStart, msg.Length - lineStart);
            }
        }

        [Obsolete]
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
