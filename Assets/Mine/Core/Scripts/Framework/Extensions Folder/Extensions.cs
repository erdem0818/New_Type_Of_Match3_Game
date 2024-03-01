using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.Extensions_Folder
{
    public static class Extensions
    {
        public static bool AnyOut<TSource>(this IEnumerable<TSource> enumerable, Func<TSource, bool> predicate, out TSource t)
        {
            t = default;
            foreach (var source in enumerable)
            {
                if (!predicate(source)) continue;
                t = source;
                return true;
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
            return $"<size={size}>{str}</size>";
        }
        
        //Color
        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Vector2 WithX(this Vector2 vector2, float x)
        {
            return new Vector2(x, vector2.y);
        }
        
        public static Vector2 WithY(this Vector2 vector2, float y)
        {
            return new Vector2(vector2.x, y);
        }
    }
}

