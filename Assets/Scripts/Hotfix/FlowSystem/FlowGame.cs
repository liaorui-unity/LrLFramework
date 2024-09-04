using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Flow
{
    public class FlowGame : ListFlow<IFlowTask>
    {
        static StepMachine stepMachine = new StepMachine();
        static List<IFlowTask> tasks => stepMachine.tasks;
        public static int Count => tasks.Count;
        public static float Progress => (float)stepMachine.current / Count;

        public static void GetLayerFlow(int layer = 0)
        {
            if (OnRegisterCallback != null)
            {
                OnRegisterCallback();
            }

            tasks.AddRange(GetLayerType());
            tasks.RemoveAll(_ => _.layer != layer);
            tasks.Sort((a, b) => a.order.CompareTo(b.order));

            Debug.Log("需要处理的流程：" + tasks.Count);
        }

        public static void Clear()
        {
            stepMachine.Clear();
            stepMachine = null;
            OnRegisterCallback = null;
        }


        /// <summary>
        /// 同步调用方法
        /// </summary>
        /// <param name="call"></param>
        public static void Start(UnityAction call = null)
        {
            stepMachine.Start(call);
        }

        /// <summary>
        /// 异步调用方法
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public static async Task StartSync(UnityAction call = null)
        {
            await stepMachine.StartSync(call);
        }

        /// <summary>
        /// 携程调用方法
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns> <summary>
        public static IEnumerator IEStart(UnityAction call = null)
        {
            yield return stepMachine.IEStart(call);
        }
    }
}