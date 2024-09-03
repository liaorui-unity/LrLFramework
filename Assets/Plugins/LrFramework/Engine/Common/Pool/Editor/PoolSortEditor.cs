using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pool
{
    [CustomEditor(typeof(PoolType))]
    public class PoolSortEditor : Editor
    {

        private SerializedObject targetObj;

        private SerializedProperty useKey;
        private SerializedProperty prefab;
        private SerializedProperty preAmount;
        private SerializedProperty maxCount;
        private SerializedProperty useObjects;
        private SerializedProperty idleObjects;


        void OnEnable()
        {
            targetObj = new SerializedObject(target);

            useKey = targetObj.FindProperty("_useKey");
            prefab = targetObj.FindProperty("m_prefab");
            preAmount   = targetObj.FindProperty("preAmount");
            maxCount    = targetObj.FindProperty("maxCount");
            useObjects  = targetObj.FindProperty("useGos");
            idleObjects = targetObj.FindProperty("idleGos");
        }

        PoolType poolSort
        {
            get
            {
                return target as PoolType;
            }
        }


        public override void OnInspectorGUI()
        {
            targetObj.Update();
            EditorGUILayout.Space();

            string line = string.Empty;
            int count = Screen.width / 6;
            for (int i = 0; i < count; i++)
            {
                line += "-";
            }



            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(useKey, new GUIContent("对象池的key"));


            var type = poolSort.GetComponent<PoolLoadType>();
            if (type == null)
                EditorGUILayout.PropertyField(prefab, new GUIContent("需要加载的预置体"));
            EditorGUILayout.Space();



            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(maxCount, new GUIContent("加载的最大数量"));
            preAmount.intValue = (int)EditorGUILayout.Slider(new GUIContent("加载的数量"),preAmount.intValue, 0.0f, (float)maxCount.intValue);

            EditorGUILayout.Space();



            EditorGUILayout.Space();
            GUILayout.Box("预览对象池对象");
            GUILayout.Label(line);
            EditorGUILayout.PropertyField(idleObjects, new GUIContent("等待中的预置体"),true);

  
            EditorGUILayout.PropertyField(useObjects, new GUIContent("使用中的预置体"),true);
            GUILayout.Label(line);
            EditorGUILayout.Space();
       

            EditorGUILayout.Space();
            targetObj.ApplyModifiedProperties();
        }
    }
}
