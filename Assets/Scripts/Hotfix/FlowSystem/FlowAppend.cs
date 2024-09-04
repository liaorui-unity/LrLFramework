using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Flow
{
    public interface IFlowAppend
    {
        void Prefix();
        void Postfix();
    }

    public class FlowAppend : ListFlow<IFlowAppend>
    {
        public static void GetAppends()
        {
            GetLayerType();
        }

        public static void Prefix()
        {
            Debug.Log("处理流程逻辑前事件");
            foreach (var item in finds)
            {
                item.Prefix();
            }
        }

        public static void Postfix()
        {
            Debug.Log("处理流程逻辑后事件");
            foreach (var item in finds)
            {
                item.Postfix();
            }
        }
    }
}