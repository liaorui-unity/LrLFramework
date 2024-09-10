using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LayerAndSorting
{
    public class LayerAndSortingData : MonoBehaviour
    {
        public static List<LsInfo> lSInfos = new List<LsInfo>();

        /// <summary>
        /// 查找所有的Transfrom
        /// </summary>
        /// <param name="target"></param>
        public static void FindAllTransfrom(Transform target)
        {
            Transform[] targets = target.GetComponentsInChildren<Transform>(true);

            LsInfo main;
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


        public static LsInfo GetMainType(GameObject go)
        {
            for (int i = 0; i < fifterTypes.Count; i++)
            {
                var main = go.GetComponent(fifterTypes[i]);
                if (main != null)
                {
                    if (main is Renderer)
                    {
                        return new LsInfo(SortType.Render, go);
                    }
                    else
                    {
                        return new LsInfo(SortType.Canvas, go);
                    }
                }
            }
            return null;
        }

    }
}