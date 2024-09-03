using System;
using UnityEngine;
using System.Collections;
using System.IO;

namespace Table
{
    /// <summary>
    /// 加密工具
    /// @author hannibal
    /// @time 2019-4-3
    /// </summary>
    public class EncryptUtils
    {
        public const int MAX_ENCRYPT_SIZE = 20;

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="buf">需要加密的内容</param>
        /// <param name="index">开始索引</param>
        /// <param name="dwSize">加密字节数</param>
        /// <param name="uKey">加密key</param>
        /// <returns></returns>
        private static ushort m_defaultKey = 13;
        public static byte[] Encrypt(byte[] buf, int index, int dwSize, ushort uKey)
        {
            if (uKey == 0) uKey = m_defaultKey;

            int idx = index;
            byte v = (byte)(uKey ^ 0xBA);
            while (dwSize > 0 && idx < buf.Length)
            {
                buf[idx] ^= v;
                idx++;
                dwSize--;
            }
            return buf;
        }
    }
}