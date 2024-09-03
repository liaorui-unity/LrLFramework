//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class NetworkMsgServer : MonoBehaviour
    {
        internal static NetworkMsgServer self;
        internal Transport networkMsg;

        internal struct MsgInfo
        {
            internal ushort typeID;
            internal NetConnect connect;
            internal ArraySegment<byte> jsonBytes;
        }

        internal class CallInfo
        {
            internal Action<NetConnect, ArraySegment<byte>> call;
        }


        internal Dictionary<ushort, CallInfo> packetInfos = new Dictionary<ushort, CallInfo>();
        internal Dictionary<int, NetConnect> connectIds = new Dictionary<int, NetConnect>();
        internal Dictionary<Type, ushort> typeInfos = new Dictionary<Type, ushort>();
        internal Queue<MsgInfo> msgInfos = new Queue<MsgInfo>();
        internal List<ushort> typeIds = new List<ushort>();


        void Awake()
        {
            self = this;
            networkMsg = networkMsg ?? this.GetComponent<Transport>();

        }

        void Start()
        {
            networkMsg.OnServerDataReceived += Received;
            networkMsg.OnServerConnected += Connect;
            networkMsg.OnServerDisconnected += Disconnect;
            networkMsg.OnServerError += Error;

            networkMsg.ServerStart();
        }

        void Update()
        {
            networkMsg.ServerEarlyUpdate();

            if (msgInfos.Count > 0)
            {
                var msg = msgInfos.Dequeue();

                if (packetInfos.TryGetValue(msg.typeID, out CallInfo value))
                {
                    value.call?.Invoke(msg.connect, msg.jsonBytes);
                }
            }
        }


        private void LateUpdate()
        {
            networkMsg.ServerLateUpdate();
        }




        void OnDisable()
        {
            networkMsg.OnServerDataReceived -= Received;
            networkMsg.OnServerConnected -= Connect;
            networkMsg.OnServerDisconnected -= Disconnect;
            networkMsg.OnServerError -= Error;
        }


        internal void AddTypeMsg<T>(Action<NetConnect, T> handleType) where T : NetMessage
        {
            var uid = typeof(T).GetTypeID();

            Debug.Log("UID:"+uid);

            if (!typeIds.Contains(uid))
            {
                typeIds.Add(uid);
            }

            if (!packetInfos.ContainsKey(uid))
            {
                packetInfos[uid] = new CallInfo()
                {
                    call = (connect, msg) =>
                    {
                        Debug.Log(uid);
                        T type = msg.Bytes2Type<T>();
                        Debug.Log(type);
                        handleType?.Invoke(connect, type);
                    }
                };
            }
        }
        void Received(int id, ArraySegment<byte> byteSegments, int channel)
        {
            byte[] headByte = new byte[2];
            Buffer.BlockCopy(byteSegments.Array, byteSegments.Offset, headByte, 0, 2);

            byte[] bodyByte = new byte[byteSegments.Count - 2];
            Buffer.BlockCopy(byteSegments.Array, byteSegments.Offset + 2, bodyByte, 0, bodyByte.Length);

            var header = BitConverter.ToUInt16(headByte, 0);


            if (typeIds.Contains(header))
            {
                if (!connectIds.TryGetValue(id, out NetConnect connect))
                {
                    connect = new NetConnect(id);
                }

                msgInfos.Enqueue(new MsgInfo()
                {
                    connect = connect,
                    typeID = header,
                    jsonBytes = new ArraySegment<byte>(bodyByte)
                });
            }
            else
            {
                Debug.LogError($"typeMessage => {header} 未注册");
            }
        }

        void Connect(int id)
        {
            Debug.Log("id:" + id);
            if (!connectIds.ContainsKey(id)) connectIds.Add(id, new NetConnect(id));
        }

        void Disconnect(int id)
        {
            if (connectIds.ContainsKey(id)) connectIds.Remove(id);
        }

        void Error(int id, Exception ex)
        {
            Debug.LogError($"客户端 {id} => 报错：{ex}");
        }



        public static void Register<T>(Action<NetConnect, T> handleType) where T : NetMessage
        {
            self?.AddTypeMsg(handleType);
        }
        public static void Send<T>(NetConnect connect, T sendType) where T : NetMessage
        {
            ushort typeUID = 0;
            if (!(bool)self?.typeInfos.TryGetValue(typeof(T), out typeUID))
            {
                typeUID = typeof(T).GetTypeID();
                self?.typeInfos.Add(typeof(T), typeUID);
            }


            var bodyByte = sendType.Type2Bytes();
            var headByte = BitConverter.GetBytes(typeUID);

            byte[] bytes = new byte[bodyByte.Length + 2];

            Buffer.BlockCopy(headByte, 0, bytes, 0, 2);
            Buffer.BlockCopy(bodyByte, 0, bytes, 2, bodyByte.Length);

            self?.networkMsg.ServerSend(connect.valueId, new ArraySegment<byte>(bytes));

            NetworkPacketPools.Recover(sendType);
        }
    }
}
