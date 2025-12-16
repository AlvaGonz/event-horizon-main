namespace Security
{
    // Stub for ObscuredLong to resolve compile errors
    // Matches the pattern of ObscuredInt stub
    public struct ObscuredLong : System.IEquatable<ObscuredLong>, System.IFormattable
    {
        [UnityEngine.SerializeField] private long value;

        public ObscuredLong(long value)
        {
            this.value = value;
        }

        // Implicit conversions
        public static implicit operator ObscuredLong(long value) => new ObscuredLong(value);
        public static implicit operator long(ObscuredLong value) => value.value;

        // Operators
        public static ObscuredLong operator ++(ObscuredLong input) => new ObscuredLong(input.value + 1);
        public static ObscuredLong operator --(ObscuredLong input) => new ObscuredLong(input.value - 1);

        // Overrides
        public override string ToString() => value.ToString();
        public string ToString(string format, System.IFormatProvider formatProvider) => value.ToString(format, formatProvider);
        public override int GetHashCode() => value.GetHashCode();
        public override bool Equals(object obj) => obj is ObscuredLong oi && Equals(oi);
        public bool Equals(ObscuredLong other) => value == other.value;
    }
}
