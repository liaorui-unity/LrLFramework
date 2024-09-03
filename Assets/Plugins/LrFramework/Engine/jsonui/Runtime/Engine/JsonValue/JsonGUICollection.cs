using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Sailfish
{
    public class JsonGUICollection : JsonGUIField
    {
        protected bool IsClass   = false;
        protected bool isShow    = false;

        protected List<JsonGUIElement> fieldGUIInfos = new List<JsonGUIElement>();

        protected Action<bool, int, object> call;

        protected Func<object, object> valueHandle;

        public JsonGUICollection(Type type, object value, string name) : base(type, value, name) { }

     

 

        public override void CustomGUI()
        {
            isShow = JsonGUILayout.Fold(isShow, name);
            GUILayout.BeginHorizontal();
            GUILayout.Space(60);

            if (isShow)
            {
                GUILayout.BeginVertical();

                value = valueHandle?.Invoke(value);

                for (int i = 0; i < fieldGUIInfos.Count; i++)
                {
                    var info = fieldGUIInfos[i];
                    info.CustomGUI();    
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }


        public void ControlSet(bool isAdd, int i)
        {
            call?.Invoke(isAdd, i, value);
        }


        protected void ListSetup<T>(bool isAdd, int selectId, List<T> vs)
        {
            if (isAdd)
            {
                T value;

                if (typeof(T).IsValueType)
                {
                     value = default(T);   
                }
                else
                {
                     value = Activator.CreateInstance<T>();
                }

                var id = selectId + 1;
                vs.Insert(id, value);
                Collection<T>(isAdd, value, id);
            }
            else
            {
                vs.RemoveAt(selectId);
                Collection<T>(isAdd, value, selectId);
            }
        }

        protected void Collection<T>(bool isAdd, object value, int selectId)
        {
            if (isAdd)
            {
                var field = new JsonGUIElement(this, typeof(T), value, $"element {selectId}");
                fieldGUIInfos.Insert(selectId, field);
            }
            else
            {
                fieldGUIInfos.RemoveAt(selectId);
            }

            for (int i = selectId; i < fieldGUIInfos.Count; i++)
            {
                fieldGUIInfos[i].sortId = i;
                fieldGUIInfos[i].jsonField.name = $"element {i}";
            }
        }
    }

    public class JsonGUIArray : JsonGUICollection
    {
        public JsonGUIArray(Type type, object value, string name) : base(type, value, name)
        {
            var methodSwitch = this.GetType().GetMethod(nameof(SwitchType), BindingFlags.Public | BindingFlags.Instance);
            var methodType = methodSwitch.MakeGenericMethod(type.GetElementType());
            methodType?.Invoke(this, new object[] { value });

            var handleSwitch = this.GetType().GetMethod(nameof(TypeHandle), BindingFlags.Public | BindingFlags.Instance);
            var handleType = handleSwitch.MakeGenericMethod(type.GetElementType());
            handleType?.Invoke(this, null);

            IsClass = type.GetElementType().GetCustomAttribute<JsonFieldAttribute>() != null;
        }

        public void TypeHandle<T>()
        {
            call = (bool isAdd, int selectId, object vs) =>
            {
                var tempVs = ((T[])vs).ToList();
                ListSetup(isAdd, selectId, tempVs);
                value = tempVs.ToArray();
            };

            valueHandle = (object vs) =>
            {
                var tempVs = (T[])vs;

                if (tempVs == null)
                    tempVs = new T [0];

                for (int i = 0; i < fieldGUIInfos.Count; i++)
                {
                    tempVs[i] = (T)fieldGUIInfos[i].jsonField.value;
                }
                return tempVs;
            };
        }

        public void SwitchType<T>(T[] values)
        {
            fieldGUIInfos.Clear();

            foreach (var value in values)
            {
                var field               = new JsonGUIElement(this, typeof(T), value, $"element {fieldGUIInfos.Count}");
                    field.sortId        = fieldGUIInfos.Count;
                fieldGUIInfos.Add(field);
            }
            value = values;
        }
    }

    public class JsonGUIList : JsonGUICollection
    {
        public JsonGUIList(Type type, object value, string name) : base(type, value, name)
        {
            var methodSwitch = this.GetType().GetMethod(nameof(SwitchType), BindingFlags.Public | BindingFlags.Instance);
            var methodType = methodSwitch.MakeGenericMethod(type.GetGenericArguments()[0]);
            methodType?.Invoke(this, new object[] { value });

            var handleSwitch = this.GetType().GetMethod(nameof(TypeHandle), BindingFlags.Public | BindingFlags.Instance);
            var handleType = handleSwitch.MakeGenericMethod(type.GetGenericArguments()[0]);
            handleType?.Invoke(this, null);

            IsClass =type.GetGenericArguments()[0].GetCustomAttribute<JsonFieldAttribute>() != null;
        }


        public void TypeHandle<T>()
        {
            call = (bool isAdd, int selectId, object vs) =>
            {
                ListSetup(isAdd, selectId, (List<T>)vs);
                value = vs;
            };

            valueHandle = (object vs) =>
            {
                var tempVs = (List<T>)vs;

                if (tempVs == null)
                {
                    tempVs = new List<T>();
                }

                for (int i = 0; i < fieldGUIInfos.Count; i++)
                {
                    tempVs[i] = (T)fieldGUIInfos[i].jsonField.value;
                }
                return tempVs;
            };
        }

        public void SwitchType<T>(List<T> values)
        {
            fieldGUIInfos.Clear();

            if (values != null)
            {          
                foreach (var value in values)
                {
                    var field = new JsonGUIElement(this,typeof(T), value, $"element {fieldGUIInfos.Count}");
                    field.sortId = fieldGUIInfos.Count;
                    fieldGUIInfos.Add(field);
                }
            }
        }
    }


    public class JsonGUIElement 
    {
        public int sortId;
        public bool IsClass;
        public JsonGUIField jsonField;
 
        JsonGUICollection collection;

        public JsonGUIElement(JsonGUICollection collection, Type type, object value, string name)
        {
            this.collection = collection;
            this.jsonField = JsonGUIField.Create(null, type, value, name);
            this.IsClass = type.GetCustomAttribute<JsonFieldAttribute>() != null;
        }

        public void CustomGUI()
        {
            if (IsClass)
            {
                GUILayout.BeginHorizontal();
                jsonField.TitleLayout();
                ButtonSet();
                GUILayout.EndHorizontal();
                jsonField.ValueLayout();
            }
            else
            {
                GUILayout.BeginHorizontal();
                jsonField.CustomGUI();
                ButtonSet(); 
                GUILayout.EndHorizontal();
            }
        }

        protected void ButtonSet()
        {
            if (GUILayout.Button("+"))
            {
                this.collection.ControlSet(true, sortId);
            }

            if (GUILayout.Button("-"))
            {
                this.collection.ControlSet(false, sortId);
            }
        }
    }
}