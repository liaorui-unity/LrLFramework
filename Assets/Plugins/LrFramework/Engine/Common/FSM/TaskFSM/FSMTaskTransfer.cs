using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
    public interface IFSMTransfer
    {
        bool isComplete { get; }

        void Enter(); //×´Ì¬Î¯ÍÐ£º¿ªÊ¼
        void Update();//×´Ì¬Î¯ÍÐ£ºË¢ÐÂ
        void Exit();  //×´Ì¬Î¯ÍÐ£ºÍË³ö
        void Input(IFSMConditions conditions);
    }



    public abstract class FSMTaskTransfer :IFSMTransfer
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