using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Sailfish
{
    /// <summary>
    /// 自定义特殊类实例化
    /// </summary>
    public class JsonImporter
    {
        public Type exporter;
        public Func<Type, bool> handler;
        public JsonImporter(Type type) { exporter = type; }


        public override int GetHashCode()
        {
            return exporter.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == exporter.GetHashCode();
        }
    }


    public class Unity2Json
    {
        public static List<System.Type> jsonAttributes = new List<System.Type>() 
        {
            typeof(HeaderAttribute),
            typeof(RangeAttribute),
        };

    }


    /// <summary>
    /// 注册自定义特殊类
    /// </summary>
    public class JsonRegiter
    {
        internal static void InitRegiter()
        {
            Regiter(new JsonImporter(typeof(JsonValueVector2))
            {
                handler = (_) => _ == typeof(Vector2)
            });

            Regiter(new JsonImporter(typeof(JsonValueVector3))
            {
                handler = (_) => _ == typeof(Vector3)
            });

            Regiter(new JsonImporter(typeof(JsonGUIEnum))
            {
                handler = (_) => _.BaseType == typeof(System.Enum)
            });

            Regiter(new JsonImporter(typeof(JsonValueSingle))
            {
                handler = (_) => _.GetInterface(nameof(IFormattable)) != null
            });

            Regiter(new JsonImporter(typeof(JsonGUIClass))
            {
                handler = (_) => _.GetCustomAttribute<JsonFieldAttribute>() != null
            });

            Regiter(new JsonImporter(typeof(JsonGUIArray))
            {
                handler = (_) => _.IsArray
            });

            Regiter(new JsonImporter(typeof(JsonGUIList))
            {
                handler = (_) => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(List<>)
            });
        }

        internal static List<JsonImporter> importers = new List<JsonImporter>();


        public static void Regiter(JsonImporter import)
        {
            if (!importers.Contains(import))
            {
                importers.Add(import);
            }
        }
    }
}
