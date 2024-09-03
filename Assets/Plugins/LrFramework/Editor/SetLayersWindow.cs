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

namespace Table
{

	public class SetLayersWindow : EditorWindow 
	{

        public SetLayersWindow()
        {
            this.titleContent = new GUIContent("Layers");
        }

        [MenuItem("GameObject/Layers窗口", false, -1)]
        public static void CreateLayerWindows()
        {
            EditorWindow.GetWindow(typeof(SetLayersWindow));
        }

   

        List<string> dLayers = new List<string>();
        string[] defaultLayers;
        private void OnEnable()
        {
            for (int i = 0; i < 32; i++)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i)) == false)
                    dLayers.Add(LayerMask.LayerToName(i));
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
            scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.Width(position.width),GUILayout.Height(position.height));

            GUILayout.Space(20);
            if (changeLayers == null)
                changeLayers = new string[layerIDs.Count];
            for (int i = 0; i < layerIDs.Count; i++)
            {
                changeLayers[i] = layerDatas[layerIDs[i]].layerName;
            }
            changeID = GUILayout.Toolbar(changeID, changeLayers);

      

            GUILayout.BeginVertical("box", GUILayout.Width(position.width-10), GUILayout.MinHeight(100));


            GUILayout.Space(10);

            
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUI.skin.label.normal.textColor = new Color(0.5f, 0.8f, 1.0f, 1.0f);
            GUILayout.Label("当前层级(layer)："+ layerDatas[layerIDs[changeID]].layerName);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.Label("可选层级(layer)："+ defaultLayers[layerStds[layerIDs[changeID]].changeID]);
            layerStds[layerIDs[changeID]].changeID = EditorGUILayout.Popup(layerStds[layerIDs[changeID]].changeID, defaultLayers);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            foldout = EditorGUILayout.Foldout(foldout, "层级为 "+ layerDatas[layerIDs[changeID]].layerName + " 的物体");
            if (foldout)
            {
                for (int i = 0; i < layerDatas[layerIDs[changeID]].mainObjs.Count; i++)
                {
                    EditorGUILayout.ObjectField(layerDatas[layerIDs[changeID]].mainObjs[i], typeof(Transform), true);
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
                    EditorGUILayout.HelpBox(seleted.currentName + " 的层级更改为 "+ seleted.changeName, MessageType.Info);
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
                    int layer = LayerMask.NameToLayer(layerStds[id].changeName);

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

        /// <summary>
        /// transfrom 保存的 layer 数据
        /// </summary>
        public class LayerData
        {
            public int layer;
            public string layerName;
            public List<GameObject> mainObjs = new List<GameObject>();

            public void AddMainObjects(GameObject m_objects)
            {
                mainObjs.Add(m_objects);
            }

            public void SetLayers(int nLayer)
            {
                for (int i = 0; i < mainObjs.Count; i++)
                {
                    mainObjs[i].layer = nLayer;
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

            for (int i = 0; i < targets.Length; i++)
            {
                int layerID = targets[i].gameObject.layer;

                if (layerDatas.ContainsKey(layerID))
                {
                    layerDatas[layerID].AddMainObjects(targets[i].gameObject);
                }
                else
                {
                    LayerData data = new LayerData();
                    data.layer = targets[i].gameObject.layer;
                    data.layerName = LayerMask.LayerToName(targets[i].gameObject.layer);
                    data.AddMainObjects(targets[i].gameObject);

                    layerIDs.Add(layerID);
                    layerDatas.Add(layerID, data);
                }


                if (layerStds.ContainsKey(layerID)==false)
                {
                    SeletedLayer seleted = new SeletedLayer(dLayers);

                    int id= dLayers.FindIndex((x) => x == LayerMask.LayerToName(layerID));
                    seleted.changeID = id;
                    seleted.currentID = id;
 
                    layerStds.Add(layerID,seleted);
                }
            }
        }

    }
}
