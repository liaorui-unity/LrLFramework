//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{

    public class NetMessage 
    {
        public ushort header;
    }

    public class NetConnect
    {
        public int valueId;
        public NetConnect(int value)
        {
            valueId = value;
        }
    }

    public class NetPacketInfo
    {
        /// <summary>
        /// 协议id
        /// </summary>
        public ushort Id = 0;
        /// <summary>
        /// 类型
        /// </summary>
        public System.Type Type;
    }

    public class NetworkPacketPools
    {
        private static Dictionary<System.Type, ushort> m_packet_UshortIDs = new Dictionary<System.Type, ushort>();
        /// <summary>
        /// 反射出的协议信息
        /// </summary>
        private static Dictionary<ushort, NetPacketInfo> m_packet_infos = new Dictionary<ushort, NetPacketInfo>();
        /// <summary>
        /// 对象池
        /// </summary>
        private static Dictionary<ushort, List<NetMessage>> m_packet_pools = new Dictionary<ushort, List<NetMessage>>();
#if DEBUG
        private static Dictionary<ushort, long> m_new_count = new Dictionary<ushort, long>();
#endif
        public static NetMessage Get(ushort id)
        {
            NetMessage packet = null;

            while (packet == null)
            {
                List<NetMessage> list;
                if (m_packet_pools.TryGetValue(id, out list) && list.Count > 0)
                {
                    packet = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    NetPacketInfo packet_info = GetPacketInfo(id);
                    if (packet_info == null)
                    {
                        Debug.LogError("PacketPools - 没有找到协议对应的类信息，请在类{0}添加特性PacketAttribute");
                        return null;
                    }
                    packet = (NetMessage)Activator.CreateInstance(packet_info.Type, true);
                    if (packet == null)
                    {
                        Debug.LogError("PacketPools - 创建协议失败:" + packet_info.Type.ToString());
                        return null;
                    }
#if DEBUG
                    //统计分配次数
                    long count = 0;
                    if (!m_new_count.TryGetValue(id, out count))
                        m_new_count.Add(id, 1);
                    else
                        m_new_count[id] = ++count;
#endif
                }
            }

            packet.header = id;
            return packet;
        }

        public static T Get<T>() where T : NetMessage
        {
            NetMessage packet = null;
            ushort id = 0;


            while (packet == null)
            {
                List<NetMessage> list;
                if (!m_packet_UshortIDs.ContainsKey(typeof(T)))
                {
                    id = GetID<T>();
                }
                else
                {
                    id = m_packet_UshortIDs[typeof(T)];
                }

                if (m_packet_pools.TryGetValue(id, out list) && list.Count > 0)
                {
                    packet = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    NetPacketInfo packet_info = GetPacketInfo(id);
                    if (packet_info == null)
                    {
                        Debug.LogError("PacketPools - 没有找到协议对应的类信息，请在类{0}添加特性PacketAttribute");
                        return default(T);
                    }
                    packet = (T)Activator.CreateInstance(packet_info.Type, true);
                    if (packet == null)
                    {
                        Debug.LogError("PacketPools - 创建协议失败:" + packet_info.Type.ToString());
                        return default(T);
                    }
#if DEBUG
                    //统计分配次数
                    long count = 0;
                    if (!m_new_count.TryGetValue(id, out count))
                        m_new_count.Add(id, 1);
                    else
                        m_new_count[id] = ++count;
#endif
                }
            }

            packet.header = id;
            return packet as T;
        }


        public static void Recover(NetMessage packet)
        {
            ushort id = packet.header;
            List<NetMessage> list;
            if (!m_packet_pools.TryGetValue(id, out list))
            {
                list = new List<NetMessage>();
                m_packet_pools.Add(id, list);
            }
            if (!list.Contains(packet)) list.Add(packet);
        }

        public static string ToString(bool is_print)
        {
#if DEBUG
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.AppendLine("PacketPools使用情况:");
            foreach (var obj in m_new_count)
            {
                ushort id = obj.Key;
                string one_line = id + " New次数:" + obj.Value;
                List<NetMessage> list;
                if (m_packet_pools.TryGetValue(id, out list))
                {
                    one_line += " 空闲数量:" + list.Count;
                }
                st.AppendLine(one_line);
            }
            if (is_print) Debug.Log(st.ToString());
            return st.ToString();
#else
            return string.Empty;
#endif
        }

        #region 协议信息
        public static void AddPacketInfo(NetPacketInfo info)
        {
            if (m_packet_infos.ContainsKey(info.Id))
            {
                Debug.LogError("PacketPools::AddPacketInfo - same id is register:" + info.Id.ToString());
                return;
            }

            m_packet_infos.Add(info.Id, info);
            m_packet_UshortIDs.Add(info.Type, info.Id);
        }
        public static NetPacketInfo GetPacketInfo(ushort id)
        {
            NetPacketInfo info = null;
            if (!m_packet_infos.TryGetValue(id, out info))
            {//可能没有提取特性
                if (!m_packet_infos.TryGetValue(id, out info))
                {
                    return null;
                }
            }
            else
            {
                info = m_packet_infos[id];
            }
            return info;
        }


        public static ushort GetID<T>()
        {
            return GetID(typeof(T));
        }
        static ushort GetID(System.Type type)
        {
            var id = (ushort)(GetStable(type.FullName) & 0xFFFF);
            AddPacketInfo(new NetPacketInfo() { Id = id, Type = type });
            return id;
        }
        static int GetStable(string text)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in text)
                    hash = hash * 31 + c;
                return hash;
            }
        }

        #endregion
    }

    public static class NetworkExtend
    {
        public static byte[] Type2Bytes<T>(this T message) where T : NetMessage
        {
            try
            {
                var jsonstr = JsonUtility.ToJson(message);
                return System.Text.Encoding.UTF8.GetBytes(jsonstr);
                // using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                // {
                //     ProtoBuf.Serializer.Serialize(ms, message);
                //     return ms.ToArray();
                // }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ProtoBuf 序列化失败：{ex.Message}");
                return null;
            }
        }
        public static T Bytes2Type<T>(this ArraySegment<byte> message) where T : NetMessage
        {
            try
            {
                var jsonstr = System.Text.Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
                return JsonUtility.FromJson<T>(jsonstr);
                
                // using (System.IO.MemoryStream ms = new System.IO.MemoryStream(message.Array))
                // {
                //     return ProtoBuf.Serializer.Deserialize<T>(ms);
                // }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ProtoBuf 反序列化失败：{ex.Message}");
                return default(T);
            }
        }

        public static ushort GetTypeID<T>(this T target)
        {
            int hash = 23;
            var typeName = target.ToString();
            unchecked
            {
                foreach (char c in typeName)
                    hash = hash * 31 + c;
            }
            return (ushort)(hash & 0xFFFF);
        }
    }
}
