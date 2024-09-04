using UnityEngine;
using System.Collections;
using UnityEditor;


public class StInspector : Editor, IMonoAsset
{
    static StInspector jsonInspector;
    static Color color;

    string m_jsonPath;

    SerializedProperty serializedProperty;

    StInstance stInstance; 

    GUIStyle font = new GUIStyle();

    [InitializeOnLoadMethod]
    static void JsonInit()
    {
        DefaultAssetInspector.OnSelectAsset.AddListener(OnSelectAsset);
    }

    public static void OnSelectAsset(UnityEngine.Object uobject, AssetAction call)
    {
        var m_AssetPath = AssetDatabase.GetAssetPath(uobject);
        if (m_AssetPath.EndsWith(".st"))
        {
            jsonInspector = (StInspector)CreateEditor(uobject, typeof(StInspector));
            call?.Invoke(uobject, jsonInspector as IMonoAsset);
        }

        ColorUtility.TryParseHtmlString("#5B5858", out color);
    }

    void OnEnable()
    {
        font = new GUIStyle();
        font.fontSize = 20;
        font.normal.textColor = Color.white;
        font.contentOffset = new Vector2(0, 20);
    }


    public  void OnNewHeaderGUI()
    {
       // if (serializedProperty == null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(OnTs, GUILayout.Width(60), GUILayout.Height(60));
            
            GUILayout.Label("[StreamableObject]", font);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
        }
    }



    public override void OnInspectorGUI()
    {
        if (serializedProperty == null)
        {
            m_jsonPath = AssetDatabase.GetAssetPath(target);
            if (m_jsonPath.EndsWith(".st"))
            {
                stInstance = new StInstance();
                stInstance . FindProperty(m_jsonPath);
                serializedProperty = stInstance.sdProperty;
            }
            return;
        }

        GUI.enabled = true;

        serializedProperty.serializedObject.Update();
        EditorGUICustom.BoxColor(color, (_) =>
        {
            if (serializedProperty != null)
            {
                serializedProperty.isExpanded = true;

                EditorGUILayout.PropertyField(serializedProperty);
            }
        });

     
        if (GUI.changed)
        {
            serializedProperty.serializedObject.ApplyModifiedProperties();
            stInstance.Save();
        }
    }




    public void OnDestroy()
    {
        stInstance?.Clear();
        jsonInspector = null;
    }

 

    static Texture2D onTs;
    static Texture2D OnTs => onTs = onTs ?? Resources.Load<Texture2D>("JsonStream/st");
}