using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using JsonInfo = Sailfish.JsonGUIContent.JsonInfo;

namespace Sailfish
{
    public class JsonGUIClass : JsonGUIField
    {
        bool isShow = false;

        Dictionary<FieldInfo, JsonGUIField> fieldGUIInfos = new Dictionary<FieldInfo, JsonGUIField>();

        List<JsonGUIContent.JsonInfo> jsonInfos = new List<JsonGUIContent.JsonInfo>();

        public JsonGUIClass(Type type, object value, string name) : base(type, value, name)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            jsonInfos.Clear();
            jsonInfos = JsonGUIField.CheckFieldLayout(value, fields);
        }

    
        public override void TitleLayout()
        {
            isShow = JsonGUILayout.Fold(isShow, name);
        }

        public override void ValueLayout()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(60);

            if (isShow)
            {
                GUILayout.BeginVertical();
                foreach (var guiInfo in jsonInfos)
                {
                    guiInfo.jsonInfo.value = guiInfo.info.GetValue(value);
                    guiInfo.jsonInfo.CustomGUI();
                    guiInfo.info.SetValue(value, guiInfo.jsonInfo.value);
                }
                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
        }
    }
}