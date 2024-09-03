//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
//using Mirror;

namespace NET
{
    public class MessagePools : MonoBehaviour
    {
        static Dictionary<ushort, UnityAction<MessageInfo>> packets = 
           new Dictionary<ushort, UnityAction<MessageInfo>>();



        public static void Register<T>(Action<T> action) where T : MessageInfo
        {
            var header = MessagePacketPools.GetID<T>();

            if (!packets.ContainsKey(header))
            {
                packets.Add(header, (_) => { action?.Invoke(_ as T); });
            }
        }


        public static void UnRegister<T>() where T : MessageInfo
        {
            var header = MessagePacketPools.GetID<T>();

            if (packets.ContainsKey(header))
            {
                packets.Remove(header);
            }
        }


        public static void Send(MessageInfo msg)
        {
            if (packets.TryGetValue(msg.header, out UnityAction<MessageInfo> action))
            {
                action?.Invoke(msg);
                MessagePacketPools.Recover(msg);
            }
        }



        private void OnDestroy()
        {
            packets.Clear();
        }
    }
}



