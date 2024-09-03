using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Sailfish
{
    public class UDPSendSocket
    {
        private UdpClient m_socket = null;
        private Socket m_multipleSocket = null;
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="conn_idx"></param>
        public delegate void OnConnectClose();
        public event OnConnectClose OnClose;
        IPEndPoint ipep;


        public bool Init()
        {
            if (m_socket != null)
            {
                Debug.LogError("UDPSendSocket - 已经初始化过socket");
                return false;
            }
            try
            {
                {
                    IPEndPoint iPEnd = new IPEndPoint(IPAddress.Any, 0);
                    m_socket = new UdpClient(iPEnd);
                    m_socket.Client.ReceiveBufferSize = 1024 * 1024;
                    m_socket.Client.SendBufferSize = 1024 * 1024;
                    m_socket.Client.SendTimeout = 5000;
                    m_socket.Client.ReceiveTimeout = 5000;
                }
                Debug.Log("UDPSendSocket - 创建UDPSendSocket");
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }
        public void Close()
        {
            OnClose = null;
            if (m_socket != null)
            {
                try
                {
                    m_socket.Close();
                }
                catch (Exception) { }
                m_socket = null;
            }
        }
        /// <summary>
        /// 同步方式发送
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public int SendSync(byte[] buf, int len, string ip, ushort port)
        {
            if (m_socket == null)
                return 0;
            try
            {
                return m_socket.Send(buf, len, ip, port);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10051)
                {
                    Debug.LogException(e);
                    if (OnClose != null)
                    {
                        OnClose.Invoke();
                    }
                    this.Close();
                }
            }
            catch (System.Exception)
            {
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
            }
            return 0;
        }

        public int SendSync(byte[] buf, int len, IPEndPoint ip)
        {
            if (m_socket == null)
                return 0;
            try
            {
                return m_socket.Send(buf, len, ip);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10051)
                {
                    Debug.LogException(e);
                    if (OnClose != null)
                    {
                        OnClose.Invoke();
                    }
                    this.Close();
                }
            }
            catch (System.Exception)
            {
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
            }
            return 0;
        }


        public int SendSync(byte[] buf, int len)
        {
            if (m_socket == null)
                return 0;
            try
            {
                return m_multipleSocket.SendTo(buf, len, SocketFlags.None, ipep);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10051)
                {
                    Debug.LogException(e);
                    if (OnClose != null)
                    {
                        OnClose.Invoke();
                    }
                    this.Close();
                }
            }
            catch (System.Exception)
            {
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
            }
            return 0;
        }

        /// <summary>
        /// 异步方式发送
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public int SendAsync(byte[] buf, int len, string ip, ushort port)
        {
            if (m_socket == null)
                return 0;
            try
            {
                m_socket.BeginSend(buf, len, ip, port, new AsyncCallback(OnSend), m_socket);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10051)
                {
                    Debug.LogException(e);
                    if (OnClose != null)
                    {
                        OnClose.Invoke();
                    }
                    this.Close();
                }
            }
            catch (System.Exception)
            {
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
                return 0;
            }
            return len;
        }
        private void OnSend(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.Close();
            UdpClient socket = (UdpClient)ar.AsyncState;
            if (socket == null)
            {
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
                return;
            }
            try
            {
                socket.EndSend(ar);
            }
            catch(SocketException e)
            {
                if (e.ErrorCode != 10051)
                {
                    Debug.LogException(e);
                    if (OnClose != null)
                    {
                        OnClose.Invoke();
                    }
                    this.Close();
                }
            }
            catch(Exception)
            {
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
            }
        }
    }
}
