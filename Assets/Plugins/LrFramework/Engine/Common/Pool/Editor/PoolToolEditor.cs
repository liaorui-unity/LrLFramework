
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Pool.Log;
namespace Pool
{
    [CustomEditor(typeof(PoolMgr))]
    public class PoolToolCustomEditor : Editor
    {
        PoolMgr pool
        {
            get
            {
                return target as PoolMgr;
            }
        }

        string[] show = new string[] { "ON", "OFF" };

        int lastSelect = -1;
        int select = 1;
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("自动功能", MessageType.Info);
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("创建对象池 Type ID", GUILayout.MaxWidth(120));

            if (GUILayout.Button("创建"))
            {
                if (!Application.isPlaying) SaveEnumID();
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);

            Repaint();
        }



        public void SaveEnumID()
        {
            string path = Application.dataPath + @"/Scripts/Auto";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            PoolType[] alls = pool.transform.GetComponentsInChildren<PoolType>(true);

            List<string> names = new List<string>();
            for (int i = 0; i < alls.Length; i++)
            {
                if (!string.IsNullOrEmpty(alls[i].UseKey))
                    names.Add(alls[i].UseKey);
            }

            if (names.Count > 0)
                CreateScript(path, "PoolID", names);

            AssetDatabase.Refresh();
        }

        void CreateScript(string folder, string name, List<string> names)
        {

            StringBuilder sb = new StringBuilder();


            //生成头部
            sb.AppendLine(string.Format(@"
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Sailfish
{{
    public partial class {0}
    {{", name));

            foreach (var item in names)
            {
                sb.AppendLine(string.Format(@"         public const string {0}=""{1}"";", item, item));
            }
            sb.AppendLine(@"    }
}");
            File.WriteAllText(string.Format("{0}/{1}.cs", folder, name), sb.ToString());
        }
    }
}
