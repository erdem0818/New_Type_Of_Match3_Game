using System;
using System.Collections.Generic;
using UnityEngine;

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

        public static string ToBold(this string str)
        {
            return $"<b>{str}</b>";
        }

        public static string ToColor(this string str, Color color)
        {
            var hexColor = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hexColor}>{str}</color>";
        }

        public static string ToItalic(this string str)
        {
            return $"<i>{str}</i>";
        }

        public static string ToSize(this string str, int size)
        {
            return string.Format("<size={0}>{1}</size>", size, str);
        }
    }
}

