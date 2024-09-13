using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LayerAndSorting
{
    public class LayerAndSortingData : MonoBehaviour
    {
        public static List<LsInfo> lSInfos = new List<LsInfo>();

        public static LsInfo root = new LsInfo(SortType.Render, null, -1) { depth = -1, name = "Root" };


        /// <summary>
        /// 查找所有的Transfrom
        /// </summary>
        /// <param name="target"></param>
        public static void FindAllTransfrom(Transform target)
        {
            Transform[] targets = target.GetComponentsInChildren<Transform>(true);

            lSInfos.Clear();

            for (int i = 0; i < targets.Length; i++)
            {
                var main = GetMainType(targets[i].gameObject, lSInfos.Count+1);
                if (main != null)
                {
                    main.depth = 0;
                    lSInfos.Add(main);
                }
            }
        }

        static List<Type> fifterTypes = new List<Type>()
         {
            typeof(Canvas),
            typeof(Renderer),
            typeof(SortingGroup),
        };


        static LsInfo GetMainType(GameObject go, int id)
        {
            for (int i = 0; i < fifterTypes.Count; i++)
            {
                var main = go.GetComponent(fifterTypes[i]);
                if (main != null)
                {
                    if (main is Renderer)
                    {
                        return new LsInfo(SortType.Render, go, id);
                    }
                    else if (main is Canvas)
                    {
                        return new LsInfo(SortType.Canvas, go, id);
                    }
                    else if (main is SortingGroup)
                    {
                        return new LsInfo(SortType.SortGrop, go, id);
                    }
                }
            }
            return null;
        }

    }
}