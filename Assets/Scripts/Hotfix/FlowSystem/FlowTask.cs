using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniRx;
using UnityEngine.Events;

namespace Flow
{

    public interface IFlowTask
    {
        //排序序号order = 10为加载场景
        //需要加载场景后的逻辑应在10以后

        /// <summary>
        /// layer层级
        /// </summary>
        /// <value></value>
        int layer { get; }


        /// </summary> 
        /// 顺序排序
        /// </summary>
        /// <value></value>
        int order { get; }

        Task Logic();
    }


    public class StepMachine
    {
        public List<IFlowTask> tasks = new List<IFlowTask>();
        public int current = 0;


        public void Register(IFlowTask flow)
        {
            if (tasks.Contains(flow) == false)
            {
                tasks.Remove(flow);
            }
        }

        public void Clear()
        {
            tasks.Clear();
        }

        /// <summary>
        /// 同步调用方法
        /// </summary>
        /// <param name="call"></param>
        public void Start(UnityAction everyCall = null)
        {
            //复制一份，防止在执行过程中添加新的任务
            var tasks = new List<IFlowTask>(this.tasks);
            current = 0;
            foreach (var task in tasks)
            {
                task.Logic();
                current++;
                everyCall?.Invoke();
            }
            tasks.Clear();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public async Task StartSync(UnityAction everyCall = null)
        {
            //复制一份，防止在执行过程中添加新的任务
            var tasks = new List<IFlowTask>(this.tasks);
            current = 0;
            foreach (var task in tasks)
            {
                await task.Logic();
                current++;
                everyCall?.Invoke();
            }
            tasks.Clear();
        }

        /// <summary>
        /// 携程调用方法
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns> <summary>
        public IEnumerator IEStart(UnityAction everyCall = null)
        {
            //复制一份，防止在执行过程中添加新的任务
            var tasks = new List<IFlowTask>(this.tasks);
            current = 0;
            foreach (var task in tasks)
            {
                yield return task.Logic();
                current++;
                everyCall?.Invoke();
            }
            tasks.Clear();
        }
    }
}