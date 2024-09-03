using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ViewBinder), true)]
public class ViewBinderInspector : Editor
{
    bool isOver = false;
    public override void OnInspectorGUI()
    {
       

        GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;


     


        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Space(2);

        if (GUILayout.Button("viewBinder View"))
        {
            ViewBinderEditor.ShowWindow();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("generate Cs"))
        {
            if (target is ViewBinder viewBinder)
            {
                if (string.IsNullOrEmpty(viewBinder.classType))
                {
                    EditorUtility.DisplayDialog("提示", "classType is null", "OK");
                    Debug.LogError("classType is null");
                    return;
                }
                ViewBinderClassCreat.SaveViewBinderClass(this.target as ViewBinder);
                AssetDatabase.Refresh();
            }
        }
        GUILayout.Space(2);
        GUILayout.EndVertical();

    }
}
