using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;


public class EditorGUICustom
{
    static Dictionary<int, Rect> keyValuePairs = new Dictionary<int, Rect>();

    public static void DrawBox(Rect rect)
    {
        // ���Ʊ߿�
        GUI.Box(rect, "");
        Handles.color = Color.white;

        Handles.DrawLines(new Vector3[]
            {
                new Vector3(rect.x, rect.y),
                new Vector3(rect.x, rect.y + rect.height),
                new Vector3(rect.x, rect.y + rect.height),
                new Vector3(rect.x + rect.width, rect.y + rect.height),
                new Vector3(rect.x + rect.width, rect.y + rect.height),
                new Vector3(rect.x + rect.width, rect.y),
                new Vector3(rect.x + rect.width, rect.y),
                new Vector3(rect.x, rect.y)
            });
    }
    public static void DrawTitleLabel(Rect rect, string label, TextAlignment alignment = TextAlignment.Center)
    {
        // ���Ʊ�ǩ
        var size = GUI.skin.label.CalcSize(new GUIContent(label));
        var labelRect = new Rect(rect);

        int m_height = 18;
        float m_scale = 1.2f;

        if (alignment == TextAlignment.Center)
            labelRect = new Rect(rect.x + (rect.width - size.x * m_scale) / 2f, rect.y + size.y - m_height, size.x * m_scale, size.y);
        else if (alignment == TextAlignment.Left)
            labelRect = new Rect(rect.x, rect.y + (-size.y + m_height), size.x * m_scale, size.y);
        else if (alignment == TextAlignment.Right)
            labelRect = new Rect(rect.x + (rect.width - size.x * m_scale), rect.y + size.y - m_height, size.x * m_scale, size.y);

        EditorGUI.DropShadowLabel(labelRect, label);
    }
    public static bool DrawTitleButton(Rect rect, string label, TextAlignment alignment = TextAlignment.Center)
    {
        // ���Ʊ�ǩ
        var size = GUI.skin.label.CalcSize(new GUIContent(label));
        var labelRect = new Rect(rect);

        int m_height = 19;
        float m_scale = 1.2f;

        if (alignment == TextAlignment.Center)
            labelRect = new Rect(rect.x + (rect.width - size.x * m_scale) / 2f, rect.y + (-size.y + m_height), size.x * m_scale, size.y);
        else if (alignment == TextAlignment.Left)
            labelRect = new Rect(rect.x, rect.y + (-size.y + m_height), size.x * m_scale, size.y);
        else if (alignment == TextAlignment.Right)
            labelRect = new Rect(rect.x + (rect.width - size.x * m_scale), rect.y + (-size.y + m_height), size.x * m_scale, size.y);

        return GUI.Button(labelRect, label, EditorStyles.boldLabel);
    }




    public static bool ButtonTitle(Rect rect, string label, TextAlignment alignment = TextAlignment.Center)
    {
        // ���Ʊ߿�
        DrawBox(rect);

        // ���Ʊ�ǩ(Button)
        return DrawTitleButton(rect, label, alignment);
    }

    public static void BoxColor(Color color, System.Action<Rect> hander)
    {
        var rect  = GUILayoutUtility.GetRect(0, 0);
        var keyID = GUIUtility.GetControlID(FocusType.Passive);

        if (Event.current.type == EventType.Repaint)
        {
            if (keyValuePairs.TryGetValue(keyID, out var temp))
            {
                rect.height = temp.height;
            }
        }

        rect = rect.LessenWidth(1);
        EditorGUI.DrawRect(rect, color);


        var lastY = GUILayoutUtility.GetRect(0, 0);
        GUILayout.BeginHorizontal();
        GUILayout.Space(1);
        GUILayout.BeginVertical();
        hander?.Invoke(rect);
        GUILayout.EndVertical();
        GUILayout.Space(1);
        GUILayout.EndHorizontal();
        var nextY = GUILayoutUtility.GetRect(0, 0);


        rect.height = nextY.y - lastY.y;

        if (Event.current.type == EventType.Repaint)
        {
            keyValuePairs[keyID] = rect;
        }
    }

    public static void DrawBoxTitleColor(string label, Color titleColor, System.Action<Rect> hander)
    {
        var rect = GUILayoutUtility.GetRect(0, 20);

        rect = rect.LessenWidth(3);

        var keyID = GUIUtility.GetControlID(FocusType.Passive);
        var title = rect;

        EditorGUI.DrawRect(title, titleColor);

        if (Event.current.type == EventType.Repaint)
        {
            if (keyValuePairs.TryGetValue(keyID, out var temp))
            {
                rect.height = temp.height;
            }
        }


        // ���Ʊ߿�
        DrawBox(rect);


        var lastY = GUILayoutUtility.GetRect(0, 0);

        GUILayout.BeginHorizontal();
        GUILayout.Space(2);
        GUILayout.BeginVertical();
        hander?.Invoke(rect);
        GUILayout.EndVertical();
        GUILayout.Space(2);
        GUILayout.EndHorizontal();

        var nextY = GUILayoutUtility.GetRect(0, 0);

        rect.height = nextY.y - lastY.y + 20;
        rect = rect.LessenWidth(10);


        if (Event.current.type == EventType.Repaint)
        {
            keyValuePairs[keyID] = rect;
        }

        DrawTitleLabel(rect, label, TextAlignment.Center);
    }

