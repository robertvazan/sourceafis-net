using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public sealed class TopListF<V>
        where V : new()
    {
        struct Item
        {
            public float Key;
            public V Value;

            public Item(float key, V value)
            {
                Key = key;
                Value = value;
            }
        }

        readonly int Capacity;
        List<Item> List = new List<Item>();

        public TopListF(int capacity)
        {
            Capacity = capacity;
        }

        public void Add(float key, V value)
        {
            List.Add(new Item(key, value));
            if (List.Count > 2 * Capacity)
                Sort();
        }

        void Sort()
        {
            List.Sort((left, right) => Calc.Compare(left.Key, right.Key));
            if (List.Count > Capacity)
                List.RemoveRange(Capacity, List.Count - Capacity);
        }

        public V[] GetValues()
        {
            Sort();
            return List.ConvertAll<V>(item => item.Value).ToArray();
        }
    }
}
