// Stubs for missing CodeWriter.ExpressionParser library
// Placed here to allow compilation of Database module
using System;

namespace CodeWriter.ExpressionParser
{
    public struct Variant 
    {
        // Stub basic conversions to allow usage
        public static implicit operator Variant(float f) => default;
        public static implicit operator Variant(int i) => default;
        public static implicit operator Variant(bool b) => default; // Added implicit boolean conversion
        
        public static implicit operator float(Variant v) => 0f;
        public static implicit operator int(Variant v) => 0;
        public static implicit operator bool(Variant v) => false;
        
        // Added properties required by generated code
        public int AsInt => 0;
        public float AsSingle => 0f;
        public bool AsBool => false;

        public override string ToString() => "VariantStub";
    }

    // Generic delegate/interface to differentiate from non-generic local Expression interface
    public delegate T Expression<T>(); 

    public interface IFunction<T> { }
    
    public class ExpressionContext<T>
    {
        public ExpressionContext(ExpressionContext<T> parent, Func<string, Expression<T>> varResolver, Func<string, IFunction<T>> funcResolver) {}
        public void RegisterVariable(string name, Expression<T> value) {}
    }

    public class VariantExpressionParser
    {
        public Expression<Variant> Compile(string expression, ExpressionContext<Variant> context, bool optimization) => () => default;
    }
}
