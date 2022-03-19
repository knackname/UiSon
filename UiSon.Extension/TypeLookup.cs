// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;

namespace UiSon.Extension
{
    /// <summary>
    /// Static lookups for type related algs
    /// </summary>
    public static class TypeLookup
    {
        /// <summary>
        /// Identifies types for a switch statement
        /// </summary>
        public static Dictionary<Type, ValueishType> TypeToId = new Dictionary<Type, ValueishType>
        {
            { typeof(string), ValueishType._string},
            { typeof(sbyte), ValueishType._sbyte},
            { typeof(byte), ValueishType._byte},
            { typeof(short), ValueishType._short},
            { typeof(ushort), ValueishType._ushort},
            { typeof(int), ValueishType._int},
            { typeof(uint), ValueishType._uint},
            { typeof(long), ValueishType._long},
            { typeof(ulong), ValueishType._ulong},
            { typeof(nint), ValueishType._nint},
            { typeof(nuint), ValueishType._nuint},
            { typeof(float), ValueishType._float},
            { typeof(double), ValueishType._double},
            { typeof(decimal), ValueishType._decimal},
            { typeof(bool), ValueishType._bool},
            { typeof(char), ValueishType._char},
            { typeof(Type), ValueishType._type},
        };
    }
}
