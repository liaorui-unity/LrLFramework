using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DisplayShowWindow : EditorWindow
{
    private string message = "This is a custom dialog!";
    private bool showCancelButton = true;

    private System.Action<bool> action;

    private int time = 10;

    EditorDelay delay;

    GUIStyle whileGUIStyle = new GUIStyle();
    GUIStyle fontGUIStyle = new GUIStyle();
    public static DisplayShowWindow ShowWindow(Rect rect, string message, System.Action<bool> action)
    {
        DisplayShowWindow window = EditorWindow.GetWindowWithRect<DisplayShowWindow>(rect);
        window.titleContent = new GUIContent("Custom Dialog");
        window.minSize = new Vector2(300, 120);

        window.ShowUtility();
        window.message = message;
        window.action = action;

        return window;
    }

    void OnEnable()
    {
        whileGUIStyle.normal.background = Texture2D.grayTexture;

        fontGUIStyle.normal.textColor = Color.white;
        fontGUIStyle.fontSize = 16;
        fontGUIStyle.fontStyle = FontStyle.Bold;

        delay = new EditorDelay();

        delay.Start(time, () =>
           {
               Close();
               action?.Invoke(true);
               action = null;
           });
    }




    private void OnGUI()
    {
        GUI.Box(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)), "", whileGUIStyle);

        GUILayout.BeginVertical();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(message, fontGUIStyle);
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();


        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal("box");

        GUILayout.FlexibleSpace();



        if (GUILayout.Button($"OK({time -(int)delay.runTime})", GUILayout.Width(50)))
        {
            // 处理确认逻辑
            Debug.Log("OK button clicked.");
            action?.Invoke(true);
            action = null;
            Close();
        }


        GUILayout.FlexibleSpace();

        if (showCancelButton && GUILayout.Button("Cancel", GUILayout.Width(50)))
        {
            // 处理取消逻辑
            Debug.Log("Cancel button clicked.");
            action?.Invoke(false);
            action = null;
            Close();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        

        Repaint();
    }
}
