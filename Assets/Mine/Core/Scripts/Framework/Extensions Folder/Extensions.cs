using System;
using System.Collections.Generic;

namespace Assets.Mine.Core.Scripts.Framework.Extensions
{
    public static class Extensions
    {
        public static bool AnyOut<TSource>(this IEnumerable<TSource> enumerable, Func<TSource, bool> predicate, out TSource t)
        {
            t = default;
            foreach (var item in enumerable)
            {
                if(predicate(item))
                {
                    t = item;
                    return true;
                }
            }   

            return false;
        }
    }
}

