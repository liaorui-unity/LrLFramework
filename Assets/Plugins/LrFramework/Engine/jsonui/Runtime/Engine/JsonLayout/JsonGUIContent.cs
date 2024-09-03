using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;


namespace Sailfish
{
    public class JsonGUIContent : MonoBehaviour
    {
        static JsonGUIContent instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitJsonGUI()
        {
            if (instance == null)
            {
                JsonRegiter.InitRegiter();
                instance = new GameObject("JsonGUI").AddComponent<JsonGUIContent>();
                jsonGUI = new JsonGUIShow();
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        public static float defaultWidth = 400;
        public static float defaultHeigth = Screen.height;

        static JsonGUIShow jsonGUI;

        static Dictionary<Type, JsonMono> typeDatas = new Dictionary<Type, JsonMono>();



        public static void AddJsonMono(JsonMono json)
        {
            if (typeDatas.ContainsKey(json.GetType()) == false)
                typeDatas.Add(json.GetType(), json);
        }

        public static void RemoveJsonMono(JsonMono json)
        {
            if (typeDatas.ContainsKey(json.GetType()))
                typeDatas.Remove(json.GetType());
        }


        void Awake()
        {
            JsonGUILayout.Init();
        }




        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
            {
                jsonGUI.IsShow = !jsonGUI.IsShow;
            }
        }
        private void OnGUI()
        {
            if (jsonGUI.IsShow)
            {
                GUI.skin.textField.normal.textColor = Color.yellow;
                GUI.skin.textField.fontStyle        = FontStyle.Bold;
                jsonGUI.CustomGUI();
            }
        }


        public class JsonInfo
        {
            public FieldInfo info;
            public JsonGUIField jsonInfo;
        }

        public class JsonGUIShow
        {
            public static Action otherGUI;


            public bool IsShow = false;

            JsonMono currentJson;
            FieldInfo[] currentInfos;

            List<JsonInfo> currentJsonInfos = new List<JsonInfo>();

            Vector2 scrollV2;
            Rect save = new Rect(defaultWidth / 4 / 2, defaultHeigth - 40 - 10, defaultWidth / 4, 40);
            Rect exit = new Rect(defaultWidth / 2 + defaultWidth / 4 / 2, defaultHeigth - 40 - 10, defaultWidth / 4, 40);

            Rect window = new Rect(0, 0, defaultWidth, defaultHeigth);
            Rect fieldWindow = new Rect(defaultWidth, 0, defaultWidth, defaultHeigth);


            GUIStyle jsonStyle  => JsonGUILayout.jsonStyle;
            GUIStyle jsonButton => JsonGUILayout.jsonButton;


            GUIStyle saveStyle;
            GUIStyle exitStyle;

            public JsonGUIShow()
            {
                saveStyle = new GUIStyle();
                saveStyle.normal.background = JsonGUILayout.Save;
                saveStyle.hover. background = JsonGUILayout.SaveHover;

                exitStyle = new GUIStyle();
                exitStyle.normal.background = JsonGUILayout.Exit;
                exitStyle.hover .background = JsonGUILayout.ExitHover;
            }


            public void CustomGUI()
            {
                if (typeDatas.Count > 0)
                    GUI.Window(0, window, DrawGUI, "Json配置表",jsonStyle);

                if (currentJson)
                    GUI.Window(1, fieldWindow, ShowField, currentJson.ToString(),jsonStyle);

                otherGUI?.Invoke();
            }

            void DrawGUI(int id)
            {
                ShowButton(id);


                foreach (var item in typeDatas)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(item.Key.ToString(), jsonButton, GUILayout.Width(defaultWidth - 40), GUILayout.Height(30)))
                    {

                        //清楚现有JsonInfo
                        foreach (var json in currentJsonInfos)
                        {
                            json.jsonInfo.Disable();
                        }
                        currentJsonInfos.Clear();

                        currentJson?.SaveJsonFile();
                        currentJson      = item.Value;
                        currentInfos     = currentJson.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                        currentJsonInfos = JsonGUIField.CheckFieldLayout(currentJson, currentInfos);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

           

            void ShowButton(int id)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("", saveStyle, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    currentJson?.SaveJsonFile();
                }

                GUILayout.Space(5);

                if (GUILayout.Button("", exitStyle, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    IsShow = false;
                    //清楚现有JsonInfo
                    foreach (var json in currentJsonInfos)
                    {
                        json.jsonInfo.Disable();
                    }
                    currentJsonInfos.Clear();
                    currentJson?.SaveJsonFile();
                    currentJson = null;
                }

                GUILayout.Space(20);
                GUILayout.EndHorizontal();
            }

            void ShowField(int id)
            {
                GUILayout.Space(30);

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                scrollV2 = GUILayout.BeginScrollView(scrollV2);
                foreach (var item in currentJsonInfos)
                {
                    GUILayout.Space(6);
                    item.jsonInfo.value = item.info?.GetValue(currentJson);
                    item.jsonInfo.CustomGUI();
                    item.info?.SetValue(currentJson, item.jsonInfo.value);
                }
                GUILayout.EndScrollView();
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
            }
        }
    }
}
