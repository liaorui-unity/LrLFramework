using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pool
{
    [CustomEditor(typeof(PoolLoadType))]
    public class PoolLoadTypeEditor : Editor
    {

        private SerializedObject targetGo;
        private SerializedProperty loadType;

        AddressType address;
        AssetBundleType assetBundleType;

        void OnEnable()
        {
            targetGo = new SerializedObject(target);
            loadType = targetGo.FindProperty("type");

            address = new AddressType();
            address.Init(target);

            assetBundleType = new AssetBundleType();
            assetBundleType.Init(target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(loadType, new GUIContent("加载类型"));

            if (loadType.enumValueIndex == 0)
            {
                assetBundleType.GUI();
            }

            else if (loadType.enumValueIndex == 1)
            {
                address.GUI();
            }
            targetGo.ApplyModifiedProperties();
        }

        public class AddressType
        {
            SerializedObject targetGo;
            SerializedProperty asset;

            PoolLoadType target;

            string path;

            public void Init(Object target)
            {
                targetGo = new SerializedObject(target);
                asset = targetGo.FindProperty("assetReference");
                this.target = target as PoolLoadType;
            }

            public void GUI()
            {
                if (target.assetReference != null)
                {
                    path = AssetDatabase.GUIDToAssetPath(target.assetReference.AssetGUID);
                }

                EditorGUILayout.HelpBox("加载Addressable包的资源", MessageType.Info);

                EditorGUILayout.LabelField("Asset Reference (地址)", path);
                EditorGUILayout.PropertyField(asset, new GUIContent("Asset Reference (Prefab)"));
            }
        }

        public class AssetBundleType
        {
            PoolLoadType target;

            string[] abPaths;
            string[] abFiles;
            string[] abShowFiles;
            string[] abAllNames;

            Object select;
            Object lastObject;

            int selectAbId = 0;
            int selectLastAbId = -1;

            int selectFileId = 0;
            int selectLastFileId = -1;

            public void Init(Object target)
            {
                this.target = target as PoolLoadType;

                abAllNames = AssetDatabase.GetAllAssetBundleNames();
                selectAbId = abAllNames.ToList().FindIndex(_ => _ == this.target.assetbundle);

                InitOne();
            }

            void InitOne()
            {
                if (selectAbId >= 0)
                {
                    Refresh(abAllNames[selectAbId]);
                    selectFileId = abFiles.ToList().FindIndex(_ => _ == this.target.assetbundlePrefab);

                    if (selectFileId >= 0)
                    {
                        select = AssetDatabase.LoadAssetAtPath(abPaths[selectFileId], typeof(GameObject));
                    }
                }
            }

            public void GUI()
            {
                EditorGUILayout.HelpBox("加载Ab包的资源", MessageType.Info);

                selectAbId = EditorGUILayout.Popup("assetBundle 包(name)", selectAbId, abAllNames);

                if (abShowFiles == null)
                {
                    InitOne();
                    return;
                }
                selectFileId = EditorGUILayout.Popup("assetBundle 包(地址)", selectFileId, abShowFiles);
                select = EditorGUILayout.ObjectField("assetBundle 包(Prefab)", select, typeof(GameObject), false);

                if (selectAbId != selectLastAbId)
                {
                    selectLastAbId = selectAbId;
                    Refresh(abAllNames[selectAbId]);
                    this.target.assetbundle = abAllNames[selectAbId];
                }

                if (selectFileId != selectLastFileId)
                {
                    selectLastFileId = selectFileId;
                    this.target.assetbundlePrefab = Path.GetFileNameWithoutExtension(abShowFiles[selectFileId]);
                    lastObject = select = AssetDatabase.LoadAssetAtPath(abPaths[selectFileId], typeof(GameObject));
                }


                if (select != lastObject)
                {
                    lastObject = select;

                    var path = AssetDatabase.GetAssetPath(select);
                    var importer = AssetImporter.GetAtPath(path);
                    var name = Path.GetFileNameWithoutExtension(path);
                    var ab = (importer.assetBundleName + "." + importer.assetBundleVariant).TrimEnd('.');

                    Refresh(ab);

                    selectAbId = abAllNames.ToList().FindIndex(_ => _ == ab);
                    this.target.assetbundle = ab;
                    selectFileId = abFiles.ToList().FindIndex(_ => _ == name);
                    this.target.assetbundlePrefab = name;
                }
            }

            void Refresh(string ab)
            {
                abPaths = AssetDatabase.GetAssetPathsFromAssetBundle(ab).Where(_ => _.Contains(".prefab")).ToArray();
                abFiles = new string[abPaths.Length];
                abShowFiles = new string[abPaths.Length];
                for (int i = 0; i < abPaths.Length; i++)
                {
                    abFiles[i] = Path.GetFileNameWithoutExtension(abPaths[i]);
                    abShowFiles[i] = $"{abPaths[i].Replace('/', '\\')}";
                }
            }
        }
    }
}
