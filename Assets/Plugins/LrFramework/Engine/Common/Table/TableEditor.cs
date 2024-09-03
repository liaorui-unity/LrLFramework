using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using Table;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TableEditor : MonoBehaviour
{
    [System.Serializable]
    public class PrefabInfo
    {
        public GameObject button;
        public GameObject parent;

        public GameObject Create()
        {
            return Instantiate(button, parent.transform);
        }
    }

    [System.Serializable]
    public class Grip
    {
        public PrefabInfo rowInfo;
        public PrefabInfo gripInfo;

        public Dictionary<GameObject, Vector2> grips    = new Dictionary<GameObject, Vector2>();
        public Dictionary<Vector2, GameObject> rGrips   = new Dictionary<Vector2, GameObject>();
        public Dictionary<GameObject,CustomInfo> values = new Dictionary<GameObject, CustomInfo>();

        public UnityAction change;
        public UnityAction<GameObject,CustomInfo> contentCall;

        public void Replace() 
        {
            foreach (var item in rGrips.Values)
            {
                var input = item.GetComponentInChildren<Text>();
                input.text = string.Empty;
            }

            foreach (var item in values.Keys)
            {
                var input = item.GetComponentInChildren<Text>();
                input.text = string.Empty;
            }
            values.Clear();
        }

        public void InitFixed(Vector2 position,string infoStr)
        {
            var go     = rGrips[position];
            var input  = go.GetComponentInChildren<Text>();
            input.text = infoStr;
        }

        public void InitValues(Vector2 position, CustomInfo info)
        {
            var go     = rGrips[position];
            var input  = go.GetComponentInChildren<Text>();
            values[go] = info;

            input.text = info.GetValue().ToString();
        }


        public void CreateCell(int row,int column)
        {
            for (int i = 0; i < row; i++)
            {
                var parent = rowInfo.Create();
                this.gripInfo.parent = parent;
                parent.SetActive(true);

                for (int j = 0; j < column; j++)
                {
                    var go       = this.gripInfo.Create();
                    var position = new Vector2(i, j);

                    grips[go]    = position;
                    rGrips[position] = go;

                    go.SetActive(true);
                    var input = go.GetComponent<Button>();
                    input.onClick.AddListener(() => { OnSelect(go); });
                }
            }
        }

        // 添加事件触发器方法
        void AddEventTrigger(GameObject go, EventTriggerType eventTriggerType, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            var eventTrigger = go.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = go.AddComponent<EventTrigger>();
            }

            var entry = new EventTrigger.Entry { eventID = eventTriggerType };
            entry.callback.AddListener(action);
            eventTrigger.triggers.Add(entry);
        }


        public void CreateFixed(int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                var parent = rowInfo.Create();
                this.gripInfo.parent = parent;
                parent.SetActive(true);

                for (int j = 0; j < column; j++)
                {
                    var go = this.gripInfo.Create();
                    var position = new Vector2(i, j);

                    grips[go]        = position;
                    rGrips[position] = go;

                    go.SetActive(true);
                }
            }
        }

        private void OnSelect(GameObject go)
        {
            if (values.TryGetValue(go, out var field))
            {
                contentCall?.Invoke(go, field);
                change?.Invoke();
            }
        }

        private void OnEndEdit(GameObject go, string str)
        {
            if (values.TryGetValue(go, out var field))
            {
                change?.Invoke();
            }
        }
    }

   
    public class CustomInfo
    {
        object instance;
        FieldInfo field;

        public CustomInfo(FieldInfo info, object instance)
        {
            this.instance = instance;
            field = info;
        }

        public void SetValue(string str)
        {
            if (field.FieldType.IsArray || field.FieldType.IsGenericType)
            {
                // 检查字段是否是数组类型
                if (field.FieldType.IsArray)
                {
                    // 获取数组元素的类型
                    Type elementType = field.FieldType.GetElementType();

                    str = str.TrimStart('[').TrimEnd(']');
                    str = str.TrimStart('(').TrimEnd(')');

                    // 将字符串解析为数组元素
                    string[] stringValues = str.Split(',');
                    // 创建数组实例
                    Array array = Array.CreateInstance(elementType, stringValues.Length);

                    // 获取类型转换器
                    TypeConverter converter = TypeDescriptor.GetConverter(elementType);

                    // 将解析后的数组元素转换为字段的数组类型
                    for (int i = 0; i < stringValues.Length; i++)
                    {
                        try
                        {
                            object convertedValue = converter.ConvertFromString(stringValues[i].Trim());
                            array.SetValue(convertedValue, i);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Error converting value: " + stringValues[i] + " to type: " + elementType + ". Exception: " + ex.Message);
                            return;
                        }
                    }
                    // 将转换后的数组赋给对象的字段
                    field.SetValue(instance, array);

                }
            }
            else
            {
                field.SetValue(instance, Convert.ChangeType(str, field.FieldType));
            }
        }

        public string GetValue()
        {
            if (field.FieldType.IsArray|| field.FieldType.IsGenericType)
            { 
                // 获取字段的值
                Array array = (Array)field.GetValue(instance);

                // 将数组元素转换为字符串
                string[] stringValues = new string[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    stringValues[i] = array.GetValue(i).ToString();
                }
                // 拼接字符串表示形式
                return "[" + string.Join(",", stringValues) + "]";
            }
            return field.GetValue(instance).ToString();
        }
    }


    [System.Serializable]
    public class Title
    {
        public PrefabInfo info;
        public UnityAction<PropertyInfo> call;
        public List<Button[]> groups = new List<Button[]>();
        public Dictionary<Button, PropertyInfo> titles = new Dictionary<Button, PropertyInfo>();

        public int count = 5;

        int page = 0;

        public Button last;
        public Button next;

    

        public void Init()
        {
            var pros = TableManager.instance.runTimeType.GetProperties();

            foreach (var item in pros)
            {
                var go = info.Create();
                var button = go.GetComponent<Button>();
                titles[button] = item;

                button.onClick.AddListener(() => Click(button));

                go.SetActive(true);
                go.GetComponentInChildren<Text>().text = item.Name;
            }

            // 使用 LINQ 将数组分组
            groups = titles.Keys
                    .ToArray()
                    .Select((value, index) => new { value, index })
                    .GroupBy(x => x.index / count)
                    .Select(g => g.Select(x => x.value).ToArray())
                    .ToList();

            last.onClick.AddListener(LastTitle);
            next.onClick.AddListener(NextTitle);

            Switch();
        }

        public void Default()
        {
            if (groups.Count > 0)
            {
                Click(groups[0][0]);
            }
        }

        public void Click(Button main)
        {
            var property = titles[main];

            foreach (var item in titles.Keys)
            {
                item.interactable = !(item == main);
            }

            call?.Invoke(property);
        }


        public void NextTitle()
        {
            page += 1;
            page = Mathf.Clamp(page, 0, groups.Count);
            Switch();
        }

        public void LastTitle()
        {
            page -= 1;
            page = Mathf.Clamp(page, 0, groups.Count);
            Switch();
        }

        void Switch()
        {
            //显示page当前页
            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var value = i == page;

                for (int n = 0; n < group.Length; n++)
                {
                    group[n].gameObject.SetActive(value);
                }
            }
            Set();
        }

        void Set()
        {
            next.interactable = page < groups.Count-1;
            last.interactable = page > 0;
        }
    }

    [System.Serializable]
    public class Save
    {
        public Button exitBtn;
        public Button noBtn;
        public Button okBtn;

        public GameObject panel;

        public UnityAction<bool> selectCall;

        bool isDirty = false;

        public void Init()
        { 
            exitBtn.onClick.AddListener(Show);
        }

        public void Change()
        {
            isDirty = true;
        }


        public void Show()
        {
            if (isDirty)
            {
                panel.SetActive(true);
                okBtn.onClick.AddListener(() => Select(true));
                noBtn.onClick.AddListener(() => Select(false));
            }
            else
            {
                Select(false);
            }
        }

        void Select(bool isOk)
        {
            Hide();
            selectCall?.Invoke(isOk);
        }

        void Hide()
        {
            panel.SetActive(false);
        }
    }

    [System.Serializable]
    public class Content
    {
        public GameObject panel;

        public InputField input;
        public Button     apply;
        public Button     cancel;

        CustomInfo currentInfo;
        GameObject currentGo;

        
        public void Init()
        {
            cancel.onClick.AddListener(Apply);
            apply .onClick.AddListener(Apply);
            input .onEndEdit.AddListener((str) => OnEndEdit(str));
        }

        public void Input(GameObject go, CustomInfo info)
        {
            panel.SetActive(true);
            currentInfo = info;
            currentGo   = go;
            input.text  = info.GetValue();
            input.ActivateInputField();
        }

        void OnEndEdit(string str)
        {
            currentInfo.SetValue(str);

            var info = currentGo.GetComponentInChildren<InputField>();
            if (info != null)
            {
                info.text = str;
                return;
            }
            var txt = currentGo.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = str;
            }
        }

        public void Cancel()
        {
            Apply();
        }

        public void Apply()
        {
            if (panel.activeInHierarchy)
                panel.SetActive(false);
        }
    }


    public Grip  fixedGrip;
    public Grip  customGrip;
    public Title title;
    public Save  savePanel;
    public Content content;

    PropertyInfo titleInfo;



    private void Start()
    {
        title.Init();
        title.call = OnClick;

        content.Init();

        fixedGrip .CreateFixed(2, 25);
        customGrip.CreateCell(25, 25);

        customGrip.change = savePanel.Change;
        customGrip.contentCall = content.Input;

        savePanel .selectCall = SelectFunc;
        savePanel .Init();

        title.Default();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            content.Apply();
        }
    }


    public void OnClick(PropertyInfo title)
    {
        this.titleInfo = title;

        var value = title.GetValue(null);
        var table = title.PropertyType.GetField("table", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var array = title.PropertyType.GetField("array", BindingFlags.Public | BindingFlags.Instance| BindingFlags.NonPublic );

        var tableType   = table.FieldType.GetGenericArguments()[1];
        var tableFields = tableType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var properties  = tableType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        var list = (Array)array.GetValue(value);

        var column = tableFields.Length;
        var row    = list.Length;


        fixedGrip.Replace();
        customGrip.Replace();

        for (int j = 0; j < column; j++)
        {
            fixedGrip.InitFixed(new Vector2(1, j), properties[j].Name);
            fixedGrip.InitFixed(new Vector2(0, j), properties[j].PropertyType.Name);
        }
        

        for (int i = 0; i < row; i++)
        {
            var item = list.GetValue(i);
            for (int j = 0; j < column; j++)
            {
                var info = new CustomInfo(tableFields[j], item);
                customGrip.InitValues(new Vector2(i,j), info);
            }
        }
    }

    public void SelectFunc(bool isOk)
    {
        if (isOk)
        {
            TableManager.instance.SaveMethod(
                titleInfo.GetValue(null) ,
                titleInfo.PropertyType.GetGenericArguments());
        }

        TableManager.instance.Hide(false);
    }
}
