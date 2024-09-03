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
    public interface IFSMState<Key>
    {
        void Enter(); //状态委托：开始
        void Update();//状态委托：刷新
        void Exit();  //状态委托：退出
    }

    public interface IFSMInput<Key>
    {
        void Input(params object[] vs);
        void Connect(Key key, IFSMState<Key> state);
        void Disconnect(IFSMState<Key> state);
        void Release();
        bool TryGetValue(IFSMConditions<Key> condition, out IFSMInput<Key> resulf);
    }


    public class FSMInput<Key> : IFSMInput<Key>
    {
        internal Key findKey;
        internal Dictionary<Key, IFSMInput<Key>> concats = new Dictionary<Key, IFSMInput<Key>>();

        public virtual void Connect(Key key, IFSMState<Key> state)
        {
            concats[key] = state as IFSMInput<Key>;
        }

        public virtual void Disconnect(IFSMState<Key> state)
        {
            foreach (var item in concats)
            {
                if (item.Value == state)
                {
                    concats.Remove(item.Key);
                    break;
                }
            }
        }

        public virtual void Input(params object[] vs) { }

        public virtual void Release()
        {
            findKey = FSMMonitor<Key>.DefaultKey;
            concats.Clear();
        }

        public virtual bool TryGetValue(IFSMConditions<Key> condition, out IFSMInput<Key> resulf)
        {
            if (condition.Condition(out findKey))
            {
                concats.TryGetValue(findKey, out resulf);
                return true;
            }
            else
            {
                resulf = FSMMonitor<Key>.DefaultInput;
                return false;
            }
        }
    }

    public abstract class FSMTransfer<Key> : FSMInput<Key>, IFSMState<Key>
    {
        public virtual void Enter() { }

        public virtual void Update() { }

        public virtual void Exit() { }
    }
}
