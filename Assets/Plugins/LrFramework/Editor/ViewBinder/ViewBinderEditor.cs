// PrefabObjBinder编辑器

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ViewBinderEditor : EditorWindow
{
    internal struct SelectInfo
    {
        internal ViewBinder.Item m_selected;
        internal Rect box;
    }

    internal GameObject m_prefabObjBinderObj;
    private ViewBinder m_binder;
    private List<ViewBinder.Item> m_itemList;
    private List<ViewBinder.Item> m_searchMatchItemList = new List<ViewBinder.Item>();
    private Vector2 m_scrollViewPos;
    private Vector2 m_scrollComponent;
    private List<Component> m_comList = new List<Component>();
    private string m_itemName;

    private SelectInfo m_selectedInfo;

    private string m_itemNameSearch;
    private string m_selectedItemName;
    private string m_lockBtnName;
    private string m_lockIconName;
    private Object m_itemObj;
    private bool m_lock;
    private string m_componentStr;
    enum ItemOption
    {
        AddItem,
        RemoveItem,
        ReplaceItem,
        ClearItems,
        SearchItems
    }

    private GUIStyle m_labelSytleYellow;
    private GUIStyle m_labelStyleNormal;

    private GUIContent m_gameUIContent;

    private GUIContent m_selectContent;

    public static void ShowWindow()
    {
        var window = GetWindow<ViewBinderEditor>();
        window.titleContent = new GUIContent("预设对象绑定", AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.EventSystems.EventSystem)), "decent");
        window.Init();
    }

    [MenuItem("GameObject/PrefabObjBinder Window", priority = 0)]
    public static void PrefabObjBinderWindow()
    {
        if (Selection.activeGameObject.GetComponent<ViewBinder>())
            ShowWindow();
        else
            Debug.LogError("no PrefabObjBinder on this GameObject");
    }

    void Awake()
    {
        m_labelStyleNormal                  = new GUIStyle(EditorStyles.miniButton);
        m_labelStyleNormal.fontSize         = 12;
        m_labelStyleNormal.normal.textColor = Color.white;
        m_labelStyleNormal.alignment        = TextAnchor.MiddleLeft;

        m_labelSytleYellow                  = new GUIStyle(EditorStyles.miniButton);
        m_labelSytleYellow.fontSize         = 12;
        m_labelSytleYellow.normal.textColor = Color.yellow;
        m_labelSytleYellow.alignment        = TextAnchor.MiddleLeft;

        m_gameUIContent      = EditorGUIUtility.IconContent("d_GameObject Icon");
        m_gameUIContent.text = "GameObject";
        m_lockIconName       = "LockIcon-On";
    }

    void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    void Init()
    {
        m_itemList = new List<ViewBinder.Item>();
        m_comList = new List<Component>();
        m_lockBtnName = "item组件列表";
        m_componentStr = string.Empty;
        m_lock = false;
        if (Selection.activeGameObject.GetComponent<ViewBinder>())
        {
            m_prefabObjBinderObj = Selection.activeGameObject;
            OnRefreshBtnClicked();
        }
    }

    const int interval = 5;
    const int label= 100;


    int ControlTop => Screen.height / 2;
    float ControlWidth = Screen.width / 2;



    private bool isDragging = false; // 是否正在拖动

    void OnGUI()
    {
        // 绘制可拖动的分割条
        Rect dragRect = new Rect(ControlWidth - 5, 0, 10, Screen.height);
        EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.ResizeHorizontal);
        HandleDrag(dragRect);

        BeginBox(new Rect(0, 0, ControlWidth, Screen.height));
        DrawPrefabObjBinderField();
        DrawSearchBtn();
        DrawSearchItemList();
        EndBox();

        BeginLine(new Rect(ControlWidth, 0, interval, Screen.height));
        EndBox();

        BeginBox(new Rect(ControlWidth + interval, 0, Screen.width - ControlWidth - interval, ControlTop));
        DrawLockBtn();
        GUILayout.Space(2);
        
        DrawComponentList();
        EndBox();

        BeginBox(new Rect(ControlWidth + interval, ControlTop , Screen.width - ControlWidth - interval, ControlTop ));
        GUILayout.Space(2);
        DrawItemField();
        EndBox();
    }

    void HandleDrag(Rect dragRect)
    {
        Event currentEvent = Event.current;

        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                if (dragRect.Contains(currentEvent.mousePosition))
                {
                    isDragging = true;
                    currentEvent.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    ControlWidth += currentEvent.delta.x;
                    ControlWidth = Mathf.Clamp(ControlWidth, 100, Screen.width - 100); // 限制最小宽度和最大宽度
                    currentEvent.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDragging)
                {
                    isDragging = false;
                    currentEvent.Use();
                }
                break;
        }
    }


    private void DrawSearchBtn()
    {
        GUILayout.BeginHorizontal();
        string before = m_itemNameSearch;
        string after = EditorGUILayout.TextField("", before, "SearchTextField");
        if (before != after) m_itemNameSearch = after;

        if (GUILayout.Button("", "SearchCancelButton"))
        {
            m_itemNameSearch = "";
            GUIUtility.keyboardControl = 0;
        }
        ComponentOperation(m_binder, ItemOption.SearchItems, after);
        GUILayout.EndHorizontal();
    }

    private void DrawPrefabObjBinderField()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("预设体", GUILayout.Width(80));
        var oldObj = m_prefabObjBinderObj;
        m_prefabObjBinderObj = EditorGUILayout.ObjectField(m_prefabObjBinderObj, typeof(GameObject), true) as GameObject;

        EditorGUILayout.EndHorizontal();


        if (!m_prefabObjBinderObj)
        {
            EditorGUILayout.HelpBox("Select a PrefabObjBinder Object", MessageType.Warning);
        }
        else if (oldObj != m_prefabObjBinderObj)
        {
            m_binder = m_prefabObjBinderObj.GetComponent<ViewBinder>();
        }

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
      
        if (m_binder)
        {
            if (string.IsNullOrEmpty(m_binder.classType))
            {
                
                // 定义一个GUIStyle来显示红色感叹号
                GUIStyle warningStyle = new GUIStyle(EditorStyles.label);
                warningStyle.normal.textColor = Color.red;
                warningStyle.fontStyle = FontStyle.Bold;
                // 使用GUILayout.Label显示红色感叹号
                GUILayout.Label("Class Type", warningStyle);
                // 获取红色感叹号图标
                GUIContent errorIcon = EditorGUIUtility.IconContent("console.erroricon");
                GUILayout.Label(errorIcon, GUILayout.Width(18), GUILayout.Height(18)); // 控制图标的大小
            }
            else
            {
                GUILayout.Label("ClassType", GUILayout.Width(80));
            }
            m_binder.classType = EditorGUILayout.TextField(m_binder.classType);
        }
        EditorGUILayout.EndHorizontal();
    }

    


    private void BeginBox(Rect rect)
    {
        rect.height -= 2;
        GUILayout.BeginArea(rect);
        GUILayout.Box("", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
        GUILayout.EndArea();
        GUILayout.BeginArea(rect);
    }

    private void BeginLine(Rect rect)
    {
        rect.height -= 2;
        GUILayout.BeginArea(rect);
        GUILayout.Label("", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
        GUILayout.EndArea();
        GUILayout.BeginArea(rect);
    }


    private void EndBox()
    {
        GUILayout.EndArea();
    }

    private void DrawSearchItemList()
    {
        if (null == m_prefabObjBinderObj || null == m_binder)
            m_searchMatchItemList.Clear();
        m_scrollViewPos = EditorGUILayout.BeginScrollView(m_scrollViewPos);
        foreach (var item in m_searchMatchItemList)
        {
            GUILayout.BeginHorizontal();

            if (m_selectedInfo.m_selected == item)
            {
                EditorGUI.DrawRect(m_selectedInfo.box, new Color(0.5f, 0.5f, 1f, 0.8f)); // 淡蓝色
            }

            EditorGUILayout.LabelField("  ", GUILayout.Width(20));

            item.name = EditorGUILayout.TextField(item.name);
            item.main = EditorGUILayout.ObjectField(item.main, typeof(GameObject), true);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                m_itemList.Remove(item);
                m_binder.items = m_itemList.ToArray();
                GUILayout.EndHorizontal();
                break;
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            Rect labelRect = GUILayoutUtility.GetLastRect();
            // 检测鼠标事件
            Event currentEvent = Event.current;

            if (labelRect.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.MouseDown)
            {
                labelRect.height += 2;
                labelRect.y      -= 1;
                m_selectedInfo.box = labelRect ;
                m_selectedInfo.m_selected  = item;

                if (item.main is GameObject go)
                {
                    Selection.activeObject = go;
                    m_componentStr = go.GetType().ToString();
                }
                else if (item.main is Component component)
                {
                    Selection.activeObject = component.gameObject;
                    m_componentStr = component.GetType().ToString();
                }

                m_itemName = item.name;
                m_itemObj  = item.main;
                var icon = AssetPreview.GetMiniTypeThumbnail(item.main.GetType());
                var conntent = new GUIContent(" " + m_componentStr, icon);
                m_selectContent = conntent;
                currentEvent.Use(); 
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawItemField()
    {
        EditorGUILayout.BeginVertical();

    
        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("选择组件",EditorStyles.helpBox, GUILayout.MaxWidth(label));
        GUI.enabled = false;
        if (string.IsNullOrEmpty(m_componentStr))
        {
            GUILayout.Label("null");
        }
        else
        {
            GUILayout.Button(m_selectContent,GUILayout.MaxHeight(18));
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        m_itemName = EditorGUILayout.TextField(m_itemName);

        ItemTip();

        if (GUILayout.Button(new GUIContent("Add Item", "添加item"), GUILayout.Height(80)))
        {
            ComponentOperation(m_binder, ItemOption.AddItem);
        }
        if (GUILayout.Button(new GUIContent("Replace Item", "替换指定的item")))
        {
            if (m_prefabObjBinderObj != null)
            {
                if (string.IsNullOrEmpty(m_itemName))
                    Debug.LogWarning("请输入要删除的项目名称");
                else
                    ComponentOperation(m_binder, ItemOption.ReplaceItem);
            }
        }

        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Save", "保存刷新")))
        {
            var  binder = m_prefabObjBinderObj.GetComponent<ViewBinder>();

            // 使用Undo记录物体位置属性的更改
             Undo.RecordObject(m_prefabObjBinderObj.transform, "Change Object");
            // 更改物体位置属性
            m_prefabObjBinderObj.transform.position += new Vector3(0,0,0.00001f);

            OnRefreshBtnClicked();
        }
    }

    private void OnRefreshBtnClicked()
    {
        if (null != m_prefabObjBinderObj)
            m_binder = m_prefabObjBinderObj.GetComponent<ViewBinder>();



        if (null == m_binder)
        {
            m_itemList.Clear();
            m_comList.Clear();
        }
    }

    private void DrawLockBtn()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Select Go");
        var go =  EditorGUILayout.ObjectField(Selection.activeObject, typeof(GameObject), true) as GameObject;

        var icon = EditorGUIUtility.IconContent(m_lockIconName);
        icon.tooltip = m_lockBtnName;
        icon.text    = m_lockBtnName;

        if (GUILayout.Button(icon, EditorStyles.toolbarButton))
        {
            m_lock = !m_lock;
            if (m_lock == false)
            {
                m_lockBtnName  = "item组件列表";
                m_lockIconName = "LockIcon";
            }
            else
            {
                m_lockBtnName  = "item组件列表";
                m_lockIconName = "LockIcon-On";
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawComponentList()
    {
        var go = Selection.activeObject as GameObject; //获取选中对象
        if (go && m_lock == false)
        {
            Component[] components = go.GetComponents<Component>();
            m_comList.Clear();
            m_comList.AddRange(components);
            m_selectedItemName = go.name;
        }

        if (go == null)
        {
            m_comList.Clear();
            m_selectedItemName = "无选中对象";
        }

        m_scrollComponent = GUILayout.BeginScrollView(m_scrollComponent);

        if (go && GUILayout.Button(m_gameUIContent, "GameObject" == m_componentStr ? m_labelSytleYellow : m_labelStyleNormal))
        {
            m_itemObj       = go;
            m_componentStr  = "GameObject";
            m_selectContent = m_gameUIContent;
        }


        foreach (var com in m_comList)
        {
            var comType = com.GetType().ToString();
            comType = comType.Replace("UnityEngine.UI.", "");
            comType = comType.Replace("UnityEngine.", "");
     
           var icon      = AssetPreview.GetMiniTypeThumbnail(com.GetType());
           var conntent  = new GUIContent(" " + comType, icon);
            if (icon == null)
            {
                icon = (Texture2D)EditorGUIUtility.IconContent("d_cs Script Icon").image;
                conntent.image = icon;
            }

            if (GUILayout.Button(conntent, comType == m_componentStr ? m_labelSytleYellow : m_labelStyleNormal))
            {
                m_itemObj       = com;
                m_componentStr  = comType;
                m_selectContent = conntent;
            }
        }
        GUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    #region private method
    private void ComponentOperation(ViewBinder binder, ItemOption option, string name = " ")
    {
        if (null == binder) return;
        ViewBinder.Item item = new ViewBinder.Item();
        switch (option)
        {
            case ItemOption.AddItem:
                AddItem(item, binder);
                break;

            case ItemOption.ReplaceItem:
                ReplaceItem(item, binder);
                break;

            case ItemOption.ClearItems:
                ClearItem(item, binder);
                break;

            case ItemOption.SearchItems:
                SearchItem(item, binder, name);
                break;
        }
        binder.items = m_itemList.ToArray();
        // 这样enabled一下，才能触发预设的Override
        binder.enabled = false;
        binder.enabled = true;



    }

    private void AddItem(ViewBinder.Item item, ViewBinder binder)
    {
        item.name = m_itemName;
        item.main = m_itemObj;
        m_itemList = binder.items.ToList();
        List<string> nameList = new List<string>();
        foreach (var obj in m_itemList)
        {
            nameList.Add(obj.name);
        }
        if (!string.IsNullOrEmpty(m_itemName) && m_itemObj != null)
        {
            if (nameList.Contains(m_itemName))
            {
                Debug.LogError("重复元素");
                m_itemList.Add(item);
            }
            else
                m_itemList.Add(item);
        }
 
    }

    private void RemoveItem(ViewBinder.Item item, ViewBinder Ps)
    {
        item.name = m_itemName;

        m_itemList = Ps.items.ToList();
        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (m_itemList[i].name.ToLower() == item.name.ToLower())
            {
                m_itemList.Remove(m_itemList[i]);
                break;
            }
        }
    }

    private void ReplaceItem(ViewBinder.Item item, ViewBinder Ps)
    {
        item.name = m_itemName;
        item.main = m_itemObj;
        
        m_itemList = Ps.items.ToList();
        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (m_itemList[i].name.ToLower() == item.name.ToLower())
            {
                m_itemList[i] = item;
                break;
            }
        }
    }


    private void ClearItem(ViewBinder.Item item, ViewBinder Ps)
    {
        item.name = m_itemName;
        item.main = m_itemObj;
        m_itemList = Ps.items.ToList();

        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (m_itemList[i].main == null || string.IsNullOrEmpty(m_itemList[i].name))
            {
                m_itemList.Remove(m_itemList[i]);
            }
        }
    }

    private void SearchItem(ViewBinder.Item item, ViewBinder binder, string name)
    {
        m_itemList = binder.items.ToList();
        m_searchMatchItemList.Clear();

        foreach (var o in m_itemList)
        {
            if (string.IsNullOrEmpty(name))
            {
                m_searchMatchItemList.Add(o);
            }
            else
            {
                if (o.name.ToLower().Contains(name.ToLower()))
                {
                    m_searchMatchItemList.Add(o);
                }
                else if (null != o.main)
                {
                    var objName = o.main.name;
                    if (objName.ToLower().Contains(name.ToLower()))
                    {
                        m_searchMatchItemList.Add(o);
                    }
                }
            }
        }
    }

    private void ItemTip()
    {
        if (string.IsNullOrEmpty(m_itemName) || m_itemObj == null)
        {
            string msg = string.Empty;
            if (m_itemObj == null)
            {
                msg = "请选择项目组件";
            }
            else if (string.IsNullOrEmpty(m_itemName))
            {
                msg = "请输入要添加的项的名字";
            }

            EditorGUILayout.HelpBox(msg, MessageType.Warning);
            EditorGUILayout.Space();
        }
    }

    #endregion
}
