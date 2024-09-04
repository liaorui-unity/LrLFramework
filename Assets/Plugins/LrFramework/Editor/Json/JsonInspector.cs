using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.Events;
using Table;

public class JsonInspector : Editor, IMonoAsset
{
    public class GUIFold
    {
        public bool isFold;
    }

    public struct SelectGUI
    {
        public bool isHas;
        public int frame;
        public Rect rect;
        public JSONObject JSON;
    }


    int        m_frame = 0;
    int        m_layerId    = 0;
    bool       m_isSetDirty = false;
    string     m_jsonPath;
    JSONObject m_jSONObject;
    SelectGUI  m_select     = new SelectGUI();

    GUIStyle alphaStyle;
    GUIStyle boxStyle;

    Color back = new Color(0.0f, 0.0f, 0.0f, 1f);

    public Dictionary<JSONObject, GUIFold> folds = new Dictionary<JSONObject, GUIFold>();


    static JsonInspector jsonInspector;



    [InitializeOnLoadMethod]
    static void JsonInit()
    {
        DefaultAssetInspector.OnSelectAsset.AddListener(OnSelectAsset);
    }


    public static void OnSelectAsset(UnityEngine.Object uobject, AssetAction call)
    {
        var m_AssetPath = AssetDatabase.GetAssetPath(uobject);
        if (m_AssetPath.EndsWith(".json"))
        {
            jsonInspector = CreateEditor(uobject, typeof(JsonInspector)) as JsonInspector;
            call?.Invoke(uobject, jsonInspector as IMonoAsset);
        }
    }



    private void OnEnable()
    {
        alphaStyle = new GUIStyle();
        alphaStyle.normal.background = OnNormalAlpha;
        alphaStyle.hover .background = OnHoverAlpha;

        boxStyle=new GUIStyle();
        boxStyle.normal.background = OnActiveAlpha;

        backColor = ColorUtils.HexToColor("383838");
    }

    public void OnNewHeaderGUI() { }


    Color backColor;

    public override void OnInspectorGUI()
    {
        if (m_jSONObject == null)
        {
            folds.Clear();
            m_jsonPath = AssetDatabase.GetAssetPath(target);
            if (m_jsonPath.EndsWith(".json"))
            {
                string jsonStr = File.ReadAllText(m_jsonPath);
                m_jSONObject = new JSONObject(jsonStr);
            }

            if (m_jSONObject == null)
                return;
        }

        m_frame += 1;
        GUI.enabled = true;

        GUI.skin.box.alignment = TextAnchor.MiddleLeft;
        GUI.skin.box.fontStyle = FontStyle.Bold;
        {
            EditorGUICustom.DrawBoxTitleColor(target.name, backColor, (rect) =>
                     {
                         var icon = new Rect(rect.x+1, rect.y+1, rect.width-2, rect.height-2);
                         GUI.Label(icon, OnJs);
                     });

            EditorGUICustom.BoxColor(backColor, (rect) =>
            {
                if (m_select.isHas)
                    GUI.Box(m_select.rect, "", boxStyle);

                for (int i = 0; i < m_jSONObject.list.Count; i++)
                {
                    m_layerId = 0;
                    CheckType(m_jSONObject.list[i], m_jSONObject.keys[i]);
                }
            });

            if (m_select.isHas && m_select.frame != m_frame)
            {
                SelectNotHas();
            }
        }
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        GUI.skin.box.alignment    = TextAnchor.MiddleCenter;

  
        //有改变的话，就保存
        if (m_isSetDirty)
        {
            m_isSetDirty = false;


            var jsonStr = m_jSONObject.ToString();

            File.WriteAllText(m_jsonPath, jsonStr);
        }
    }

    private void SelectNotHas()
    {
        m_select.isHas = false;
        m_select.frame = 0;
    }


    private void CheckType(JSONObject item, string name)
    {
        if (item.IsArray)
        {
            ShowArrayField(item, name);
        }
        else if (item.IsObject)
        {
            ShowClassField(item, name);
        }
        else
        {
            ShowValueField(item, name);
        }
    }


