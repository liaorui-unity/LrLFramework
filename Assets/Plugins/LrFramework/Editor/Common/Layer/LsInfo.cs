using System.Collections;
using System.Collections.Generic;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LayerAndSorting
{

    public enum SortType
    {
        Render,
        Canvas,
        Particle
    }

    public interface IInfo
    {
        GameObject mainGo { get; set; }  
        string SortLayerName { get; set; }
        int SortLayerSortID { get; set; }
        int OrderInLayer { get; set; }
        int Layer { get; set; }
    }


    public class ParticleInfo: IInfo
    { 
        ParticleSystemRenderer _particle;
        public ParticleSystemRenderer particle
        { 
            get
            {
                if (_particle == null)
                { 
                    _particle = mainGo.GetComponent<ParticleSystemRenderer>();
                }
                return _particle;
            }
        }

        public GameObject mainGo
        {
            get; set;
        }
        public string SortLayerName { get => particle.sortingLayerName; set => particle.sortingLayerName = value; }
        public int SortLayerSortID { get; set; } = -100;
        public int OrderInLayer { get => particle.sortingOrder; set => particle.sortingOrder =value; }
        public int Layer { get => particle.gameObject.layer; set => particle.gameObject.layer =value; }
    }


    public class CanvasInfo : IInfo
    {
        Canvas _canvas;
        public Canvas canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = mainGo.GetComponent<Canvas>();
                }
                return _canvas;
            }
        }

        public GameObject mainGo
        {
            get; set;
        }
        public string SortLayerName { get => canvas.sortingLayerName; set => canvas.sortingLayerName = value; }
        public int SortLayerSortID { get; set; } = -100;
        public int OrderInLayer { get => canvas.sortingOrder; set => canvas.sortingOrder = value; }
        public int Layer { get => canvas.gameObject.layer; set => canvas.gameObject.layer = value; }
    }


    public class RenderInfo : IInfo
    {
        Renderer _renderer;
        public Renderer renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = mainGo.GetComponent<Renderer>();
                }
                return _renderer;
            }
        }

        public GameObject mainGo
        {
            get; set;
        }
        public string SortLayerName { get => renderer.sortingLayerName; set => renderer.sortingLayerName = value; }
        public int SortLayerSortID { get; set; } = -100;
        public int OrderInLayer { get => renderer.sortingOrder; set => renderer.sortingOrder = value; }
        public int Layer { get => renderer.gameObject.layer; set => renderer.gameObject.layer = value; }
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
            set => info.Layer = value;
        }

        public string SortLayerName
        {
            get => info.SortLayerName;
            set => info.SortLayerName = value;
        }

        public int SortLayerSortID
        {
            get => info.SortLayerSortID;
            set => info.SortLayerSortID = value;
        } 


        public int OrderInLayer
        {
            get => info.OrderInLayer;
            set => info.OrderInLayer = value;
        }

        public LsInfo(SortType sortType, GameObject go, int id)
        {
            if (go == null)
                return;

            this.sortType = sortType;

            if (sortType == SortType.Canvas)
                info = new CanvasInfo();
            else if (sortType == SortType.Render)
                info = new RenderInfo();
            else
                info = new ParticleInfo();

            mainGo = go;
            name = go.name;
            depth = 0;
            this. id = id+3;
        }
   
    }
}
