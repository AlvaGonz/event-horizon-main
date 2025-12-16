using System;

namespace Utilites
{
    public struct RandomState
    {
        public ulong Value;
        public static RandomState FromTickCount() => new RandomState { Value = (ulong)System.DateTime.Now.Ticks };
        public float NextFloat() => 0.5f;
        public int Next() => 0;
    }
}
