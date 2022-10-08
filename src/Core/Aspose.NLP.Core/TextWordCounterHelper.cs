using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Aspose.NLP.Core
{
    public class TextWordCounterHelper
    {
        public Dictionary<string, int> FastWordOcurrensiesInSentensesCounter(string[] ss, string[] sp)
        {
            var c = new int[sp.Length];
            var dict = new Dictionary<string, int>();

            for (int x = 0; x < ss.Length; x++)
            {
                var currentSentense = ss[x];
                int currentSentenseLenght = ss[x].Length;

                for (int y = 0; y < sp.Length; y++)
                {
                    var currentPhrase = sp[y];
                    c[y] += (currentSentenseLenght - currentSentense.Replace(currentPhrase, String.Empty).Length) / currentPhrase.Length;
                }
            }

            for (int y = 0; y < sp.Length; y++)
            {
                dict.Add(sp[y], c[y]);
            }

            return dict;
        }

        public Dictionary<string, int> FastWordCounterParallel(string[] ss, string[] sf)
        {
            var c = new int[sf.Length];
            var dict = new Dictionary<string, int>();

            Parallel.For(0, sf.Length, 
                (x, loopState) =>
                {
                    var currentPhrase = sf[x];
                    for (int y = 0; y < ss.Length; y++)
                    {
                        var currentSentense = ss[y];
                        int currentSentenseLenght = currentSentense.Length;
                        c[x] += (currentSentenseLenght - currentSentense.Replace(currentPhrase, String.Empty).Length) / currentPhrase.Length;
                    }
                });

            for (int y = 0; y < sf.Length; y++)
            {
                dict.Add(sf[y], c[y]);
            }

            return dict;
        }

        public Dictionary<string, int> FastWordCounterRegExp(string[] ss, string[] sf)
        {
            var c = new int[sf.Length];
            var dic = new Dictionary<string, int>();

            for (int x = 0; x < ss.Length; x++)
            {
                for (int y = 0; y < sf.Length; y++)
                {
                    c[y] += Regex.Matches(ss[x], Regex.Escape(sf[y])).Count;
                }
            }

            for (int y = 0; y < sf.Length; y++)
            {
                dic.Add(sf[y], c[y]);
            }

            return dic;
        }

        public ConcurrentDictionary<string, int> FastWordCounterRegExpParallel(string[] ss, string[] sf)
        {
            var cdict = new ConcurrentDictionary<string, int>();

            var poptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0))
            };

            Parallel.ForEach(ss, poptions, s =>
            {
                var data = sf.AsParallel().Select(f => (f, Regex.Matches(s, Regex.Escape(f)).Count));
                data.ForAll(d => cdict.AddOrUpdate(d.f, d.Count, (key, currentValue) => currentValue + d.Count));
            });

            return cdict;
        }

    }
}