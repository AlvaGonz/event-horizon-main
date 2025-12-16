using System;
using System.Collections.Generic;

namespace Session
{
    // Stub for session utilities
    public static class Utils
    {
    }
}

namespace Session.Utils
{
    public interface IDataChangedCallback
    {
        void OnDataChanged();
    }

    public class ObservableList<T> : List<T>
    {
        public ObservableList(IDataChangedCallback callback) { }
        public ObservableList() { }
    }

    public class ObservableMap<K, V> : Dictionary<K, V>
    {
        public ObservableMap(IDataChangedCallback callback) { }
        public ObservableMap() { }
    }

    public class ObservableSet<T> : HashSet<T>
    {
        public ObservableSet(IDataChangedCallback callback) { }
        public ObservableSet() { }
    }

    public class ObservableBitset
    {
        public ObservableBitset(IDataChangedCallback callback) { }
        public ObservableBitset() { }
        public bool this[int index] { get => false; set { } }
    }
    
    public class ObservableInventory<T>
    {
        public ObservableInventory(IDataChangedCallback callback) { }
        public ObservableInventory() { }
        public void Add(T item, int amount) { }
        public int GetQuantity(T item) => 0;
    }

    public interface IWriterStream { }
    public interface IReaderStream { }

    public class SessionDataWriter
    {
        public void Write(object data) { }
    }

    public class SessionDataReader
    {
        public T Read<T>() => default;
    }
}
