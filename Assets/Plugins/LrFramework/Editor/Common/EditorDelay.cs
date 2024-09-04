using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorDelay
{
    public float time;

    public float runTime;
    public double startTime;
    public System.Action action;

    public void Start()
    {
        startTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += EditorUpdate;
    }

    public void Start(float time, System.Action action)
    {
        startTime = EditorApplication.timeSinceStartup;
        this.time = time;
        this.action = action;
        EditorApplication.update += EditorUpdate;
    }

    void EditorUpdate()
    {
        runTime = (float)(EditorApplication.timeSinceStartup - startTime);

        if (runTime > time)
        {
            action?.Invoke();
            EditorApplication.update -= EditorUpdate;
        }
    }
}