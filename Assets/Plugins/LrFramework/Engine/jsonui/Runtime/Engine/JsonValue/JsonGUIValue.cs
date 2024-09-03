using System;
using UnityEngine;


namespace Sailfish
{
    public class JsonGUIValue : JsonGUIField
    {
        public JsonGUIValue(Type type, object value, string name) : base(type, value, name) { }

        public override void CustomGUI()
        {
            if (type == typeof(string))
            {
                Field($"{value}", name);
            }
            else if (type == typeof(bool))
            {
                FieldBool((bool)value, name);
            }
            else
            {

                NoTypeField(type, name);
            }
        }


        void Field(string value, string name)
        {
            GUILayout.BeginHorizontal();
            TitleLayout();
            this.value = GUILayout.TextField(value);
            GUILayout.EndHorizontal();
        }

        void FieldBool(bool value, string name)
        {
            GUILayout.BeginHorizontal();
            TitleLayout();
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }


        public void NoTypeField(Type type, string name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            GUILayout.Label(type.ToString());
            GUILayout.EndHorizontal();
        }

        public override void TitleLayout()
        {
            if (!string.IsNullOrEmpty(name))
            {
                GUILayout.Label(name, JsonGUILayout.fieldStyle,GUILayout.MaxWidth(120));
            }
        }
    }
}