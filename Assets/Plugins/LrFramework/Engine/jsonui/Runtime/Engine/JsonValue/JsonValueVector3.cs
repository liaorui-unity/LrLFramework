using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Sailfish
{
    public class JsonValueVector3 : JsonGUIValue
    {
        string tempXStr;
        string tempYStr;
        string tempZStr;
        string xStr;
        string yStr;
        string zStr;


        bool isChange = false;

        public JsonValueVector3(Type type, object value, string name) : base(type, value, name)
        {
            var vec3 = (Vector3)value;
            xStr = vec3.x.ToString();
            yStr = vec3.y.ToString();
            zStr = vec3.z.ToString();
        }


        public override void CustomGUI()
        {
            GUILayout.BeginHorizontal();

            TitleLayout();

            var v3 = (Vector3)value;

            tempXStr = GUILayout.TextField(xStr);
            tempYStr = GUILayout.TextField(yStr);
            tempZStr = GUILayout.TextField(zStr);

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

            if (zStr != tempZStr)
            {
                var match = Regex.Match(tempZStr, JsonMono.pattern);
                if (match.Success)
                {
                    isChange = true;
                    zStr = match.Value;
                }
                else
                    tempZStr = zStr;
            }

            GUILayout.EndHorizontal();


            if (isChange)
            {
                isChange = false;

                if (!float.TryParse(xStr, out float vX))
                {
                    vX = v3.x;
                }

                if (!float.TryParse(yStr, out float vY))
                {
                    vY = v3.y;
                }

                if (!float.TryParse(zStr, out float vZ))
                {
                    vZ = v3.z;
                }

                value = new Vector3(vX, vY, vZ);
            }
        }
    }
}
