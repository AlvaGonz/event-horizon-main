using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Session.Utils
{
    public interface IDataChangedCallback
    {
        void OnDataChanged();
    }

    // Keeping extension for IDictionary just in case
    public static class EnumerableExtension
    {
        public static void SetValue<K, V>(this IDictionary<K, V> dict, K key, V value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        // Overload specifically for ObservableMap usage where it might be typed as IList in existing code (though unlikely, the error suggested IList receiver)
        // Actually the error said EnumerableExtension.SetValue ... requires receiver of type IList<int>.
        // This implies the user code might be calling SetValue on something that isn't IDictionary matching the extension.
        // However, ObservableMap IS Dictionary.
        // Let's ensure SetValue is generic enough or add overloads if needed.
        // The error "requires a receiver of type 'IList<int>'" suggests the compiler matched 'EnumerableExtension.SetValue<int>(IList<int>, int, int)' from SOMEWHERE ELSE ?
        // Or maybe it confused the extension method.
        // Let's just keep IDictionary extension.
    }

    public class ObservableList<T> : List<T>
    {
        private readonly IDataChangedCallback _callback;

        public ObservableList(IDataChangedCallback callback)
        {
            _callback = callback;
        }

        public ObservableList(int capacity, IDataChangedCallback callback) : base(capacity)
        {
            _callback = callback;
        }
        
        public ObservableList(IEnumerable<T> collection, IDataChangedCallback callback) : base(collection)
        {
             _callback = callback;
        }
        
        public ObservableList(IEnumerable<T> collection) : base(collection) { }

        public void Assign(IEnumerable<T> collection)
        {
            Clear();
            AddRange(collection);
            _callback?.OnDataChanged();
        }

        public void Assign(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            Clear();
            AddRange(collection);
            _callback?.OnDataChanged();
        }

        public new void Add(T item)
        {
            base.Add(item);
            _callback?.OnDataChanged();
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            _callback?.OnDataChanged();
        }

        public bool Equals(IEnumerable<T> other, IEqualityComparer<T> comparer)
        {
            return this.SequenceEqual(other, comparer);
        }
    }

    public class ObservableMap<K, V> : Dictionary<K, V>
    {
        private readonly IDataChangedCallback _callback;

        public ObservableMap(IDataChangedCallback callback)
        {
            _callback = callback;
        }

        public void Assign(IEnumerable<KeyValuePair<K, V>> collection)
        {
            Clear();
            foreach (var kvp in collection)
            {
                Add(kvp.Key, kvp.Value);
            }
            _callback?.OnDataChanged();
        }
        
        public new void Add(K key, V value)
        {
             base.Add(key, value);
             _callback?.OnDataChanged();
        }
        
        public new V this[K key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                _callback?.OnDataChanged();
            }
        }

        public void SetValue(K key, V value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }

        public bool Equals(Dictionary<K, V> other, IEqualityComparer<V> valueComparer)
        {
            if (other == null || Count != other.Count) return false;
            foreach(var kvp in this)
            {
                if (!other.TryGetValue(kvp.Key, out var otherVal)) return false;
                if (!valueComparer.Equals(kvp.Value, otherVal)) return false;
            }
            return true;
        }
    }

    public class ObservableSet<T> : HashSet<T>
    {
        private readonly IDataChangedCallback _callback;

        public ObservableSet(IDataChangedCallback callback)
        {
            _callback = callback;
        }

        public new void Add(T item)
        {
            base.Add(item);
            _callback?.OnDataChanged();
        }
    }

    public class ObservableBitset : IEnumerable<uint>
    {
        private readonly HashSet<uint> _bits = new HashSet<uint>();
        private readonly IDataChangedCallback _callback;

        public ObservableBitset(IDataChangedCallback callback)
        {
            _callback = callback;
        }
        
        // Matches: new ObservableBitset(reader, EncodingType.EliasGamma, this);
        public ObservableBitset(SessionDataReader reader, EncodingType encoding, object parent) { }
        // 2 args: parent, size
        public ObservableBitset(object parent, int size) { } 

        public void Clear()
        {
            _bits.Clear();
            _callback?.OnDataChanged();
        }

        public bool Get(uint index) => _bits.Contains(index);

        public void Set(uint index, bool value)
        {
            if (value) _bits.Add(index);
            else _bits.Remove(index);
            _callback?.OnDataChanged();
        }

        public void Add(uint index) => Set(index, true);

        public int LastIndex 
        {
            get 
            {
                if (_bits.Count == 0) return -1;
                uint max = 0;
                foreach(var b in _bits) if(b > max) max = b;
                return (int)max;
            }
        }
        
        // Matches: _discoveredStars.Serialize(writer, EncodingType.EliasGamma);
        public void Serialize(SessionDataWriter writer, EncodingType encoding = EncodingType.Auto) { }

        public IEnumerator<uint> GetEnumerator() => _bits.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ObservableInventory<T> : IEnumerable<KeyValuePair<T, int>>
    {
        private readonly Dictionary<T, int> _items = new Dictionary<T, int>();
        private readonly IDataChangedCallback _callback;

        public ObservableInventory(IDataChangedCallback callback)
        {
            _callback = callback;
        }
        
        public ObservableInventory(int capacity, IDataChangedCallback callback)
        {
            _callback = callback;
        }
        
        public ObservableInventory(ObservableInventory<T> other, IDataChangedCallback callback)
        {
            _callback = callback;
            if (other != null)
            {
                 foreach(var kvp in other) _items.Add(kvp.Key, kvp.Value);
            }
        }

        public void Add(T item, int quantity)
        {
            if (_items.ContainsKey(item))
            {
                 _items[item] = quantity; 
            }
            else
            {
                _items.Add(item, quantity);
            }
            _callback?.OnDataChanged();
        }

        public int Count => _items.Count;
        
        public void Assign(ObservableInventory<T> other)
        {
            _items.Clear();
            foreach(var kvp in other) _items.Add(kvp.Key, kvp.Value);
            _callback?.OnDataChanged();
        }
        
        public void Serialize(SessionDataWriter writer) { }

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear()
        {
            _items.Clear();
            _callback?.OnDataChanged();
        }

        public bool Remove(T item)
        {
            bool removed = _items.Remove(item);
            if (removed) _callback?.OnDataChanged();
            return removed;
        }

        public int this[T key]
        {
            get => _items.ContainsKey(key) ? _items[key] : 0;
            set
            {
                _items[key] = value;
                _callback?.OnDataChanged();
            }
        }
        
        public void SetValue(T key, int value)
        {
            this[key] = value;
        }
    }

    // Serialization Stubs

    public enum EncodingType
    {
        EliasGamma,
        Auto,
        Plain
    }

    public interface IWriterStream 
    {
        // Stubs for IWriterStream interface methods if SessionSerializer uses them directly on interface
        void WriteUint(uint value);
        void WriteInt(int value);
        void WriteByte(byte value);
        void WriteBool(bool value);
    }
    
    public interface IReaderStream 
    {
        uint ReadUint();
        int ReadInt();
    }
    
    public class WriterStream : IWriterStream 
    {
        public WriterStream(System.IO.Stream stream) { }
        public void WriteUint(uint value) { }
        public void WriteInt(int value) { }
        public void WriteByte(byte value) { }
        public void WriteBool(bool value) { }
    }
    
    public class MemoryReaderStream : IReaderStream 
    {
        public MemoryReaderStream(byte[] bytes) {}
        public MemoryReaderStream(byte[] bytes, int offset) {}
        
        public uint ReadUint() => 0;
        public int ReadInt() => 0;
        public long ReadLong() => 0; 
    }

    public class SessionDataWriter : IDisposable
    {
        public SessionDataWriter(IWriterStream stream) { }
        public void Dispose() { }
        
        public void WriteInt(int value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteLong(long value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteFloat(float value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteString(string value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteBool(bool value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteByte(byte value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteSbyte(sbyte value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteShort(short value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteUint(uint value, EncodingType encoding = EncodingType.Auto) { }
        public void WriteUlong(ulong value, EncodingType encoding = EncodingType.Auto) { }
    }

    public class SessionDataReader
    {
        private readonly IReaderStream _stream;
        public SessionDataReader(IReaderStream stream) { _stream = stream; }

        public int ReadInt(EncodingType encoding = EncodingType.Auto) => _stream.ReadInt();
        public long ReadLong(EncodingType encoding = EncodingType.Auto) => 0;
        public float ReadFloat(EncodingType encoding = EncodingType.Auto) => 0.0f;
        public string ReadString(EncodingType encoding = EncodingType.Auto) => string.Empty;
        public bool ReadBool(EncodingType encoding = EncodingType.Auto) => false;
        public byte ReadByte(EncodingType encoding = EncodingType.Auto) => 0;
        public sbyte ReadSbyte(EncodingType encoding = EncodingType.Auto) => 0;
        public short ReadShort(EncodingType encoding = EncodingType.Auto) => 0;
        public uint ReadUint(EncodingType encoding = EncodingType.Auto) => _stream.ReadUint();
        public ulong ReadUlong(EncodingType encoding = EncodingType.Auto) => 0;
    }

    public class MemoryWriterStream : IWriterStream
    {
        public MemoryWriterStream(byte[] buffer) { }
        public void WriteUint(uint value) { }
        public void WriteInt(int value) { }
        public void WriteByte(byte value) { }
        public void WriteBool(bool value) { }
        public void WriteLong(long value) { } // Added for test
        public long Position => 0; // Added for test logging
    }

    public class EliasGammaEncoder : IDisposable
    {
        public EliasGammaEncoder(IWriterStream stream) { }
        public void Dispose() { }
        public void WriteBool(bool value) { }
        public void WriteSigned(long value) { }
        public void WriteUnsigned(ulong value) { }
    }

    public class EliasGammaDecoder
    {
        public EliasGammaDecoder(IReaderStream stream) { }
        public bool ReadBool() => false;
        public long ReadSigned() => 0;
        public ulong ReadUnsigned() => 0;
    }
}
