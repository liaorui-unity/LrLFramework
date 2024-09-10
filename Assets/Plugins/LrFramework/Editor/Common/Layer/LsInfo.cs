using System.Collections;
using System.Collections.Generic;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LayerAndSorting
{

    public enum SortType
    {
        Render,
        Canvas
    }

    public class LsInfo : TreeElement
    {
        public SortType sortType = SortType.Render;

        Canvas m_Canvas;
        Renderer m_Render;

        public GameObject mainGo
        {
            get
            {
                if (sortType == SortType.Render)
                    return m_Render.gameObject;
                else
                    return m_Canvas.gameObject;
            }
        }

        public int Layer
        {
            get
            {
                if (sortType == SortType.Render)
                    return m_Render.gameObject.layer;
                else
                    return m_Canvas.gameObject.layer;
            }
            set
            {
                if (sortType == SortType.Render)
                    m_Render.gameObject.layer = value;
                else
                    m_Canvas.gameObject.layer = value;
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

        public LsInfo(SortType sortType, GameObject go)
        {
            if (go == null)
                return;

            this.sortType = sortType;
            if (sortType == SortType.Canvas)
                m_Canvas = go.GetComponent<Canvas>();
            else
                m_Render = go.GetComponent<Renderer>();
        }


        public void SetLayer(int id)
        {
            if (sortType == SortType.Canvas)
            {
                if (m_Canvas != null)
                {
                    m_Canvas.gameObject.layer = id;
                }
            }
            else
            {
                if (m_Render != null)
                {
                    m_Render.gameObject.layer = id;
                }
            }
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
}
