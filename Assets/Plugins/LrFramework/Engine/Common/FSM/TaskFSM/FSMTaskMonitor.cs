using Sailfish;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// T类型为判断状态条件 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSMTaskMonitor 
    {
        List<IFSMTransfer>  concats    = new List<IFSMTransfer>();
        Queue<IFSMTransfer> taskQueues = new Queue<IFSMTransfer>();

        IFSMTransfer currentState;

        FSMTaskConditions defaultConditions;

        bool isComplete = false;

        public FSMTaskMonitor()
        {
            defaultConditions = new FSMTaskConditions();
            CallUnit.updateCall.AddListener(Update);
        }


        public void Release()
        {
            concats   .Clear();
            taskQueues.Clear();
            currentState = null;
            CallUnit.updateCall.RemoveListener(Update);
        }

        public void Add(IFSMTransfer transfer)
        {
            if (concats.Contains(transfer) == false)
            {
                concats   .Add(transfer);
                taskQueues.Enqueue(transfer);
            }
        }


        public void Skip()
        {
            defaultConditions.Skip();
        }


        public void Goto(IFSMTransfer gotoState)
        {
            if (concats.Contains(gotoState))
            {
                var num = concats.FindIndex((_) => _ == gotoState);

                //重新刷新队列
                taskQueues.Clear();
                for (int i = num + 1; i < concats.Count; i++)
                {
                    taskQueues.Enqueue(concats[i]);
                }

                SwitchState(gotoState);
            }
        }


        public void Input(IFSMConditions conditions)
        {
            currentState?.Input(conditions);
        }


        public void Update()
        {
            if (isComplete == false)
            {
                if (currentState == null || (bool)currentState?.isComplete || defaultConditions.Condition())
                {
                    if (taskQueues.Count <= 0)
                    {
                        isComplete = true;
                        return;
                    }
                    SwitchState(taskQueues.Dequeue());
                }
                currentState?.Update();
            }
        }

        void SwitchState(IFSMTransfer transfer)
        {
            if (concats.Contains(transfer))
            {
                currentState?.Exit();
                currentState = transfer;
                currentState?.Enter();
            }
        }
    }
}