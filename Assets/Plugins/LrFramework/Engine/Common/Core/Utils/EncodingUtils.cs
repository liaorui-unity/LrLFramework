using UnityEngine;
using System.Collections;
using System.Text;
using System;
using LogInfo;

namespace Table
{
    /// <summary>
    /// 编码转换
    /// @author hannibal
    /// @time 2015-1-22
    /// </summary>
    public class EncodingUtils
    {
        /// <summary>
        /// 实现多种编码方式的转换
        /// </summary>
        /// <param name="str">要转换的字符</param>
        /// <param name="From">从哪种方式转换，如UTF-8</param>
        /// <param name="To">转换成哪种编码,如GB2312</param>
        /// <returns>转换结果</returns>
        static public string ConvertStr(string str, string From, string To)
        {

            byte[] bs = System.Text.Encoding.GetEncoding(From).GetBytes(str);
            bs = System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(From), System.Text.Encoding.GetEncoding(To), bs);
            string res = System.Text.Encoding.GetEncoding(To).GetString(bs);
            return res;
        }

        /// <summary>
        /// url地址栏转换
        /// </summary>
        /// <returns>The encode.</returns>
        /// <param name="str">String.</param>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        /// <summary>
        /// string转base64
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ToBase64(string code)
        {
            try
            {
                string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));
                return str;
            }
            catch (Exception e)
            {
                Info.LogError("转换失败:" + e.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// base64转string
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string FromBase64(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        /// <summary>
        /// base64转byte[]
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static byte[] DecodeBase64Tobytes(string code)
        {
            string decode = FromBase64(code);
            byte[] bytes = Encoding.UTF8.GetBytes(decode);
            return bytes;
        }
    }
}