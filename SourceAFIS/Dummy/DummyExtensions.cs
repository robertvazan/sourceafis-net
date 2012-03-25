using System;
using System.Linq;
using System.Collections.Generic;

namespace SourceAFIS.Dummy
{
    public static class DummyExtensions
    {
        public static int RemoveAll<T>(this List<T> list, Predicate<T> match)
        {
            var remaining = list.Where(item =>!match(item)).ToList();
            int removed = list.Count - remaining.Count;
            list.Clear();
            list.AddRange(remaining);
            return removed;
        }
    }
}
