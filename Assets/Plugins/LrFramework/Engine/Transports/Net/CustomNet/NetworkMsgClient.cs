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
using UniRx;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Mirror
{


	public class NetworkMsgClient : MonoBehaviour
	{
		internal static NetworkMsgClient self;
		internal Transport networkMsg;

		internal struct MsgInfo
		{
			internal ushort typeID;
			internal ArraySegment<byte> jsonBytes;
		}

		internal class CallInfo
		{
			internal Action<ArraySegment<byte>> call;
		}

		internal Dictionary<ushort, CallInfo>       packetInfos = new Dictionary<ushort, CallInfo>();
		internal Dictionary<Type, ushort>             typeInfos = new Dictionary<Type, ushort>();
		internal Queue<MsgInfo>                        msgInfos = new Queue<MsgInfo>();
		internal List<ushort>                           typeIds = new List<ushort>();


		void Awake()
		{
			self = this;
			networkMsg = networkMsg ?? this.GetComponent<Transport>();
	
		}

	    void Start()
        {
			networkMsg.OnClientDataReceived += Received;
			networkMsg.OnClientConnected += Connect;
			networkMsg.OnClientDisconnected += Disconnect;
			networkMsg.OnClientError += Error;
		}

	

		void Update()
        {
			networkMsg.ClientEarlyUpdate();

			if (msgInfos.Count > 0)
			{
				var msg = msgInfos.Dequeue();

				if (packetInfos.TryGetValue(msg.typeID, out CallInfo value))
				{
					value.call?.Invoke(msg.jsonBytes);
				}
			}

			if (Input.GetKeyDown(KeyCode.C))
			{
				Debug.Log("连接");
				networkMsg.ClientConnect("192.168.31.88");
			}
        }

		private void LateUpdate()
		{
			networkMsg.ClientLateUpdate();
		}


		void OnDisable()
        {
			networkMsg.OnClientDataReceived -= Received;
			networkMsg.OnClientConnected -= Connect;
			networkMsg.OnClientDisconnected -= Disconnect;
			networkMsg.OnClientError -= Error;
		}




		internal void AddTypeMsg<T>(Action<T> handleType) where T : NetMessage
		{
			var uid = typeof(T).GetTypeID();

			if (!typeIds.Contains(uid))
			{
				typeIds.Add(uid);
			}

			if (!packetInfos.ContainsKey(uid))
			{
				packetInfos[uid] = new CallInfo()
				{
					call = (_) =>
					{
						handleType?.Invoke(_.Bytes2Type<T>());
					}
				};
			}
		}


		void Received(ArraySegment<byte> byteSegments, int channel)
		{
			byte[] headByte = new byte[2];
			Buffer.BlockCopy(byteSegments.Array, byteSegments.Offset, headByte, 0, 2);

			byte[] bodyByte = new byte[byteSegments.Count - 2];
			Buffer.BlockCopy(byteSegments.Array, byteSegments.Offset + 2, bodyByte, 0, bodyByte.Length);

			var header = BitConverter.ToUInt16(headByte, 0);

			if (typeIds.Contains(header))
			{
				msgInfos.Enqueue(new MsgInfo()
				{
					typeID = header,
					jsonBytes = new ArraySegment<byte>(bodyByte)
				}) ;
			}
			else
			{
                Debug.LogError($"typeMessage => {header} 未注册");
			}
		}



		void Connect()
		{
			Debug.Log("连接服务器成功");
		}

		void Disconnect()
		{
			Debug.Log("连接断开");
		}

		void Error(Exception ex)
		{
			Debug.LogError($"报错：{ex}");
		}


		public static void Register<T>(Action<T> handleType) where T : NetMessage
		{
			self?.AddTypeMsg(handleType);
		}

		public static void Send<T>(T sendType) where T : NetMessage
		{
			ushort typeUID = 0;
			if (!(bool)self?.typeInfos.TryGetValue(typeof(T), out typeUID))
			{
				typeUID = typeof(T).GetTypeID();
				self?.typeInfos.Add(typeof(T), typeUID);
			}

			var bodyByte = sendType.Type2Bytes();
			var headByte = BitConverter.GetBytes(typeUID);
	
			byte[] bytes = new byte[bodyByte.Length +  2];	

			Buffer.BlockCopy(headByte, 0, bytes, 0, 2);
			Buffer.BlockCopy(bodyByte, 0, bytes, 2, bodyByte.Length);

			self?.networkMsg.ClientSend(new ArraySegment<byte>(bytes, 0, bytes.Length));

			NetworkPacketPools.Recover(sendType);
		}
	}

}
