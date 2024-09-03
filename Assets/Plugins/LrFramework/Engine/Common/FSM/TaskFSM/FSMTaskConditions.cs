using Sailfish;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{

    public interface IFSMConditions
    {
        void Skip();
        bool Condition();
    }

    public class FSMTaskConditions: IFSMConditions
    {
        bool judge;

        public virtual  bool Condition()
        {
            if (judge)
            {
                judge = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void Skip()
        {
            judge = true;
        }
    }
}