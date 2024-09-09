//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2019-12-11 09:29:13
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

namespace Table
{
    public class SetSortingLayersIdWindow : EditorWindow
    {
        public SetSortingLayersIdWindow()
        {
            this.titleContent = new GUIContent("Sorting Layers ID");
        }

        [MenuItem("GameObject/Sorting Layers ID窗口", false, -1)]
        public static void CreateLayerWindows()
        {
            EditorWindow.GetWindow(typeof(SetSortingLayersIdWindow));
        }


        List<string> dLayers = new List<string>();
        string[] defaultLayers;
        private void OnEnable()
        {
            for (int i = 0; i < SortingLayer.layers.Length; i++)
            {
                dLayers.Add(SortingLayer.layers[i].name);
            }
            defaultLayers = dLayers.ToArray();

            FindAllTransfrom(Selection.activeGameObject.transform);
        }

        Vector2 scrollPos = Vector2.zero;

        int changeID = 0;
        string[] changeLayers;
        bool foldout = false;

        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Space(20);
            if (changeLayers == null)
                changeLayers = new string[layerIDs.Count];
            for (int i = 0; i < layerIDs.Count; i++)
            {
                changeLayers[i] = layerDatas[layerIDs[i]].layerName;
            }
            changeID = GUILayout.Toolbar(changeID, changeLayers);



            GUILayout.BeginVertical("box", GUILayout.Width(position.width - 10), GUILayout.MinHeight(100));


            GUILayout.Space(10);

            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUI.skin.label.normal.textColor = new Color(0.5f, 0.8f, 1.0f, 1.0f);
            GUILayout.Label("当前层级(Sorting layer)：" + layerDatas[layerIDs[changeID]].layerName);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.Label("可选层级(Sorting layer)：" + defaultLayers[layerStds[layerIDs[changeID]].changeID]);
            layerStds[layerIDs[changeID]].changeID = EditorGUILayout.Popup(layerStds[layerIDs[changeID]].changeID, defaultLayers);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            foldout = EditorGUILayout.Foldout(foldout, "层级为 " + layerDatas[layerIDs[changeID]].layerName + " 的物体");
            if (foldout)
            {
                for (int i = 0; i < layerDatas[layerIDs[changeID]].mainObjs.Count; i++)
                {
                    EditorGUILayout.ObjectField(layerDatas[layerIDs[changeID]].mainObjs[i].gm, typeof(Transform), true);
                }
            }

            GUILayout.EndVertical();



            //---提示对应layer的改变---start
            for (int i = 0; i < layerIDs.Count; i++)
            {
                SeletedLayer seleted = layerStds[layerIDs[i]];
                if (seleted.currentName == seleted.changeName)
                {
                    EditorGUILayout.HelpBox(seleted.currentName + " 的层级尚未选择 ", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox(seleted.currentName + " 的层级更改为 " + seleted.changeName, MessageType.Info);
                }
            }
            //---提示对应layer的改变---end


            //---提示前面要有改变---start
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("前面所有提示不报错再确定更改", MessageType.Warning);
            GUILayout.Space(30);
            //---提示前面要有改变---end


            //---------确定改变按钮-----------start
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            if (GUILayout.Button("Apply", GUILayout.MaxWidth(100), GUILayout.MaxHeight(30)))
            {
                for (int i = 0; i < layerDatas.Count; i++)
                {
                    int id = layerIDs[i];
                    int layer = SortingLayer.NameToID(layerStds[id].changeName);

                    if (layerStds[id].changeName != layerStds[id].currentName)
                    {
                        layerDatas[id].SetLayers(layer);
                        Debug.Log(layerStds[id].currentName + " 的层级更改为 " + layerStds[id].changeName);
                    }
                }

                this.Close();
            }
            GUILayout.Label("");
            GUILayout.EndHorizontal();
            //---------确定改变按钮-----------end

            GUILayout.EndScrollView();
        }


        public enum SortType
        {
            Render,
            Canvas
        }

        public class SortTypeData
        {
            public SortType sortType = SortType.Render;

            Canvas m_Canvas;
            Renderer m_Render;

            public GameObject gm
            {
                get
                {
                    if (sortType == SortType.Render)
                        return m_Render.gameObject;
                    else
                        return m_Canvas.gameObject;
                }
            }

            public int SortLayerID
            {
                get
                {
                    if (sortType == SortType.Render)
                        return m_Render.sortingLayerID;
                    else
                        return m_Canvas.sortingLayerID;
                }
            }

            public int OrderInLayer
            {
                get
                {
                    if (sortType == SortType.Render)
                        return m_Render.sortingOrder;
                    else
                        return m_Canvas.sortingOrder;
                }
            }



