using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.Extensions_Folder
{
    public static class Extensions
    {
        public static List<TTarget> ToListAsConvert<TSource, TTarget>(this IEnumerable<TSource> enumerable) where TTarget : class
        {
            List<TTarget> result = new List<TTarget>();
            
            foreach (var member in enumerable)
            {
                if (member is TTarget target)
                    result.Add(target);
                else
                    Debug.Log("null");
            }

            return result;
        }

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
        
        //UniRx Extensions

        #region UniRx

        public static IDisposable SubscribeIntToTextPro(this IObservable<int> source, TMP_Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = $"{x}");
        }

        #endregion
        
        #region String

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

        #endregion

        #region Color

        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        #endregion

        #region Vector

        public static Vector2 WithX(this Vector2 vector2, float x)
        {
            return new Vector2(x, vector2.y);
        }
        
        public static Vector2 WithY(this Vector2 vector2, float y)
        {
            return new Vector2(vector2.x, y);
        }

        public static Vector3 MultiplyByPercent(this Vector3 vector3, float percent)
        {
            return new Vector3(vector3.x.MultiplyByPercent(percent),
                vector3.y.MultiplyByPercent(percent),
                vector3.z.MultiplyByPercent(percent));
        }

        #endregion
        
        public static float MultiplyByPercent(this float f, float percent)
        {
            percent = Mathf.Clamp(percent, -100, 100);
            switch (percent)
            {
                case < 0:
                    f += (percent * f) / 100f;
                    return f;
                case > 0:
                    f -= (percent * f) / 100f;
                    return f;
                default:
                    return f;
            }
        }
    }
}

