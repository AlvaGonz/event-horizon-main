using System.Collections.Generic;

namespace Utilites.Collections
{
    public class SimpleInventory<T>
    {
        public void Add(T item, int count = 1) { }
        public int GetQuantity(T item) => 0;
        public IEnumerable<T> Items => new List<T>();
        public void Remove(T item, int count = 1) { }
        public void Clear() { }
    }
}
