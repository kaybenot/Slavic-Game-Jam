#nullable enable

using System;

namespace Helpers {
    public static class NullableUtil {
        
        public static TRet Map<TVal, TRet>(this TVal value, Func<TVal, TRet> converter) where TVal : class {
            return converter(value);
        }
        
        public static void Run<TVal>(this TVal value, Action<TVal> converter) where TVal : class {
            converter(value);
        }
    }
}