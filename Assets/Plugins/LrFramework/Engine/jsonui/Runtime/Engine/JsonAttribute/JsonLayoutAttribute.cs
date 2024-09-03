using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sailfish
{

    /// <summary>
    /// 忽视布局特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Field)]
    public class JsonNotLayoutAttribute : Attribute
    {

    }


    /// <summary>
    /// 布局字段特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class)]
    public class JsonFieldAttribute : Attribute
    {

    }



    /// <summary>
    /// 布局类型特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Field)]
    public class JsonLayoutAttribute : Attribute
    {
        [Header ("布局类型")]
        public Type layoutType;
    }
}