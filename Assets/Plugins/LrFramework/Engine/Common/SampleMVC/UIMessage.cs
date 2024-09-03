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

namespace Sailfish
{
	public class UIMsg
	{
		public object[] args;
		public UIMsg(params object[] args)
		{
			this.args = args;
		}
	}


	public class TypeMsg
	{
		public Dictionary<System.Type, System.Action<UIMsg>> m_Msgs;
	}


	public class UIMessage  
	{
		static Dictionary<string, TypeMsg> m_Types = new Dictionary<string, TypeMsg>();

		internal static TypeMsg FindType(string msgKey)
		{
			if (m_Types.TryGetValue(msgKey, out TypeMsg typeMsg))
			{
				return typeMsg;
			}
			else
			{
				typeMsg = new TypeMsg();
				typeMsg.m_Msgs = new Dictionary<System.Type, System.Action<UIMsg>>();
				m_Types[msgKey] = typeMsg;
				return typeMsg;
			}
		}


		internal static void TriggerMsg(string msgKey, UIMsg msg)
		{
			var msgType = FindType(msgKey);

			foreach (var item in msgType.m_Msgs)
			{
				item.Value?.Invoke(msg);
			}
		}


		internal static void AddTypeMsg<T>(string msgKey, System.Action<UIMsg> msgCall)
		{
			var typeMsg = FindType(msgKey);
			typeMsg.m_Msgs[typeof(T)] = msgCall;
		}


		internal static void RemoveTypeMsg<T>(string msgKey)
		{
			var typeMsg = FindType(msgKey);

			if (typeMsg.m_Msgs.ContainsKey(typeof(T)))
			{
				typeMsg.m_Msgs.Remove(typeof(T));
			}
		}


		internal static void RemoveAllTypeMsg<T>()
		{
            foreach (var item in m_Types.Values)
            {
				if (item.m_Msgs.ContainsKey(typeof(T)))
				{
					item.m_Msgs.Remove(typeof(T));
				}
			}
		}

		internal static void RemoveAllTypeMsg<T>(string msgKey)
		{
			if (m_Types.TryGetValue(msgKey, out TypeMsg typeMsg))
			{
				typeMsg.m_Msgs.Clear();
			}
			m_Types.Remove(msgKey);
		}

	}

	public static class UIMessageExtension
    {
		public static void AddTypeMsg<T>(this T target, string msgKey, System.Action<UIMsg> msgCall)
		{
			UIMessage.AddTypeMsg<T>(msgKey, msgCall);
		}

		public static void RemoveAllTypeMsg<T>(this T target)
		{
			UIMessage.RemoveAllTypeMsg<T>();
		}

		public static void RemoveTypeMsg<T>(this T target, string msgKey)
		{
			UIMessage.RemoveTypeMsg<T>(msgKey);
		}

		public static void RemoveAllTypeMsg<T>(this T target, string msgKey)
		{
			UIMessage.RemoveAllTypeMsg<T>(msgKey);
		}	

		public static void TriggerUIMsg<T>(this T target, string msgKey, UIMsg msg)
		{
			UIMessage.TriggerMsg(msgKey, msg);
		}

		public static void TriggerUIMsg(this UIMsg target, string msgKey)
		{
			UIMessage.TriggerMsg(msgKey, target);
		}
	}
}
