using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class OverrideScriptIcon
{
    static Texture2D onTs;
    static Texture2D OnTs => onTs = onTs ?? Resources.Load<Texture2D>("JsonStream/st_Icon");
    static OverrideScriptIcon()
    {
        if (OnTs != null)
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        // 获取文件的路径
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (path.EndsWith(".st"))
        {
            GUI.DrawTexture(new Rect(selectionRect.x+1, selectionRect.y-1, 18, 18), OnTs);
        }
    }

    private static void SetCustomIcon(string guid, Texture2D icon)
    {
        EditorGUIUtility.SetIconForObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(guid)), icon);
    }
}