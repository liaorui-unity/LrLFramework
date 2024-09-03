//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;



namespace Sailfish
{
  
    public enum NetworkState
    {
        Server=0,
        Host,
        Client
    }


    [ProtoBuf.ProtoContract]
    public class Tyy : NetMessage
    {
        [ProtoBuf.ProtoMember(1)] public int jius;
        [ProtoBuf.ProtoMember(2)] public string tyii;
        [ProtoBuf.ProtoMember(3)] public Dictionary<string,Tu> Tus;
    }

    [ProtoBuf.ProtoContract]
    public class Tu 
    {
        [ProtoBuf.ProtoMember(1)] public string y;
    }


    public class ExampleTcp : MonoBehaviour 
    {
		public Transport transport;
        public bool isServer = false;
        public int  clientID;

        public NetworkState networkState = NetworkState.Server;

        public List<int> clientIDs = new List<int>();


        void Init()
        {    
            if (isServer)
                transport.OnServerConnected += (id) =>
                {
                    Connected(id);
                };
            else
                transport.OnClientConnected += () =>
                {
                    Connected(-1);
                };


            if (isServer)
                transport.OnServerDataReceived += (int id, System.ArraySegment<byte> msg, int channel) =>
                {
                    Received(id, msg, channel);
                };
            else
                transport.OnClientDataReceived += (System.ArraySegment<byte> msg, int channel) =>
                {
                    Received(-1, msg, channel);
                };

         
            if (isServer)
                transport.OnServerDisconnected += (id) =>
                {
                    Disconnected(id);
                };
            else
                transport.OnClientDisconnected += () =>
                {
                    Disconnected(-1);
                };

            if (isServer)
                transport.OnServerError += (id, ex) =>
                {
                    Error(id, ex);
                };
            else
                transport.OnClientError += (ex) =>
                {
                    Error(-1, ex);
                };


        }



        public void StartServer()
        {
            networkState = NetworkState.Server;
            Init();
        }

        public void StartHost()
        {
            networkState = NetworkState.Host;
            Init();
        }

        public void StartClient()
        {
            networkState = NetworkState.Client;
            Init();
        }



        public void Received(int id, ArraySegment<byte> msg, int channel)
        {
            //using (System.IO.MemoryStream bufferStream = new System.IO.MemoryStream())
            //{
            //    bufferStream.Write(msg.Array, msg.Offset, msg.Count);
            //}
         //   handleReceived?.Invoke(id, msg, channel);
        }


        public void Send(NetMessage message)
        {
            var bytes = message.Type2Bytes();
            //var writer = NetworkWriterPool.Get();
            //writer.Write(message);
            //var arraySegment= writer.ToArraySegment();

            Debug.Log(message);

            var arraySegment = new ArraySegment<byte>(bytes);
            if (isServer) transport.ServerSend(clientID, arraySegment);
            else          transport.ClientSend(arraySegment);
        }

        public void Send(byte[] bytes)
        {
            Debug.Log(transport);
            var arraySegment = new ArraySegment<byte>(bytes);
            if (isServer) transport.ServerSend(clientID, arraySegment);
            else          transport.ClientSend(arraySegment);
        }

        public void Send(ArraySegment<byte> arraySegment)
        {
            if (isServer) transport.ServerSend(clientID, arraySegment);
            else          transport.ClientSend(arraySegment);
        }


        public void Disconnected(int clientID)
        {
            if (clientID != -1)
                Debug.Log($"客户端:({clientID}) 断开连接");

            if (isServer)
                if (clientIDs.Contains(clientID))
                {
                    clientIDs.Remove(clientID);
                }
        }

        public void Connected(int clientID)
        {
            if (clientID != -1)
                Debug.Log($"客户端:({clientID}) 加入连接");

            if (isServer)
                if (!clientIDs.Contains(clientID))
                {
                     clientIDs.Add(clientID);
                }
        }


        public void Error(int clientID,Exception ex)
        {
            Debug.LogError($"clientID:{clientID} => {ex.Message}");
        }


        private void Update()
        {
            if (isServer) transport.ServerEarlyUpdate();
            else          transport.ClientEarlyUpdate();
        }


        private void LateUpdate()
        {
            if (isServer) transport.ServerLateUpdate();
            else          transport.ClientLateUpdate();
        }



        int width = Screen.width/2;
        int height = Screen.height/2;
        
        
        private void OnGUI()
        {
            Rect rect = new Rect(width - 200, height, 200, 50);
            Rect rect1 = new Rect(width, height, 200, 50);

            if (isServer)
            {
                if (GUI.Button(rect, "Server"))
                {
                    Init();
                    transport.ServerStart();
                 
                }
            }
            else
            { 
                if (GUI.Button(rect1, "Client"))
                {
                    Init();
                    transport.ClientConnect("192.168.31.88");
                 
                }
            }
        }
    }
}
