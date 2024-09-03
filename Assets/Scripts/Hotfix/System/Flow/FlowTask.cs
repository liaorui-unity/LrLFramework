using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniRx;
using System.Reflection;
using UnityEngine.Events;

public interface IFlowTask
{
    //排序序号order = 10为加载场景
    //需要加载场景后的逻辑应在10以后
    int  layer { get; }
    int  order { get; }
    Task Logic();
}


public class FlowGame
{
    public List<IFlowTask> tasks = new List<IFlowTask>();

    public void FindFlow(int layer = 0)
    {
        //当前程序集中查找所有继承FlowTask的类
        var types = Assembly.GetExecutingAssembly().GetTypes()
                   .Where(t =>
                   {
                       return t.GetInterfaces().Contains(typeof(IFlowTask));
                   })
                   .ToList();

     

        foreach (var type in types)
        {
            tasks.Add(Activator.CreateInstance(type) as IFlowTask);
        }

        tasks.RemoveAll(_ => _.layer != layer);
        tasks.Sort((a, b) => a.order.CompareTo(b.order));

        Debug.Log("需要处理的流程：" + tasks.Count);
    }

    public void Register(IFlowTask task)
    {
        if (tasks.Contains(task) == false)
        {
            tasks.Add(task);
        }
    }

    public async Task Start(UnityAction call = null)
    {
        foreach (var task in tasks)
        {
            await task.Logic();
            call?.Invoke();
        }
        tasks.Clear();
    }
}