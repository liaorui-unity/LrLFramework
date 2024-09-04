using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Flow
{
    public class ListFlow<T> where T : class
    {
        protected static List<T> finds = new List<T>();
        public delegate T Callback();

        /// <summary>
        /// 不在同一个程序集中的类，需要手动注册
        /// </summary>
        public static Callback OnRegisterCallback;

        public static void Register(T flow)
        {
            if (finds.Contains(flow) == false)
            {
                finds.Add(flow);
            }
        }

        public static List<T> GetLayerType()
        {
            if (OnRegisterCallback != null)
            {
                finds.Add(OnRegisterCallback());
            }

            //当前程序集中查找所有继承FlowTask的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                       .Where(t =>
                        {
                            return t.GetInterfaces().Contains(typeof(T));
                        })
                       .ToList();

            foreach (var type in types)
            {
                finds.Add(Activator.CreateInstance(type) as T);
            }

            return finds;
        }
    }
}