    public static void DrawTitleColor(string label, Color titleColor, System.Action<Rect> hander)
    {
        var rect = GUILayoutUtility.GetRect(0, 20);
        var keyID = GUIUtility.GetControlID(FocusType.Passive);
        var title = rect;

        EditorGUI.DrawRect(title, titleColor);

        if (Event.current.type == EventType.Repaint)
        {
            if (keyValuePairs.TryGetValue(keyID, out var temp))
            {
                rect.height = temp.height;
            }
        }


        var lastY = GUILayoutUtility.GetRect(0, 0);

        hander?.Invoke(rect);

        var nextY = GUILayoutUtility.GetRect(0, 0);


        rect.height = nextY.y - lastY.y + 20;
        if (Event.current.type == EventType.Repaint)
        {
            keyValuePairs[keyID] = rect;
        }

        // ���Ʊ�ǩ
        DrawTitleLabel(rect, label, TextAlignment.Center);
    }


    public static void FoldedBox(string label, ref bool fold, System.Action<Rect> hander)
    {
        var rect  = GUILayoutUtility.GetRect(0, 20);
        var keyID = GUIUtility.GetControlID(FocusType.Passive);
        var title = rect;

        var lastY = GUILayoutUtility.GetRect(0, 0);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        if(fold)
        {
            EditorGUILayout.BeginVertical();
            hander?.Invoke(rect);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(2);
        GUILayout.EndHorizontal();


        var nextY = GUILayoutUtility.GetRect(0, 0);
    
        rect.height = nextY.y - lastY.y + 20;

        var box = rect.Lessen(new Vector2(2,1));

        //DrawBox(box);
        EditorGUI.DrawRect(box, new Color(0.5f, 0.5f, 0.5f, 0.2f));
        EditorGUI.DrawRect(title, Color.gray);

        if (Event.current.type == EventType.Repaint)
        {
            if (keyValuePairs.TryGetValue(keyID, out var temp))
            {
                rect.height = temp.height;
            }
        }

        if (Event.current.type == EventType.Repaint)
        {
            keyValuePairs[keyID] = rect;
        }
        rect = rect.LessenWidth(25);

        DrawTitleLabel(rect, label, TextAlignment.Left);

        //显示可以折叠框的箭头
        if (fold)
        {
            GUI.Label(new Rect(5, rect.y, 20, 20), EditorGUIUtility.IconContent("IN foldout on"));
        }
        else
        {
            GUI.Label(new Rect(5, rect.y, 20, 20), EditorGUIUtility.IconContent("IN foldout"));
        }
        GUILayout.Space(2);
        if (Event.current.type == EventType.MouseDown && title.Contains(Event.current.mousePosition))
        {
            fold = !fold;
            Event.current.Use();
        }
    }

    public static string HandleDragAndDrop(Rect dropArea, string inPath, int mode = 0)
    {
        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (!dropArea.Contains(evt.mousePosition))
                return inPath;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (var draggedObject in DragAndDrop.objectReferences)
                {
                    if (draggedObject is DefaultAsset)
                    {
                        string path = AssetDatabase.GetAssetPath(draggedObject);

                        if (mode == 0)
                        {
                            return inPath = path;
                        }
                        else if (mode == 1)
                        {
                            if (AssetDatabase.IsValidFolder(path))
                            {
                                return inPath = path;
                            }
                        }
                        else if (mode == 2)
                        {
                            if (AssetDatabase.IsValidFolder(path) == false)
                            {
                                return inPath = path;
                            }
                        }
                    }
                }
            }

            evt.Use();
        }

        return inPath;
    }

    public static Object DragObject(Rect dropArea)
    {
        Object @object = null;

        if (dropArea.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var path in DragAndDrop.paths)
                {
                    @object = AssetDatabase.LoadAssetAtPath<Object>(path);
                    break;
                }
                Event.current.Use();
            }
        }

        return @object;
    }

    public static Rect OnDragGUI(Rect rect, ref bool isDragging, ref Vector2 offset, System.Action<Rect> OnGUI)
    {
        var m_titleRect = rect;
        m_titleRect.y -= 20;
        m_titleRect.height = 20;
        var type = Event.current.type;
        GUI.Button(m_titleRect, "");


        Event.current.type = type;
        OnGUI?.Invoke(m_titleRect);


        switch (type)
        {
            case EventType.MouseDown:
                if (m_titleRect.Contains(Event.current.mousePosition))
                {
                    isDragging = true;
                    offset = Event.current.mousePosition - m_titleRect.position;
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    m_titleRect.position = Event.current.mousePosition - offset;
                    rect.position = m_titleRect.position + new Vector2(0, 20);
                }
                break;

            case EventType.MouseUp:
                isDragging = false;
                break;
        }

        return rect;
    }

}

