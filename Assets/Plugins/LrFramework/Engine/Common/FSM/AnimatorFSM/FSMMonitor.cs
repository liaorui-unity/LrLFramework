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

namespace Sailfish
{
	/// <summary>
	/// T类型为判断状态条件 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FSMMonitor<Key>
	{
		public readonly static Key DefaultKey = default(Key);
		public readonly static IFSMState<Key> DefaultState = default(IFSMState<Key>);
		public readonly static IFSMInput<Key> DefaultInput = default(IFSMInput<Key>);

		Dictionary<Key, IFSMInput<Key>> concats = new Dictionary<Key, IFSMInput<Key>>();

		IFSMInput<Key> tempState;
		IFSMInput<Key> currentState;

		IFSMConditions<Key> condition;

		public FSMMonitor()
		{
			condition = new FSMConditions<Key>();
			CallUnit.updateCall.AddListener(Check);
		}			

		public FSMMonitor(IFSMConditions<Key> conditions)
		{
			condition = conditions;
			CallUnit.updateCall.AddListener(Check);
		}


		public void ConcatsTransfer(Key key, IFSMState<Key> itemTransfer)
		{
			if (!concats.ContainsKey(key))
			{
				concats[key] = itemTransfer as IFSMInput<Key>;
			}
		}


		public void Default(IFSMState<Key> defaulfState)
		{
			if (concats.ContainsValue(defaulfState as IFSMInput<Key>))
			{
				SwitchState(defaulfState as IFSMInput<Key>);
			}
		}



		public void ConditionsTransfer(Key key)
		{
			condition?.Transfer(key);
		}


		public void Release()
		{
            foreach (var item in concats)
            {
				item.Value.Release();
            }
			concats.Clear();

			tempState = null;
			currentState = null;
			CallUnit.updateCall.RemoveListener(Check);
		}


		public void Goto(IFSMState<Key> gotoState)
		{
            SwitchState(gotoState as IFSMInput<Key>);
        }


		public void InputMsg(params object[] vs)
		{
			currentState?.Input(vs);
		}



		void Check()
		{
			if (currentState.TryGetValue(condition, out tempState))
			{
				SwitchState(tempState);
			}

			(currentState as IFSMState<Key>)?.Update();
		}

        void SwitchState(IFSMInput<Key> tempState)
        {
			if (concats.ContainsValue(tempState))
			{
				(currentState as IFSMState<Key>)?.Exit();

				currentState = tempState;

				(currentState as IFSMState<Key>)?.Enter();
			}
        }
    }
}