            public SortTypeData(SortType sortType, GameObject gm)
            {
                this.sortType = sortType;
                if (sortType == SortType.Canvas)
                    m_Canvas = gm.GetComponent<Canvas>();
                else
                    m_Render = gm.GetComponent<Renderer>();
            }

            public void SetSortLayer(int id)
            {
                if (sortType == SortType.Canvas)
                {
                    if (m_Canvas != null)
                    {
                        if (m_Canvas.overrideSorting || m_Canvas.isRootCanvas)
                        {
                            m_Canvas.sortingLayerID = id;
                        }
                    }
                }
                else
                {
                    if (m_Render != null)
                    {
                        m_Render.sortingLayerID = id;
                    }
                }
            }

            public void SetOrderInLayer(int id)
            {
                if (sortType == SortType.Canvas)
                {
                    if (m_Canvas != null)
                    {
                        if (m_Canvas.overrideSorting || m_Canvas.isRootCanvas)
                        {
                            m_Canvas.sortingOrder = id;
                        }
                    }
                }
                else
                {
                    if (m_Render != null)
                    {
                        m_Render.sortingOrder = id;
                    }
                }
            }
        }


        /// <summary>
        /// transfrom 保存的 layer 数据
        /// </summary>
        public class LayerData
        {
            public int layer;
            public string layerName;
            public List<SortTypeData> mainObjs = new List<SortTypeData>();

            public void AddMainObjects(SortTypeData m_objects)
            {
                mainObjs.Add(m_objects);
            }

            public void SetLayers(int nLayer)
            {
                for (int i = 0; i < mainObjs.Count; i++)
                {
                    mainObjs[i].SetSortLayer(nLayer);
                }
            }
        }

        /// <summary>
        /// layer 保存的 选择 数据 
        /// </summary>
        public class SeletedLayer
        {
            public int changeID;
            public int currentID;

            public string changeName
            {
                get { return dLayers[changeID]; }
            }

            public string currentName
            {
                get { return dLayers[currentID]; }
            }

            List<string> dLayers;
            public SeletedLayer(List<string> names)
            {
                dLayers = names;
            }
        }

        /// <summary>
        /// 保存transfrom对应int的数组
        /// </summary>
        public Dictionary<int, LayerData> layerDatas = new Dictionary<int, LayerData>();
        public Dictionary<int, SeletedLayer> layerStds = new Dictionary<int, SeletedLayer>();
        public List<int> layerIDs = new List<int>();

        /// <summary>
        /// 查找所有的Transfrom
        /// </summary>
        /// <param name="target"></param>
        public void FindAllTransfrom(Transform target)
        {
            Transform[] targets = target.GetComponentsInChildren<Transform>(true);

            SortTypeData main;
            for (int i = 0; i < targets.Length; i++)
            {
                if (SoringLayersData.GetMainType(targets[i].gameObject, out main))
                {
                    int layerID = main.SortLayerID;

                    if (layerDatas.ContainsKey(layerID))
                    {
                        layerDatas[layerID].AddMainObjects(main);
                    }
                    else
                    {
                        LayerData data = new LayerData();
                        data.layer = main.SortLayerID;
                        data.layerName = SortingLayer.IDToName(main.SortLayerID);
                        data.AddMainObjects(main);

                        layerIDs.Add(layerID);
                        layerDatas.Add(layerID, data);
                    }


                    if (layerStds.ContainsKey(layerID) == false)
                    {
                        SeletedLayer seleted = new SeletedLayer(dLayers);

                        int id = dLayers.FindIndex((x) => x == SortingLayer.IDToName(layerID));
                        seleted.changeID  = id;
                        seleted.currentID = id;

                        layerStds.Add(layerID, seleted);
                    }
                }
            }
        }

        public class SoringLayersData
        {
            public static List<Type> layerTypes = new List<Type>();

            static void Init()
            {
                layerTypes.Clear();
                layerTypes.Add(typeof(Canvas));
                layerTypes.Add(typeof(TrailRenderer));
                layerTypes.Add(typeof(ParticleSystemRenderer));
            }

            public static bool GetMainType(GameObject gm, out SortTypeData outMain)
            {

                if (layerTypes.Count <= 0)
                {
                    Init();
                }

                for (int i = 0; i < layerTypes.Count; i++)
                {
                    var main = gm.GetComponent(layerTypes[i]);
                    if (main != null)
                    {
                        if (main is Renderer)
                        {
                            outMain = new SortTypeData(SortType.Render, gm);
                            return true;
                        }
                        else
                        {
                            outMain = new SortTypeData(SortType.Canvas, gm);
                            return true;
                        }
                    }
                }
                outMain = null;
                return false;
            }
        }
    }
}