    public void  ShowValueField(JSONObject jSON,string name)
    {
        if (jSON.IsString)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name,GUILayout.Width(100));
            CheckValue(GUILayout.TextArea(jSON.str),ref jSON.str);
            GUILayout.EndHorizontal();
        }
        else if (jSON.IsBool)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(100));
            CheckValue(EditorGUILayout.Toggle(jSON.b), ref jSON.b);
            GUILayout.EndHorizontal();
        }
        else if (jSON.IsNumber)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(100));
            if (jSON.useInt)
            {
                CheckValue(EditorGUILayout.LongField(jSON.i), ref jSON.i);
            }
            else
            {
                CheckValue(EditorGUILayout.FloatField(jSON.n), ref jSON.n);
            }
            GUILayout.EndHorizontal();
        }
    }

    public void  ShowArrayField(JSONObject jSON, string name)
    {
        var curID = m_layerId += 1;
 
        GUIFold isFold = LayoutFold(jSON,name);

        if (isFold.isFold)
        {
            for (int i = 0; i < jSON.list.Count; i++)
            {
                m_layerId = curID;
                var item = jSON.list[i];
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(20 * m_layerId);
                    GUILayout.BeginVertical();
                    {
                        var start = GUILayoutUtility.GetRect(0, 0);
                        CheckType(item, $"Element {i}");
                        var end = GUILayoutUtility.GetRect(0, 0);

                        var calculateRect = new Rect(start.x, start.y, end.width, end.y - start.y);

                        if (GUI.Button(calculateRect, "", alphaStyle))
                        {
                            m_select.isHas = true;
                            m_select.rect  = calculateRect;
                            m_select.JSON  = item;
                        }

                        if (m_select.JSON == item)
                        {
                            m_select.frame = m_frame;
                            if (calculateRect.height > 0) m_select.rect = calculateRect;
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

            ButtonWindow(jSON);
        }
    }

    private void ButtonWindow(JSONObject jSON)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (jSON.list.Count > 0)
            {
                if (GUILayout.Button("+", GUILayout.Width(88)))
                {
                    var json = jSON.list[0].Copy();
                    jSON.Add(json);
                }

                if (GUILayout.Button("-", GUILayout.Width(88)))
                {
                    if (jSON.list.Contains(m_select.JSON))
                    {
                        jSON.list.Remove(m_select.JSON);
                    }
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    public void  ShowClassField(JSONObject jSON, string name)
    {
        var curID = m_layerId += 1;
        GUIFold isFold = LayoutFold(jSON,name);

        if (isFold.isFold)
        {
            for (int i = 0; i < jSON.list.Count; i++)
            {
                m_layerId = curID;
                var item = jSON.list[i];
                GUILayout.BeginHorizontal();
                GUILayout.Space(20 * m_layerId);
                GUILayout.BeginVertical();
                CheckType(item, jSON.keys[i]);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
    }


    private void CheckValue<T>(T value,ref T lastValue) 
    {
        if (value.Equals(lastValue) == false)
        {
            lastValue  = value;
            m_isSetDirty = true;
        }
    }

    private GUIFold LayoutFold(JSONObject jSON, string name)
    {
        if (folds.TryGetValue(jSON, out GUIFold isFold) == false)
        {
            isFold = new GUIFold() { isFold = false };
            folds.Add(jSON, isFold);
        }

        isFold.isFold = Fold(isFold.isFold, name);
        return isFold;
    }


    public  bool Fold(bool value, string name, bool isCanSelect = true)
    {
        var IsValue = value;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(value ? OnTex : OffTex , GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
        {
            IsValue = !IsValue;
        }

        if (!string.IsNullOrEmpty(name))
        {
            if (isCanSelect)
            {
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                if (GUILayout.Button(name))
                {
                    IsValue = !IsValue;
                }
            }
            else
            {
                GUILayout.Box(name);
            }
        }

        GUILayout.EndHorizontal();

        return IsValue;
    }

 
    public void OnDestroy()
    {
        folds.Clear();

        m_select.JSON = null;
        m_jSONObject = null;
        jsonInspector = null;
    }

    static Texture2D offTex;
    static Texture2D onTex;
    static Texture2D onJs;
    static Texture2D onNormalAlpha;
    static Texture2D onHoverAlpha;
    static Texture2D onActiveAlpha;

    static Texture2D OffTex => offTex = offTex ?? Resources.Load<Texture2D>("JsonIcon/Off");
    static Texture2D OnTex => onTex = onTex ?? Resources.Load<Texture2D>("JsonIcon/On");
    static Texture2D OnJs => onJs = onJs ?? Resources.Load<Texture2D>("JsonIcon/js");
    static Texture2D OnNormalAlpha => onNormalAlpha = onNormalAlpha ?? Resources.Load<Texture2D>("JsonIcon/normal");
    static Texture2D OnHoverAlpha  => onHoverAlpha = onHoverAlpha ?? Resources.Load<Texture2D>("JsonIcon/hover");
    static Texture2D OnActiveAlpha => onActiveAlpha = onActiveAlpha ?? Resources.Load<Texture2D>("JsonIcon/active");
}