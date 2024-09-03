using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Sailfish
{
    public class JsonValueSingle : JsonGUIValue
    {
        string tempStr;
        string valueStr;


        public JsonValueSingle(Type type, object value, string name) : base(type, value, name)
        {
            tempStr = valueStr =$"{value}";
        }


        public override void CustomGUI()
        {
            if (type == typeof(float))
            {
                Field<float>();
            }
            else if (type == typeof(int))
            {
                Field<int>();
            }
            else
            {
                NoTypeField(type, name);
            }
        }


        void Field<T>()
        {
            GUILayout.BeginHorizontal();
            TitleLayout();

            tempStr = GUILayout.TextField(tempStr);
            GUILayout.EndHorizontal();

            if (valueStr != tempStr)
            {
                var match = Regex.Match(tempStr, JsonMono.pattern);
                if (match.Success)
                {
                    valueStr = match.Value;
                    this.value = (T)Convert.ChangeType(valueStr, typeof(T));
                }
                else
                {
                    if (string.IsNullOrEmpty(tempStr))
                    { 
                        this.value = (T)Convert.ChangeType(0, typeof(T));
                    }
                    else
                    {
                        tempStr = valueStr;
                    }
                }
            }
        }
    }
}
