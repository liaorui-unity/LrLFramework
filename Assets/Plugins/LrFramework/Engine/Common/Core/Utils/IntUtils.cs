using UnityEngine;
using System.Collections;
using System;


namespace Table
{
    /// <summary>
    /// 数字
    /// @author hannibal
    /// @time 2014-11-26
    /// </summary>
    public class IntUtils
    {
        static public bool HasFlag(uint a, uint b)
        {
            return ((a & b) == 0) ? false : true;
        }

        static public uint InsertFlag(ref uint a, uint b)
        {
            a |= b;
            return a;
        }
        static public uint RemoveFlag(ref uint a, uint b)
        {
            a ^= b;
            return a;
        }
        /// <summary>
        /// 获得一个数字的位数
        /// </summary>
        static public int GetNumberLength(ulong num)
        {
            if (num == 0) return 1;
            int len = 0;
            while (num > 0)
            {
                ++len;
                num = num / 10;
            }
            return len;
        }

        /**
         * 转16进制字符串*
         * 123->ffff
         */
        static public string ToHexString(long value)
        {
            string str = Convert.ToString(value, 16);
            if (str.Length == 1) str = "0" + str;
            return str;
        }
        //最接近2次方的值
        static public int upper_power_of_two(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        /** 比较两个单精度浮点是否相等
        * @param 
        * @return 如符合指定精度返回true;否则返回false.
        */
        static public bool FloatEqual(float fFirst, float fSecond, float fOffset)
        {
            return (Mathf.Abs(fFirst - fSecond) <= fOffset ? true : false);
        }

        /** 判断一个浮点数是否是0
        * @param fValue - 比较的数
        * @param fOffset - 比较精度
        * @return 如符合指定精度返回true;否则返回false.
        */
        static public bool FloatEqualZERO(float fValue, float fOffset = 0.00001f)
        {
            return (Mathf.Abs(fValue) <= fOffset ? true : false);
        }

    }
}