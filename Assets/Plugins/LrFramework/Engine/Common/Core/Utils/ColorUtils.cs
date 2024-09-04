using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using LogInfo;

namespace Table
{
    /// <summary>
    /// 颜色
    /// @author hannibal
    /// @time 2015-2-3
    /// </summary>
    public class ColorUtils
    {
        /// <summary>
        /// color 转换hex
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }

        /// <summary>
        /// hex转换到color
        /// </summary>
        /// <returns></returns>
        public static Color HexToColor(string color)
        {
            if (color.Length == 8)
            {
                int v = int.Parse(color, System.Globalization.NumberStyles.HexNumber);
                //转换颜色
                return new Color(
                //int>>移位 去低位
                //&按位与 去高位
                ((float)(((v >> 24) & 255))) / 255,
                ((float)(((v >> 16) & 255))) / 255,
                ((float)((v >> 8) & 255)) / 255,
                ((float)((v >> 0) & 255)) / 255
                );
            }
            else if (color.Length == 6)
            {
                int v = int.Parse(color, System.Globalization.NumberStyles.HexNumber);
                //转换颜色
                return new Color(
                //int>>移位 去低位
                //&按位与 去高位
                ((float)(((v >> 16) & 255))) / 255,
                ((float)((v >> 8) & 255)) / 255,
                ((float)((v >> 0) & 255)) / 255
                );
            }
            else
                return Color.white;
        }
        /**
         * 16进制颜色转rgba
         * color = "ff(R)ff(G)ff(B)ff(A)"
         */
        static public void GetRGBA(string color, out int r, out int g, out int b, out int a)
        {
            if (color.Length != 8)
            {
                r = g = b = a = 255;
                Info.LogError("ColorUtils::GetRGBA - color error:" + color);
                return;
            }
            string str = color.Substring(0, 2);
            r = Convert.ToInt32(str, 16);

            str = color.Substring(2, 2);
            g = Convert.ToInt32(str, 16);

            str = color.Substring(4, 2);
            b = Convert.ToInt32(str, 16);

            str = color.Substring(6, 2);
            a = Convert.ToInt32(str, 16);
        }
    }
}