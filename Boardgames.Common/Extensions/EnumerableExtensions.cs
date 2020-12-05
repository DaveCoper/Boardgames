using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boardgames.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<T> FisherYatesShuffle<T>(this IEnumerable<T> items, Random rng)
        {
            var list = items.ToList();
            for (int n = list.Count - 1; n > 0; --n)
            {
                int k = rng.Next(n + 1);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }

            return list;
        }
    }
}
