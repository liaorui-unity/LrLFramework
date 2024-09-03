using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Sailfish
{
    public class JsonValueVector2 : JsonGUIValue
    {

        string tempXStr;
        string tempYStr;
        string xStr;
        string yStr;


        bool isChange = false;

        public JsonValueVector2(Type type, object value, string name) : base(type, value, name)
        {
            var vec2 = (Vector2)value;
            xStr = vec2.x.ToString();
            yStr = vec2.y.ToString();
        }


        public override void CustomGUI()
        {
            GUILayout.BeginHorizontal();
            TitleLayout();

            var v2 = (Vector2)value;

            tempXStr = GUILayout.TextField(xStr);
            tempYStr = GUILayout.TextField(yStr);

            if (xStr != tempXStr)
            {
                var match = Regex.Match(tempXStr, JsonMono.pattern);
                if (match.Success)
                {
                    isChange = true;
                    xStr = match.Value;
                }
                else
                    tempXStr = xStr;
            }

            if (yStr != tempYStr)
            {
                var match = Regex.Match(tempYStr, JsonMono.pattern);
                if (match.Success)
                {
                    isChange = true;
                    yStr = match.Value;
                }
                else
                    tempYStr = yStr;
            }

            GUILayout.EndHorizontal();


            if (isChange)
            {
                isChange = false;
                if (!float.TryParse(xStr, out float vX))
                {
                    vX = v2.x;
                }

                if (!float.TryParse(yStr, out float vY))
                {
                    vY = v2.y;
                }

                value = new Vector2(vX, vY);
            }
        }
    }
}
