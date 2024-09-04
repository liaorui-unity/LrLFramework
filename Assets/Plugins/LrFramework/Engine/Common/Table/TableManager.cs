using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using ProtoBuf;
using System;
using System.Linq;
using UnityEngine.Events;
using LogInfo;

namespace Table
{
    /// <summary>
    /// 配置表管理器
    /// @author hannibal
    /// @time 2019-6-13
    /// </summary>
    public class TableManager : Singleton<TableManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Info.Log("实例化："+TableManager.instance.ToString());
        }

        /// <summary>
        /// Resources目录下的路径
        /// </summary>
        public string TablePath  = "Table";
        public const string path = "Table/TablePanel";

        public Type runTimeType;
        private GameObject main;



        public void Show()
        {
            if (main == null)
            {
                main = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            }
            main.SetActive(true);
        }

        public void Hide(bool isDestroy=true)
        {
            main.SetActive(false);
            if (isDestroy)
            {
                GameObject.Destroy(main);
            }
        }

        public TableManager()
        {
            if (instance == null)
            {
            }
        }


        public void SaveMethod(object table, Type[] types) 
        {
            var method = this.GetType()
                       . GetMethod(nameof(SaveTable))
                       . MakeGenericMethod(types);

            method.Invoke(this, new object[] { table });
        }

        private void SaveTable<K, T>(Table<K, T> table) where T : Row<K>
        {
            string name = typeof(T).Name;
            string file_path = SaveTablePath(name);

            DataSrc<K, T> dataSrc = new DataSrc<K, T>();
            dataSrc.array = table.GetArray();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, dataSrc);
                    byte[] data = ms.ToArray();
                    FileUtils.WriteFileByte(file_path, data);
                    Debug.Log("配置表保存成功:" + file_path);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("配置表{0}保存失败:{1}", name, e.ToString()));
            }
        }

        public void LoadMethod<K, T>(Type type) where T : Row<K>
        {
            runTimeType = type;
            string name = typeof(T).Name;

            Info.Log("加载配置表:" + name);

            try
            {
                //加载
                string file_path = ComePath(name);
                byte[] ta = AssetLoader.LoadBytes(file_path);

                if (ta == null)
                {
                    Debug.LogError("配置表加载失败:" + file_path);
                    return;
                }

                PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                pi.SetValue(null, LoadTable<K, T>(ta), null);
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("配置表{0}读取失败:{1}", name, e.ToString()));
            }
        }

        public Table<K, T> LoadTable<K, T>(byte[] ta) where T : Row<K>
        {
            DataSrc<K, T> dataSrc = Load<K, T>(ta);
            if (dataSrc != null)
            {
                return new Table<K, T>(dataSrc);
            }
            return null;
        }
        private DataSrc<K, T> Load<K, T>(byte[] ta) where T : Row<K>
        {
            if (ta != null)
            {
                using (MemoryStream ms = new MemoryStream(ta))
                {
                    return Serializer.Deserialize<DataSrc<K, T>>(ms);
                }
            }
            return null;
        }

        private string SaveTablePath(string tableName)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var persistentPath = string.Format("{0}/{1}/{2}.bytes", Application.persistentDataPath, TablePath, tableName);
            return persistentPath;
#endif
            var path = string.Format("{0}/{1}/{2}.bytes", Application.streamingAssetsPath, TablePath, tableName);
            return path;
        }

        private string GetTablePath(string tableName)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var persistentPath = string.Format("{0}/{1}/{2}.bytes", Application.persistentDataPath, TablePath, tableName);
            if (File.Exists(persistentPath))
            { 
                return persistentPath;
            }
#endif
            var path = string.Format("{0}/{1}/{2}.bytes", Application.streamingAssetsPath, TablePath, tableName);
            return path;
        }


        private string ComePath(string tableName)
        {
            var path = string.Format("{0}/{1}.bytes",  TablePath, tableName);
            return path;
        }
    }
}
