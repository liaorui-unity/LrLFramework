using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
    public interface IFSMTransfer
    {
        bool isComplete { get; }

        void Enter(); //状态委托：开始
        void Update();//状态委托：刷新
        void Exit();  //状态委托：退出
        void Input(IFSMConditions conditions);
    }



    public abstract class FSMTaskTransfer : IFSMTransfer
    {
        public bool isComplete { get; protected set; } = false;


        public virtual void Enter()
        {
            isComplete = false;
        }

        public virtual void Exit()
        {
           
        }

        public virtual void Update()
        {

        }

        public virtual void Input(IFSMConditions conditions) 
        {
            if (conditions.Condition())
            {
                isComplete = true;
            }
        }
    }
}