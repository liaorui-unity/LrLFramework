using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Table
{
    /// <summary>
    /// 查找资源丢失
    /// </summary>
    public class EditorFindMissingResources : EditorWindow
    {
        [MenuItem("Tools/查看资源丢失/地图", false, 100)]
        public static void FindTerrainMissing()
        {
          //  EditorUtils.ClearLog();

            GetWindow<EditorFindMissingResources>().titleContent = new GUIContent("查找Missing资源");
            GetWindow<EditorFindMissingResources>().Show();
            string[] allassetpaths = AssetDatabase.GetAllAssetPaths();
            var allres = allassetpaths
                .Where(a => a.IndexOf("res/Scene") >= 0);
            Find(allres);
        }
        [MenuItem("Tools/查看资源丢失/特效", false, 100)]
        public static void FindEffectMissing()
        {
           // EditorUtils.ClearLog();

            GetWindow<EditorFindMissingResources>().titleContent = new GUIContent("查找Missing资源");
            GetWindow<EditorFindMissingResources>().Show();
            string[] allassetpaths = AssetDatabase.GetAllAssetPaths();
            var allres = allassetpaths
                .Where(a => a.IndexOf("res/Prefabs/Effect") >= 0);
            Find(allres);
        }
        [MenuItem("Tools/查看资源丢失/角色模型、动作", false, 100)]
        public static void FindRoleMissing()
        {
           // EditorUtils.ClearLog();

            GetWindow<EditorFindMissingResources>().titleContent = new GUIContent("查找Missing资源");
            GetWindow<EditorFindMissingResources>().Show();
            string[] allassetpaths = AssetDatabase.GetAllAssetPaths();
            var allres = allassetpaths
                .Where(a => a.IndexOf("res/Prefabs/Char") >= 0);
            Find(allres);
        }
        [MenuItem("Tools/查看资源丢失/UI", false, 100)]
        public static void FindUIMissing()
        {
           // EditorUtils.ClearLog();

            GetWindow<EditorFindMissingResources>().titleContent = new GUIContent("查找Missing资源");
            GetWindow<EditorFindMissingResources>().Show();
            string[] allassetpaths = AssetDatabase.GetAllAssetPaths();
            var allres = allassetpaths
                .Where(a => a.IndexOf("Resources/UI") >= 0);
            Find(allres);
        }
        [MenuItem("Tools/查看资源丢失/所有", false, 100)]
        public static void FindAllMissing()
        {
          //  EditorUtils.ClearLog();

            GetWindow<EditorFindMissingResources>().titleContent = new GUIContent("查找Missing资源");
            GetWindow<EditorFindMissingResources>().Show();
            string[] allassetpaths = AssetDatabase.GetAllAssetPaths();
            Find(allassetpaths);
        }

        private static Dictionary<UnityEngine.Object, List<UnityEngine.Object>> prefabs = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();
        private static Dictionary<UnityEngine.Object, string> refPaths = new Dictionary<UnityEngine.Object, string>();
        private static void Find(IEnumerable<string> allassetpaths)
        {
            prefabs.Clear();
            var gos = allassetpaths
                 .Where(a => a.EndsWith("prefab"))//筛选 是以prefab为后缀的 预设体资源
                .Select(a => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(a));//加载这个预设体
                                                                                   //拿到的是所有加载好的预设体
            foreach (var item in gos)
            {
                GameObject go = item as GameObject;
                if (go)
                {
                    Component[] cps = go.GetComponentsInChildren<Component>(true);//获取这个物体身上所有的组件
                    foreach (var cp in cps)//遍历每一个组件
                    {
                        if (!cp)
                        {
                            if (!prefabs.ContainsKey(go))
                            {
                                prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                            }
                            else
                            {
                                prefabs[go].Add(cp);
                            }
                            continue;
                        }
                        SerializedObject so = new SerializedObject(cp);//生成一个组件对应的S俄日阿里则对Object对象 用于遍历这个组件的所有属性
                        var iter = so.GetIterator();//拿到迭代器
                        while (iter.NextVisible(true))//如果有下一个属性
                        {
                            //如果这个属性类型是引用类型的
                            if (iter.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                //引用对象是null 并且 引用ID不是0 说明丢失了引用
                                if (iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0)
                                {
                                    if (!refPaths.ContainsKey(cp)) refPaths.Add(cp, iter.propertyPath);
                                    else refPaths[cp] += " | " + iter.propertyPath;
                                    if (prefabs.ContainsKey(go))
                                    {
                                        if (!prefabs[go].Contains(cp)) prefabs[go].Add(cp);
                                    }
                                    else
                                    {
                                        prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EditorUtility.DisplayDialog("", "就绪", "OK");
        }
        //以下只是将查找结果显示
        private Vector3 scroll = Vector3.zero;
        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.BeginVertical();
            foreach (var item in prefabs)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, typeof(GameObject), true, GUILayout.Width(250));
                EditorGUILayout.BeginVertical();
                foreach (var cp in item.Value)
                {
                    if (cp)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(cp, cp.GetType(), true, GUILayout.Width(300));
                        if (refPaths.ContainsKey(cp))
                        {
                            GUILayout.Label(refPaths[cp]);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}