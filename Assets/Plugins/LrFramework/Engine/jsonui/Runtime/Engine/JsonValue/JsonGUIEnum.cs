using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace Sailfish
{
    public class JsonGUIEnum : JsonGUIField
    {
        int nameId = 0;

        Rect enumWindow;
        Rect lastWindow;

        string[] names;
        bool isShow = false;

        Vector2 radio;


        public JsonGUIEnum(Type type, object value, string name) : base(type, value, name)
        {
            isShow = false;
            names  = type.GetEnumNames();
            nameId = names.ToList().FindIndex(_ => _ == value.ToString());
        }

        public override void CustomGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name,JsonGUILayout.fieldStyle);
 
            if (GUILayout.Button(names[nameId], GUILayout.Width(240)))
            {
                isShow = !isShow;
            }
            GUILayout.EndHorizontal();

            if (isShow)
            {
                if (lastWindow.position.y <= 0)
                {
                    lastWindow = GUILayoutUtility.GetLastRect();

                    if (lastWindow.position.y > 0)
                    {
                        Enable();
                    }
                }
            }
            else
            {
                Disable();
            }

            value = Enum.Parse(type, names[nameId]);
        }

        public void Enable()
        {
            var width  = 280;
            var min    = (names.Length + 1) * 25;
            var pointY = lastWindow.position.y + 55;
            var pointX = lastWindow.position.x + 2 * JsonGUIContent.defaultWidth - width;
            var height = min > JsonGUIContent.defaultHeigth ? JsonGUIContent.defaultHeigth : min;

            enumWindow = new Rect(new Vector2(pointX, pointY), new Vector2(width, height + 10));
            JsonGUIContent.JsonGUIShow.otherGUI = OtherMenu;
        }

        public override void Disable()
        {
            isShow = false;
            lastWindow = new Rect();
            JsonGUIContent.JsonGUIShow.otherGUI = null;
        }


        void OtherMenu() 
        {
            GUI.Window(2, enumWindow, DrawWindow,string.Empty);
            GUI.BringWindowToFront(2);
        }


 

        void DrawWindow(int  id)
        { 
            radio = GUILayout.BeginScrollView(radio);

            for (int i = 0; i < names.Length; i++)
            {
                var show = names[i];
                if (i == nameId) 
                {
                    show += "      ¡Ì";
                }
     
                if (GUILayout.Button(show))
                {
                    isShow = false;
                    nameId = names.ToList().FindIndex(_ => _ == names[i]);
                    JsonGUIContent.JsonGUIShow.otherGUI = null; 
                    lastWindow = new Rect();
                }
            }

            GUILayout.EndScrollView();
        }

    }
}
