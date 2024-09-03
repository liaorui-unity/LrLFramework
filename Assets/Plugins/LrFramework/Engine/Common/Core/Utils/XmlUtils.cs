using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Sailfish
{
    public class XmlUtils
    {
        /// <summary>
        /// 读xml节点
        /// </summary>
        private static XmlNodeElement m_XmlReadStream = new XmlNodeElement();
        static public XmlNodeElement ReadNode(XmlElement xe, string field)
        {
            m_XmlReadStream.Reset();
            if (xe == null || !xe.HasAttribute(field))
                return m_XmlReadStream;
            string str = xe.GetAttribute(field);
            m_XmlReadStream.Read(str.Trim());
            return m_XmlReadStream;
        }
    }

    public class XmlNodeElement
    {
        private string elementValue;

        public void Reset() { elementValue = ""; }
        public void Read(string str) { elementValue = str; }

        public override string ToString() { if (elementValue == "_")return ""; return elementValue; }
        public bool ToBool() { return ToInt64() != 0 ? true : false; }
        public byte ToByte() { return (byte)ToInt64(); }
        public sbyte ToSByte() { return (sbyte)ToInt64(); }
        public char ToChar() { return (char)ToInt64(); }
        public short ToInt16() { return (short)ToInt64(); }
        public ushort ToUInt16() { return (ushort)ToInt64(); }
        public int ToInt32() { return (int)ToInt64(); }
        public uint ToUInt32() { return (uint)ToInt64(); }
        public float ToFloat() { return (float)ToDecimal(); }
        public double ToDouble() { return (double)ToDecimal(); }
        public long ToInt64()
        {
            long v;
            v = long.TryParse(elementValue, out v) ? v : 0;
            return v;
        }
        public ulong ToUInt64()
        {
            ulong v;
            v = ulong.TryParse(elementValue, out v) ? v : 0;
            return v;
        }
        public decimal ToDecimal()
        {
            decimal v;
            v = decimal.TryParse(elementValue, out v) ? v : 0;
            return v;
        }
    }
}