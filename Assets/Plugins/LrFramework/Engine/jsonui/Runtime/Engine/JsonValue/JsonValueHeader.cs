using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Sailfish
{

    /// <summary>
    /// 进度条布局
    /// </summary>
    public class JsonLayoutHeaderAttribute : JsonLayoutAttribute
    {
        internal string content;

        public JsonLayoutHeaderAttribute(string content)
        {
            this.content = content;
            layoutType = typeof(JsonValueHeader);
        }
    }



    public class JsonValueHeader : JsonGUIValue
    {

        GUIStyle style = new GUIStyle();

        internal string content;
        public JsonValueHeader(Type type, object value, string name) : base(type, value, name)
        {
            style.fontSize         = 16;
            style.fontStyle        = FontStyle.Bold;
            style.normal.textColor = Color.red;
        }

        public override void JsonLayoutData(JsonLayoutAttribute layoutData)
        {
            var header = (JsonLayoutHeaderAttribute)layoutData;
            content = header.content;
        }


        public override void CustomGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"[{content}]", style,GUILayout.MaxHeight(16));
            GUILayout.EndHorizontal();
        }
    }
}
