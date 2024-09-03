using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sailfish
{
    public class JsonGUILayout
    {
        static Texture2D offTex;
        static Texture2D onTex;

        static Texture2D onBack;

        static Texture2D exit;
        static Texture2D save;

        static Texture2D exitHover;
        static Texture2D saveHover;


        static Texture2D OffTex => offTex = offTex ?? Resources.Load<Texture2D>("JsonIcon/Off");
        static Texture2D OnTex  => onTex = onTex   ?? Resources.Load<Texture2D>("JsonIcon/On");

        public static Texture2D OnBack => onBack = onBack ?? Resources.Load<Texture2D>("JsonIcon/back");

        public static Texture2D Exit   => exit = exit     ?? Resources.Load<Texture2D>("JsonIcon/close");

        public static Texture2D Save   => save = save     ?? Resources.Load<Texture2D>("JsonIcon/Save");

        public static Texture2D ExitHover => exitHover = exitHover ?? Resources.Load<Texture2D>("JsonIcon/closeHover");

        public static Texture2D SaveHover => saveHover = saveHover ?? Resources.Load<Texture2D>("JsonIcon/SaveHover");

        public static Texture2D texture => new Texture2D(100, 100, TextureFormat.RGBA32, true);

        public static GUIStyle jsonStyle ;
        public static GUIStyle fieldStyle ;
        public static GUIStyle jsonButton ;
        public static GUIStyle textFieldStyle;

        public static void Init()
        {

            jsonStyle = new GUIStyle();
            jsonStyle.fontSize          = 24;
            jsonStyle.alignment         = TextAnchor.UpperCenter;
            jsonStyle.fontStyle         = FontStyle.Bold;
            jsonStyle.normal.background = JsonGUILayout.OnBack;

            jsonButton = new GUIStyle();
            jsonButton.fontSize          = 20;
            jsonButton.alignment         = TextAnchor.MiddleCenter;
            jsonButton.normal.background = texture;//Texture2D.grayTexture;
            jsonButton.normal.textColor  = Color.blue;
            jsonButton.hover.background  = Texture2D.whiteTexture;
            jsonButton.hover.textColor   = Color.red;

            fieldStyle = new GUIStyle();
            fieldStyle.fontSize  = 20;
            fieldStyle.alignment = TextAnchor.MiddleLeft;
            fieldStyle.fontStyle = FontStyle.Bold;

        }



        public static bool Fold(bool value,string name,bool isCanSelect=false)
        {
            var IsValue = value;
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(value ? OffTex: OnTex, GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
            {
                IsValue = !IsValue;
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (isCanSelect)
                {
                    GUILayout.Button(name,fieldStyle);
                }
                else
                {
                    GUILayout.Label(name,fieldStyle);
                }
            }

            GUILayout.EndHorizontal();

            return IsValue;
        }


       


        public static bool Fold(ref bool value, string name)
        {
       
            var resulf = false;
            GUILayout.BeginHorizontal();
       
            if (GUILayout.Button(value ? OffTex : OnTex, GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
            {
                value = !value;
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (GUILayout.Button(name, fieldStyle))
                {
                    resulf = true;
                }
            }

            GUILayout.EndHorizontal();

            return resulf;
        }

    }
}