using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole.Algo
{
    public static class WordWrap
    {
        public static IEnumerable<string> Prefix(IEnumerable<string> input, string prefix)
        {
            var noPrefix = new string(' ', prefix.Length);
            bool first = true;

            foreach (var s in input)
            {
                yield return (first ? prefix : noPrefix) + s;
                first = false;
            }
        }

        public static IEnumerable<string> Wrap(string msg, int maxWidth)// string prefix)
        {
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
                            lineStart = FirstChar(msg, lineEnd);
                        }
                        break;
                    default:
                        if (i - lineStart >= maxWidth)
                        {
                            var lineEnd = (lastSpace > lineStart) ? lastSpace : i;
                            yield return msg.Substring(lineStart, Math.Max(0, lineEnd - lineStart));
                            lineStart = FirstChar(msg, lineEnd);
                        }
                        break;
                }
            }

            if (msg.Length != lineStart)
            {
                yield return msg.Substring(lineStart, msg.Length - lineStart);
            }
        }

        private static int FirstChar(string msg, int pos) {
            for (int i = pos; i < msg.Length; i++)
                if (!(msg[i] == ' ' || msg[i] == '\n'))
                    return i;
            return msg.Length;
        }
    }
}
