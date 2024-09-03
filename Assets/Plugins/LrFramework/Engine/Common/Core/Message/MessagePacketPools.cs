//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using Sailfish;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NET
{
    /// packet特性
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SyncPacketAttribute : System.Attribute { }


    public class MessageInfo
    {
        public ushort header { get; set; }
    }

    public class MessagePacketInfo
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


    public class MessagePacketPools
    {
        private static Dictionary<System.Type, ushort> m_packet_UshortIDs = new Dictionary<System.Type, ushort>();
        /// <summary>
        /// 反射出的协议信息
        /// </summary>
        private static Dictionary<ushort, MessagePacketInfo> m_packet_infos = new Dictionary<ushort, MessagePacketInfo>();
        /// <summary>
        /// 对象池
        /// </summary>
        private static Dictionary<ushort, List<MessageInfo>> m_packet_pools = new Dictionary<ushort, List<MessageInfo>>();
#if DEBUG
        private static Dictionary<ushort, long> m_new_count = new Dictionary<ushort, long>();
#endif
        public static MessageInfo Get(ushort id)
        {
            MessageInfo packet = null;

            while (packet == null)
            {
                List<MessageInfo> list;
                if (m_packet_pools.TryGetValue(id, out list) && list.Count > 0)
                {
                    packet = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    MessagePacketInfo packet_info = GetPacketInfo(id);
                    if (packet_info == null)
                    {
                        Debuger.LogError("PacketPools - 没有找到协议对应的类信息，请在类{0}添加特性PacketAttribute");
                        return null;
                    }
                    packet = (MessageInfo)Activator.CreateInstance(packet_info.Type, true);
                    if (packet == null)
                    {
                        Debuger.LogError("PacketPools - 创建协议失败:" + packet_info.Type.ToString());
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

        public static T Get<T>() where T : MessageInfo
        {
            MessageInfo packet = null;
            ushort id = 0;
            

            while (packet == null)
            {
                List<MessageInfo> list;
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
                    MessagePacketInfo packet_info = GetPacketInfo(id);
                    if (packet_info == null)
                    {
                        Debuger.LogError("PacketPools - 没有找到协议对应的类信息，请在类{0}添加特性PacketAttribute");
                        return default(T);
                    }
                    packet = (T)Activator.CreateInstance(packet_info.Type, true);
                    if (packet == null)
                    {
                        Debuger.LogError("PacketPools - 创建协议失败:" + packet_info.Type.ToString());
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



        public static void Recover(MessageInfo packet)
        {
            ushort id = packet.header;
            List<MessageInfo> list;
            if (!m_packet_pools.TryGetValue(id, out list))
            {
                list = new List<MessageInfo>();
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
                List<MessageInfo> list;
                if (m_packet_pools.TryGetValue(id, out list))
                {
                    one_line += " 空闲数量:" + list.Count;
                }
                st.AppendLine(one_line);
            }
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
#else
            return string.Empty;
#endif
        }

        #region 协议信息
        public static void AddPacketInfo(MessagePacketInfo info)
        {
            if (m_packet_infos.ContainsKey(info.Id))
            {
                Debuger.LogError("PacketPools::AddPacketInfo - same id is register:" + info.Id.ToString());
                return;
            }

            m_packet_infos.Add(info.Id, info);
            m_packet_UshortIDs.Add(info.Type,info.Id);
        }
        public static MessagePacketInfo GetPacketInfo(ushort id)
        {
            MessagePacketInfo info = null;
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
    
        /// <summary>
        /// 提取特性
        /// </summary>
        private static void ExtractAttribute(System.Reflection.Assembly assembly)
        {
            System.Reflection.Assembly.GetAssembly(typeof(MessageInfo));

            float start_time = Time.realtimeSinceStartup;
            //外部程序集
            List<System.Type> types = AttributeUtils.FindType<MessageInfo>(assembly, false);
            if (types != null)
            {
                foreach (System.Type type in types)
                {
                    SyncPacketAttribute attr = AttributeUtils.GetClassAttribute<SyncPacketAttribute>(type);
                    if (attr == null) continue;
                    AddPacketInfo(new MessagePacketInfo() { Id = GetID(type), Type = type });
                }
            }
            Debug.Log("PacketPools:ExtractAttribute 提取特性用时:" + (Time.realtimeSinceStartup - start_time));
        }


        public static ushort GetID<T>()
        {
            return GetID(typeof(T));
        }
        static ushort GetID(System.Type type)
        {
            var id = (ushort)(GetStable(type.FullName) & 0xFFFF);
            AddPacketInfo(new MessagePacketInfo() { Id = id, Type = type });
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
}

