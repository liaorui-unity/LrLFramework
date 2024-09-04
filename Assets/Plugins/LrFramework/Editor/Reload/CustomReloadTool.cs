using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using System.Reflection;

[Overlay(typeof(SceneView), "Custom Reload","Reload", true)]
[Icon("Icons/Overlays/ToolSettings.png")]
public class CustomReloadTool : ToolbarOverlay
{
    public static System.Action updateCall;

    static string Name => $"{Application.productName}_Lock";
    public CustomReloadTool():base(new string[]
    { 
        "Reload",
        "Refresh"
    })
    {
        _isLock = PlayerPrefs.GetInt(Name, 0) == 1;
    }

    internal static bool _isLock = false;
    internal static bool  isLocked
    {
        get => _isLock;
        set
        {
            _isLock = value;
            PlayerPrefs.SetInt(Name, value ? 1 : 0);
        }
    }

    protected override Layout supportedLayouts => Layout.VerticalToolbar;


}

[EditorToolbarElement("Reload", typeof(SceneView))]
class ReloadItem : EditorToolbarButton
{
    GUIContent lockIcon   = new GUIContent(EditorGUIUtility.IconContent("LockIcon-On"));
    GUIContent unlockIcon = new GUIContent(EditorGUIUtility.IconContent("LockIcon"));

    bool isLocked
    { 
        get => CustomReloadTool.isLocked;
        set => CustomReloadTool.isLocked = value;
    }

    public ReloadItem()
    {
        clicked += OnClick;

        if (isLocked)
        {
            EditorApplication.LockReloadAssemblies();
        }

        ShowIcon();
    }

    private void OnClick()
    {
        isLocked = !isLocked;
        if (isLocked)
        {
            EditorApplication.LockReloadAssemblies();
        }
        else
        {
            EditorApplication.UnlockReloadAssemblies();
        }

        ShowIcon();
        CustomReloadTool.updateCall?.Invoke();
    }

    private void ShowIcon()
    {
        icon = (isLocked ? lockIcon.image : unlockIcon.image) as Texture2D;
    }
}


[EditorToolbarElement("Refresh", typeof(SceneView))]
class CustomItem : EditorToolbarButton
{
    GUIContent refresh => EditorGUIUtility.IconContent("d_Refresh");
    public CustomItem()
    {
        icon = refresh.image as Texture2D;
        clicked += OnClick;
        CustomReloadTool.updateCall = ShowElement;
        ShowElement();
    }

    private void OnClick()
    {
        EditorApplication.UnlockReloadAssemblies();
    }

    void ShowElement()
    {
        if (CustomReloadTool.isLocked)
        {
            style.display = DisplayStyle.Flex;
        }
        else
        {
            style.display = DisplayStyle.None;
        }
    }
}
