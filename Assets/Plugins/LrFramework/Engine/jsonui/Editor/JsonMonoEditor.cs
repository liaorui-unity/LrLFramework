using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(JsonMono),true)]
public class JsonMonoEditor : Editor
{

    JsonMono selfScript => (JsonMono)target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox($"继承MonoBehaviour的 ({target.GetType()}) Json配置表",MessageType.Info);

        base.OnInspectorGUI();
      
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存"))
        {
            selfScript.SaveJsonFile();
        }
        if (GUILayout.Button("加载"))
        {
            selfScript.LoadJsonFile();
        }
        EditorGUILayout.EndHorizontal();
    }
}



[CustomPropertyDrawer(typeof(JsonMono.JsonSetting))]
public class JosnSettingEditor: PropertyDrawer
{
    private SerializedProperty _boolValue;
    private SerializedProperty _nameValue;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_boolValue == null)
            _boolValue = property.FindPropertyRelative("isDefault");

        if (_nameValue == null)
            _nameValue = property.FindPropertyRelative("saveName");


        EditorGUICustom.DrawBoxTitleColor("JsonSetting",Color.cyan, (rect)=>
        {
            GUILayout.BeginHorizontal();
            _boolValue.boolValue = GUILayout.Toggle(_boolValue.boolValue, "是否使用默认Key", GUILayout.Width(150));
            GUI.enabled = !_boolValue.boolValue;
            _nameValue.stringValue = GUILayout.TextField(_nameValue.stringValue);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        });
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20;
    }
}

