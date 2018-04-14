using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace HackConsole
{
    public class RandomTable<T>
    {
        private (T Elem, int Odds)[] _elems;// = new (T Elem, int Odds)>();
        private int _totalOdds = 0;

        private RandomTable()
        {
        }

        static public RandomTable<T> FromString(string line, Func<string, T> func)
        {
            var ret = new RandomTable<T>();
            ret._elems = line.Split(',').Select(s =>
            {
                var t = s.Trim().Split(':');
                int odds = 1;
                if (t.Length > 1 && !int.TryParse(t[1], out odds))
                    throw new FormatException($"Invalid number format in token: {t[1]}");

                ret._totalOdds += odds;
                return (func(t[0]), ret._totalOdds);
            }).ToArray();
            return ret;
        }

        static public RandomTable<T> FromXML(XmlNode node, Func<XmlNode, T> func)
        {
            var ret = new RandomTable<T>();
            var ls = new List<(T elem, int odds)>();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                int odds = 1;
                if (childNode.Attributes["odds"] != null && !int.TryParse(childNode.Attributes["odds"].Value, out odds))
                    throw new FormatException($"Invalid number format in token: {childNode.Attributes["odds"].Value}");

                ret._totalOdds += odds;
                ls.Add((func(childNode), ret._totalOdds));
            }

            ret._elems = ls.ToArray();
            return ret;
        }

        internal T GetRand(Random rnd)
        {
            int odd = rnd.Next(_totalOdds);

            // Binary search for probability
            var min = 0;
            var max = _elems.Length - 1;
            while (min < max)
            {
                var mid = (min + max) / 2;

                if (_elems[mid].Odds <= odd)
                    min = mid + 1;
                else
                    max = mid;
            }
            return _elems[min].Elem;
        }
    }
}
