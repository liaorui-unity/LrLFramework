using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System;

namespace LayerAndSorting
{
    public enum SortType
    {
        Render,
        Canvas
    }


    public class LSInfo
    {
       
    }

    public class LayerAndSortingWindow : EditorWindow
    {
        [MenuItem("Tools/LayerAndSortingWindow")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(LayerAndSortingWindow));
        }


        public List<LSInfo> lSInfos = new List<LSInfo>();

        public GameObject selectGo;

        

        private void OnGUI()
        {

            GUILayout.Label("Layer And Sorting", EditorStyles.boldLabel);
            GUILayout.Space(10);

            selectGo = EditorGUILayout.ObjectField("Select GameObject", selectGo, typeof(GameObject), true) as GameObject;

            if (selectGo == null)
            {
                return;
            }

            if (GUILayout.Button("Set Layer And Sorting"))
            {
                FindAllTransfrom(selectGo.transform);
            }

            if (lSInfos.Count > 0)
            {
                for (int i = 0; i < lSInfos.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(lSInfos[i].mainGo.name);
                    GUILayout.Label("Layer");
                    lSInfos[i].Layer = EditorGUILayout.LayerField(lSInfos[i].Layer);
                    GUILayout.Label("SortLayerID");
                    lSInfos[i].SetSortLayer(EditorGUILayout.IntField(lSInfos[i].SortLayerID));
                    GUILayout.Label("OrderInLayer");
                    lSInfos[i].SetOrderInLayer(EditorGUILayout.IntField(lSInfos[i].OrderInLayer));
                    GUILayout.EndHorizontal();
                }
            }
        }


        /// <summary>
        /// 查找所有的Transfrom
        /// </summary>
        /// <param name="target"></param>
        public void FindAllTransfrom(Transform target)
        {
            Transform[] targets = target.GetComponentsInChildren<Transform>(true);

            LSInfo main;
            for (int i = 0; i < targets.Length; i++)
            {
                main = GetMainType(targets[i].gameObject);
                if (main != null)
                {
                    lSInfos.Add(main);
                }
            }
        }

        static List<Type> fifterTypes = new List<Type>()
         {
            typeof(Canvas),
            typeof(TrailRenderer),
            typeof(ParticleSystemRenderer),
            typeof(SpriteRenderer)
        };
         

        public static LSInfo GetMainType(GameObject go)
        {
            for (int i = 0; i < fifterTypes.Count; i++)
            {
                var main = go.GetComponent(fifterTypes[i]);
                if (main != null)
                {
                    if (main is Renderer)
                    {
                        return new LSInfo(SortType.Render, go);
                    }
                    else
                    {
                        return new LSInfo(SortType.Canvas, go);
                    }
                }
            }
            return null;
        }

      
    }
}
