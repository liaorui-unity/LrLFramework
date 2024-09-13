using LogInfo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.ParticleSystem;

namespace LayerAndSorting
{

    public enum SortType
    {
        Render,
        Canvas,
        SortGrop
    }

    public interface IInfo
    {
        GameObject mainGo { get; set; }  
        string SortLayerName { get; set; }
        int SortLayerSortID { get; set; }
        int OrderInLayer { get; set; }
        int Layer { get; set; }
    }


    public class SingleInfo<T> 
    {
        T value;
        public T Value
        {
            get
            {
                if (value == null)
                {
                    value = mainGo.GetComponent<T>();
                }
                return value;
            }
        }

        public GameObject mainGo
        {
            get; set;
        }
    }


    public class SortingGropInfo : SingleInfo<SortingGroup>, IInfo
    {
        public string SortLayerName { get => Value.sortingLayerName; set => Value.sortingLayerName = value; }
        public int SortLayerSortID { get; set; } = -100;
        public int OrderInLayer { get => Value.sortingOrder; set => Value.sortingOrder = value; }
        public int Layer { get => Value.gameObject.layer; set => Value.gameObject.layer = value; }
    }

    public class CanvasInfo  : SingleInfo<Canvas>, IInfo
    {
        public string SortLayerName { get => Value.sortingLayerName; set => Value.sortingLayerName = value; }
        public int SortLayerSortID { get; set; } = -100;
        public int OrderInLayer { get => Value.sortingOrder; set => Value.sortingOrder = value; }
        public int Layer { get => Value.gameObject.layer; set => Value.gameObject.layer = value; }
    }


    public class RenderInfo : SingleInfo<Renderer>, IInfo
    {
        public string SortLayerName { get => Value.sortingLayerName; set => Value.sortingLayerName = value; }
        public int SortLayerSortID { get; set; } = -100;
        public int OrderInLayer { get => Value.sortingOrder; set => Value.sortingOrder = value; }
        public int Layer { get => Value.gameObject.layer; set => Value.gameObject.layer = value; }
    }


    public class LsInfo : TreeElement
    {
        public SortType sortType = SortType.Render;

        public IInfo info;

        public GameObject mainGo
        {
            get => info.mainGo;
            set => info.mainGo = value;
        }

        public int Layer
        {
            get => info.Layer;
            set
            {
                if (info.Layer != value)
                {
                    info.Layer = value;
                }
            }
        }

        public string SortLayerName
        {
            get => info.SortLayerName;
            set 
            {
                if (info.SortLayerName != value)
                {
                    info.SortLayerName = value;
                }
            }
        }

        public int SortLayerSortID
        {
            get => info.SortLayerSortID;
            set 
            {
                if (info.SortLayerSortID != value)
                {
                    info.SortLayerSortID = value;
                }
            }
        } 


        public int OrderInLayer
        {
            get => info.OrderInLayer;
            set
            {
                if (info.OrderInLayer != value)
                {
                    info.OrderInLayer = value;
                }
            }
            
        }

        public LsInfo(SortType sortType, GameObject go, int id)
        {
            if (go == null)
                return;

            this.sortType = sortType;

            if (sortType == SortType.Canvas)
                info = new CanvasInfo();
            else if (sortType == SortType.SortGrop)
                info = new SortingGropInfo();
            else if (sortType == SortType.Render)
                info = new RenderInfo();
          
         
            mainGo = go;
            name   = go.name;
            this. id = id;
        }
   
    }
}
