using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Table
{
    /// <summary>
    /// 公共
    /// @author hannibal
    /// @time 2014-11-14
    /// </summary>
    public class Utils
    {
        /**
        * A unique device identifier. It is guaranteed to be unique for every
        * device (Read Only). You can use it (for example) to store high scores in a central server
        * 一个唯一的设备标识符。这是保证为每一台设备是唯一的（只读）。可以使用它在中央服务器来储存高分表。
        */
        static public string DeviceID
        {
            get { return SystemInfo.deviceUniqueIdentifier; }
        }
        /// <summary>
        /// 判断是否大端
        /// </summary>
        /// <returns></returns>
        public static bool IsBigEndian()
        {
            int nCheck = 0x01aa;
            bool _isBigEndian = (nCheck & 0xff) == 0x01;
            return _isBigEndian;
        }
        public static Vector3 SafePosition(Vector3 pos, float min_value = 0.0f)
        {
            float x = pos.x, y = pos.y, z = pos.z;
            if (float.IsInfinity(x) || float.IsNaN(x)) x = min_value;
            if (float.IsInfinity(y) || float.IsNaN(y)) y = min_value;
            if (float.IsInfinity(z) || float.IsNaN(z)) z = min_value;
            return new Vector3(x, y, z);
        }
        public static Vector3 SafeScale(Vector3 scale, float min_value = 0.01f)
        {
            float x = scale.x, y = scale.y, z = scale.z;
            if (Mathf.Abs(x) < Mathf.Abs(min_value) || float.IsInfinity(x) || float.IsNaN(x)) x = min_value;
            if (Mathf.Abs(y) < Mathf.Abs(min_value) || float.IsInfinity(y) || float.IsNaN(y)) y = min_value;
            if (Mathf.Abs(z) < Mathf.Abs(min_value) || float.IsInfinity(z) || float.IsNaN(z)) z = min_value;
            return new Vector3(x, y, z);
        }

        public static bool IsInValid(Vector3 vec)
        {
            float x = vec.x, y = vec.y, z = vec.z;
            if (float.IsInfinity(x) || float.IsNaN(x)) return true;
            if (float.IsInfinity(y) || float.IsNaN(y)) return true;
            if (float.IsInfinity(z) || float.IsNaN(z)) return true;
            return false;
        }

        #region 工具

        /// <summary>
        /// 转字符串list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> Convert<T>(List<T> list)
        {
            List<string> out_list = new List<string>();
            if (list == null) return out_list;
            list.ForEach((v) => { out_list.Add(v.ToString()); });
            return out_list;
        }
        public static List<string> Convert<T>(int[] list)
        {
            List<string> out_list = new List<string>();
            if (list == null) return out_list;
            for(int i = 0; i < list.Length; ++i)
            {
                out_list.Add(list[i].ToString());
            }
            return out_list;
        }

        #endregion
    }
}