public static class CustomExtend
{
    public static Rect LessenWidth(this Rect rect, int width)
    {
        rect.width -= width * 2;
        rect.x += width;
        return rect;
    }

    public static Rect LessenHeight(this Rect rect, int height)
    {
        rect.height -= height * 2;
        rect.y += 2;
        return rect;
    }

    public static Rect Lessen(this Rect rect, Vector2 size)
    {
        rect =  LessenWidth(rect, (int)size.x);
        rect =  LessenHeight(rect, (int)size.y);

        return rect;
    }

}

public class EditorCustomDisplay : EditorWindow
{
    public string msg;
    public string ok;
    public string cancal;

    private const float WindowWidth = 300;
    private const float WindowHeight = 150;


    public System.Action OnOK;
    public System.Action OnCancal;


    public static EditorCustomDisplay Show(string title, string msg, string ok, string cancal)
    {
        var window = CreateInstance<EditorCustomDisplay>();
        window.titleContent = new GUIContent(title);
        window.position = new Rect
        (
            (Screen.currentResolution.width - WindowWidth) / 2f,
            (Screen.currentResolution.height - WindowHeight) / 2f,
             WindowWidth,
             WindowHeight
        );
        window.ShowUtility();

        window.msg = msg;
        window.ok = ok;
        window.cancal = cancal;

        return window;
    }



    private void OnEnable()
    {
        buttonSty = new GUIStyle();
        buttonSty.normal.background = Texture2D.whiteTexture;
        buttonSty.hover.background = Texture2D.blackTexture;

        buttonSty.normal.textColor = Color.black;
        buttonSty.alignment = TextAnchor.MiddleCenter;

        defaultSty = new GUIStyle();
        defaultSty.normal.background = Texture2D.normalTexture;
        defaultSty.hover.background = Texture2D.blackTexture;

        defaultSty.normal.textColor = Color.black;
        defaultSty.alignment = TextAnchor.MiddleCenter;

        textSty = new GUIStyle();
        textSty.normal.textColor = Color.yellow;
        textSty.fontSize = 20;
        textSty.fontStyle = FontStyle.Bold;
        textSty.alignment = TextAnchor.MiddleCenter;

        minSize = new Vector2(WindowWidth, WindowHeight);
        maxSize = new Vector2(WindowWidth, WindowHeight);


    }

    GUIStyle textSty;
    GUIStyle buttonSty;
    GUIStyle defaultSty;

    float m_startTime;
    float m_waitTime;
    float m_runTime;
    bool m_value;
    bool m_has;

    public void WaitHander(float waitTime, bool defaultValue)
    {
        EditorApplication.update += Wait;

        m_startTime = Time.realtimeSinceStartup;
        m_waitTime = waitTime;
        m_value = defaultValue;
        m_has = true;
        m_runTime = 0;
    }

    void Wait()
    {
        m_runTime = m_waitTime - (Time.realtimeSinceStartup - m_startTime);
        if (m_runTime <= 0)
        {
            if (m_value)
            {
                OkCall();
            }
            else
            {
                CancalCall();
            }
        }
    }


    private Rect _windowRect;
    private void OnGUI()
    {
        _windowRect.width = position.width;
        _windowRect.height = position.height;
        _windowRect.x = 0;
        _windowRect.y = 0;

        EditorGUI.DrawRect(_windowRect, color: Color.grey);

        GUILayout.BeginVertical();
        {
            EditorGUICustom.BoxColor(Color.gray, (rect) =>
            {
                GUILayout.Space(30);

                var temp = rect;
                temp.x += 5;
                temp.y += 5;
                temp.width = 100;
                temp.height = position.height - 10;

                EditorGUICustom.DrawBox(temp);

                temp.x = temp.width / 2 - 10;
                GUI.Label(temp, EditorGUIUtility.IconContent("console.warnicon"));

                GUILayout.Space(30);
            });

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(110);
                GUILayout.FlexibleSpace();
                GUILayout.Label(msg, textSty);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(110);

                var okStr = ok;
                var cancalStr = cancal;

                if (m_has)
                {
                    if (m_value)
                    {
                        okStr = $"{ok}({(int)m_runTime})";
                    }
                    else
                    {
                        cancalStr = $"{cancal}({(int)m_runTime})";
                    }
                }

                var t_value = m_has && m_value;


                if (GUILayout.Button(okStr, t_value ? defaultSty : buttonSty, GUILayout.Width(60)))
                {
                    OkCall();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(cancalStr, !t_value ? defaultSty : buttonSty, GUILayout.Width(60)))
                {
                    CancalCall();
                }

                GUILayout.Space(10);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
        GUILayout.EndVertical();

        Repaint();
    }

    public void OkCall()
    {
        OnOK?.Invoke();
        Replace();
        this.Close();
    }

    public void CancalCall()
    {
        OnCancal?.Invoke();
        Replace();
        this.Close();
    }

    void Replace()
    {
        if (m_has)
            EditorApplication.update -= Wait;

        m_has = false;
        OnOK = null;
        OnCancal = null;

    }

    void OnDisable()
    {
        OnCancal?.Invoke();
        Replace();
    }
}


