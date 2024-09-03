using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using JsonInfo = Sailfish.JsonGUIContent.JsonInfo;

namespace Sailfish
{
    public class JsonGUIField
    {

        public Type type;
        public string name;
        public object value;

        public string describe;

        public JsonGUIField(Type type, object value, string name)
        {
            this.type = type;
            this.name = name;
            this.value = value;
        }

        public virtual void TitleLayout() { }
        public virtual void ValueLayout() { }
        public virtual void CustomGUI()
        {
            TitleLayout();
            ValueLayout();
        }

        public virtual void Disable()
        {
             
        }

        public virtual void JsonLayoutData(JsonLayoutAttribute layoutData) { }

        public static JsonGUIField Create(Type type, object value, string name)
        {
            JsonGUIField infoGUI = null;

            foreach (var item in JsonRegiter.importers)
            {
                UnityEngine.Debug.Log(item);
                if (item.handler(type))
                {

                    infoGUI = (JsonGUIField)Activator.CreateInstance(item.exporter, type, value, name);
                    break;
                }
            }

            return infoGUI;
        }




        public static JsonGUIField Create(JsonLayoutAttribute layoutAttribute, Type type, object value, string name,string describe="")
        {
            JsonGUIField infoGUI = null;

            if (layoutAttribute != null)
            {
                infoGUI = (JsonGUIField)Activator.CreateInstance(layoutAttribute.layoutType, type, value, name);
                infoGUI.JsonLayoutData(layoutAttribute);
            }
            else
            {
                foreach (var item in JsonRegiter.importers)
                {
                    if (item.handler(type))
                    {
                        infoGUI = (JsonGUIField)Activator.CreateInstance(item.exporter, type, value, name);
                        break;
                    }
                }
            }

            if (infoGUI == null)
            {
                infoGUI = new JsonGUIValue(type, value, name);
            }

            infoGUI.describe = describe;
            return infoGUI;
        }


        public static List<JsonInfo> CheckFieldLayout(object fieldValue, FieldInfo[] fieldInfos)
        {
            List<JsonInfo> jsonInfos = new List<JsonInfo>();

            foreach (var info in fieldInfos)
            {
                var type = info.FieldType;
                var value = info.GetValue(fieldValue);
                var layouts = info.GetCustomAttributes<PropertyAttribute>().Where(_ => Unity2Json.jsonAttributes.Contains(_.GetType()));

                JsonGUIField jsInfo = null;
                bool isOverride = false;


                foreach (var layout in layouts)
                {
                    if (layout is RangeAttribute)
                    {
                        var range = layout as RangeAttribute;
                        var slider = new JsonLayoutSliderAttribute(range.min, range.max);
                        jsInfo = JsonGUIField.Create(slider, type, value, info.Name);
                        isOverride = true;
                    }
                    else if (layout is HeaderAttribute)
                    {
                        var header = layout as HeaderAttribute;
                        var describe = new JsonLayoutHeaderAttribute(header.header);
                        jsInfo = JsonGUIField.Create(describe, type, value, info.Name);
                    }

                    jsonInfos.Add(new JsonInfo() { info = info, jsonInfo = jsInfo });
                }

                if (isOverride == false)
                {
                    jsInfo = JsonGUIField.Create(null, type, value, info.Name);
                    jsonInfos.Add(new JsonInfo() { info = info, jsonInfo = jsInfo });
                }
            }

            return jsonInfos;
        }

    }
}
