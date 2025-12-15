// Stub for ObscuredInt to function like a normal int when AntiCheat is missing
// Placed in Global Namespace to match usage in GameItemCollection.cs without imports

[System.Serializable]
public struct ObscuredInt : System.IEquatable<ObscuredInt>, System.IFormattable
{
    [UnityEngine.SerializeField] private int value; // Not actual obfuscation, just wrapper

    public ObscuredInt(int value)
    {
        this.value = value;
    }

    // Implicit conversions to/from int
    public static implicit operator ObscuredInt(int value) => new ObscuredInt(value);
    public static implicit operator int(ObscuredInt value) => value.value;
    
    // Operators
    public static ObscuredInt operator ++(ObscuredInt input) => new ObscuredInt(input.value + 1);
    public static ObscuredInt operator --(ObscuredInt input) => new ObscuredInt(input.value - 1);

    // Overrides
    public override string ToString() => value.ToString();
    public string ToString(string format, System.IFormatProvider formatProvider) => value.ToString(format, formatProvider);
    public override int GetHashCode() => value.GetHashCode();
    public override bool Equals(object obj) => obj is ObscuredInt oi && Equals(oi);
    public bool Equals(ObscuredInt other) => value == other.value;
}
