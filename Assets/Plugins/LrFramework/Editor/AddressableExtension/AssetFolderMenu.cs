using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AddressableExtend
{
    [InitializeOnLoad]
    public class AssetFolderMenu : Editor
    {

        static AssetFolderMenu()
        {
            // 在Hierarchy视图中添加自定义菜单项
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            Event e = Event.current;

            if (e.type == EventType.ContextClick && selectionRect.Contains(e.mousePosition))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (AssetDatabase.IsValidFolder(path) && path.Contains(AssetConst.AA_Sign))
                {
                    MenuFolderEditor(path,guid);
                    e.Use();
                }

         
            }
        }

        static void MenuFolderEditor(string path,string guid)
        {
            if (path.Contains(AssetConst.AA_Sign)
                     && path.Contains("Asset/")
                     && path.Contains(AddressableBuildTools.LabelTag))
            {
                // 创建自定义菜单
                var menu = GetMenu(guid);
                menu.AddDisabledItem(new GUIContent("------------"));
                menu.AddItem(new GUIContent("删除资源Label"), false, MenuDelete, guid);
                menu.ShowAsContext();
            }
            else if (path.Contains(AssetConst.AA_Sign)
                       && path.Contains("Asset/")
                       && path.Contains(AddressableBuildTools.GroupTag))
            {
                // 创建自定义菜单
                var menu = GetMenu(guid);
                menu.AddDisabledItem(new GUIContent("------------"));
                menu.AddItem(new GUIContent("添加资源Label"), false, MenuLabelEditor, guid);
                menu.AddItem(new GUIContent("删除资源Group"), false, MenuDelete, guid);
                menu.ShowAsContext();
            }
            else if (path.Contains(AssetConst.AA_Sign) && path.Contains("Asset/"))
            {
                // 创建自定义菜单
                var menu = GetMenu(guid);
                menu.AddDisabledItem(new GUIContent("------------"));
                menu.AddItem(new GUIContent("添加资源Group"), false, MenuGroupEditor, guid);
                menu.ShowAsContext();

            }
            else if (path.EndsWith(AssetConst.AA_Sign) || path.Contains($"{AssetConst.Root_Sign}/"))
            {  // 创建自定义菜单
                var menu = GetMenu(guid);
                menu.ShowAsContext();
            }
        }


        internal static GenericMenu GetMenu(string guid)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Init"), false, MenuInit, guid);
            menu.AddItem(new GUIContent("Asset 资源适配(ID)"), false,   MenuAdaptID, guid);
            menu.AddItem(new GUIContent("Asset 资源适配(Info)"), false, MenuAdaptInfo, guid);
            menu.AddItem(new GUIContent("Asset 资源适配(All)"), false,  MenuAdaptAllInfo, guid);
            menu.AddItem(new GUIContent("Asset 重置增量资源"), false,    MenuResetInfo, guid);
        

            return menu;
        }


        internal static void MenuGroupEditor(object target)
        {
       
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {

                    if (path.Contains(AddressableBuildTools.GroupTag))
                    {
                        Debug.LogError($"创建失败,Assets组文件夹不能在含(Group)文件夹下创建，请在 {AddressableBuildTools.addressAssets}/x 的目录下创建");
                        break;
                    }

                    int count = 0;
                    string newFolder = string.Empty;

                    while (true)
                    {
                        newFolder = $"{path}/{AddressableBuildTools.GroupTag}{AssetConst.NewGroup}({count})";

                        if (!Directory.Exists(newFolder))
                        {
                            break;
                        }
                        count++;
                    }

                    var info =  GetAssetInfo(newFolder);
        
                    EditorUtility.SetDirty(info);
                    AssetDatabase.SaveAssetIfDirty(info);
                    AssetDatabase.Refresh();

                    break;
                }
            }
        }

        internal static void MenuLabelEditor(object target)
        {
          
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    int count = 1;
                    string newFolder = string.Empty;

                    while (true)
                    {
                        newFolder = $"{path}/{AddressableBuildTools.LabelTag}{AssetConst.NewLabel}({count})";

                        if (!Directory.Exists(newFolder))
                        {
                            break;
                        }
                        count++;
                    }

                    Directory.CreateDirectory(newFolder);

                    AssetDatabase.Refresh();

                    var info = GetAssetInfo(path);
                    CreatLabel(info, $"{AssetConst.NewLabel}({count})");

                    EditorUtility.SetDirty(info);
                    AssetDatabase.SaveAssetIfDirty(info);

                    break;
                }
            }
        }

        internal static void MenuInit(object target)
        {
            AssetProcessInfo.Init();
            AssetDatabase.Refresh();
        }

        internal static void MenuAdaptID(object target)
        {
            AssetProcessInfo.AutoAssetID();
        }

        internal static void MenuAdaptAllInfo(object target)
        {
            AssetProcessInfo.AutoAllAdaptGroupFolder();
        }

        internal static void MenuAdaptInfo(object target)
        {
            AssetProcessInfo.AutoAdaptGroupFolder();
        }



        internal static void MenuResetInfo(object target)
        {
            AssetProcessInfo.ReplaceUpdateContentEntry();
        }


        internal static void MenuDelete(object target)
        {
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path))
                {
                    DeleteAsset(path);
                }

                var origin = AssetDatabase.LoadAssetAtPath<Object>(path.TrimUnityPath());
                if (origin != null)
                {
                    AssetDatabase.DeleteAsset(path.TrimUnityPath());
                }
            }
        }


        internal static void RenameAsset(string oldPath, string newPath)
        {
            var isPass = IsAssetFolder(oldPath);
            var isPass2 = IsAssetFolder(newPath);

            if (isPass || isPass2)
            {
                string oldName = Path.GetFileName(oldPath);
                string newName = Path.GetFileName(newPath);

                if (oldName != newName)
                {
                    var oldPass = IsAssetFolder(oldPath);

                    if (oldPass)
                    {
                        var infoValue = AssetInfoSetting(oldPath);

                        var state = infoValue.Item1;
                        var info = infoValue.Item2;

                        if (info != null)
                        {
                            if (state == 1)
                            {
                                var full = $"{AssetDatabase.GetAssetPath(info)}";
                                if (File.Exists(full))
                                {
                                    AssetDatabase.RenameAsset(full, newName.ReplaceTag());
                                }

                                info.groupName = newName.ReplaceTag();
                            }
                            else if (state == 2)
                            {
                                var labelObject = AssetDatabase.LoadAssetAtPath<Object>(oldPath.TrimUnityPath());
                                var needRemove = info.labels.Find(_ => _.main == labelObject);

                                if (needRemove != null)
                                {
                                    needRemove.labelName = newName.ReplaceTag();
                                }
                            }
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        internal static void DeleteAsset(string path)
        {
            var isPass = IsAssetFolder(path);

            if (isPass)
            {
                var infoValue = AssetInfoSetting(path);
                var state = infoValue.Item1;
                var info = infoValue.Item2;

                if (info != null)
                {
                    if (state == 1)
                    {
                        var full = $"{AssetDatabase.GetAssetPath(info)}";

                        if (string.IsNullOrEmpty(full) == false)
                        {
                            AssetDatabase.DeleteAsset(full);
                            AssetDatabase.Refresh();
                        }
                    }
                    else if (state == 2)
                    {

                        var labelObject = AssetDatabase.LoadAssetAtPath<Object>(path.TrimUnityPath());
                        var needRemove = info.labels.Find(_ => _.main == labelObject);

                        if (needRemove != null)
                        {
                            info.labels.Remove(needRemove);
                        }
                    }
                }
            }

        

        }

        static bool IsAssetFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path) && path.Contains(AssetConst.AA_Asset))
            {
                return true;
            }
            return false;
        }

        static int IsGroupOrLabel(string path)
        {
            if (path.Contains(AddressableBuildTools.LabelTag))
            {
                return 2;
            }
            else if (path.Contains(AddressableBuildTools.GroupTag))
            {
                return 1;
            }
            return 0;
        }

        static (int, AssetInfoSetting) AssetInfoSetting(string path)
        {
            var state = IsGroupOrLabel(path);

            var groupPath = path;

            if (state == 1)
            {
                var folderName = Path.GetFileName(path).ReplaceTag();
                groupPath = $"{AssetConst.AA_Setting}/{AssetConst.Group_Sign}/{folderName}.asset";
            }
            else if (state == 2)
            {
                var baseLabel = new DirectoryInfo(path);
                var folderName = Path.GetFileName(baseLabel.Parent.FullName).ReplaceTag();
                groupPath = $"{AssetConst.AA_Setting}/{AssetConst.Group_Sign}/{folderName}.asset";
            }

            return (state, AssetDatabase.LoadAssetAtPath<AssetInfoSetting>(groupPath));
        }

        public static AssetInfoSetting GetAssetInfo(string folder)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                if (Directory.Exists(folder) == false)
                {
                    Directory.CreateDirectory(folder);
                }
            }

            AssetInfoSetting customGroup = null;

            var name = Path.GetFileNameWithoutExtension(folder);
            var customPath = $"{AssetConst.FD_Group}/{name}.asset".ReplaceTag();
            var savePath = customPath.TrimUnityPath().Replace(AddressableBuildTools.GroupTag, "");

            if (!File.Exists(customPath))
            {
                customGroup = ScriptableObject.CreateInstance<AssetInfoSetting>(); 
                AssetDatabase.Refresh();

                customGroup.main = AssetDatabase.LoadAssetAtPath<Object>(folder.TrimUnityPath());
                AssetDatabase.CreateAsset(customGroup, savePath);
            }
            else
            {
                customGroup = AssetDatabase.LoadAssetAtPath<AssetInfoSetting>(savePath);
            }
   
            customGroup.groupName = name.Replace(AddressableBuildTools.GroupTag, "");
            customGroup.addressType = folder.ReplaceSymbol().Contains("Asset/Remoted") ? AddressType.Remoted : AddressType.Local;

            EditorUtility.SetDirty(customGroup);

            return customGroup;

        }

        public static AssetInfoSetting.LabelInfo CreatLabel(AssetInfoSetting assetInfo, string label)
        {
            var findValue = assetInfo.labels?.Find(_ => _.labelName == label);

            if (findValue == null)
            {
                if (assetInfo.labels == null)
                    assetInfo.labels = new List<AssetInfoSetting.LabelInfo>();

                var mainPath = $"{AssetDatabase.GetAssetPath(assetInfo.main)}/{AddressableBuildTools.LabelTag}{label}";


                findValue = new AssetInfoSetting.LabelInfo
                {
                    labelName = label,
                    main = AssetDatabase.LoadAssetAtPath<Object>(mainPath)
                };

                if (label == "default")
                {
                    findValue.main = assetInfo.main;
                }

                assetInfo.labels.Add(findValue);
            }

            return findValue;
        }


    }

    public static class CustomExtend
    {
        public static string ReplaceSymbol(this string str)
        {
            return str.Replace('\\', '/');
        }

        public static string TrimSymbol(this string str)
        {
            return str.TrimStart('/').TrimEnd('/');
        }

        public static string TrimUnityPath(this string str)
        {
            return str.ReplaceSymbol()
                      .Replace(System.Environment.CurrentDirectory.ReplaceSymbol(), "")
                      .TrimSymbol();
        }

        public static string ReplaceTag(this string str)
        {
            return str.ReplaceSymbol()
                      .Replace(AddressableBuildTools.GroupTag, "")
                      .Replace(AddressableBuildTools.LabelTag, "");
        }
    }
}