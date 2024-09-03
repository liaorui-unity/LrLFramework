//=======================================================
// 作者：LR
// 描述：工具人
// 创建时间：2021-10-21 11:18:45
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class PoolType : MonoBehaviour
    {
        object m_Lock = new object();

        public string _useKey;
        public string UseKey => _useKey;


        public GameObject m_prefab;
        public GameObject Prefab => m_prefab;
    

        public int preAmount = 8;
        public int maxCount = 15;


        /// <summary>
        /// 脚本添加对象池对象才需要执行
        /// </summary>
        /// <param name="m_Obj">预制体</param>
        /// <param name="key">加载key</param>
        /// <param name="pre">预加载数量</param>
        /// <param name="max">最高存在数量</param>
        public void CsharpInit(GameObject m_Obj, string key, int pre, int max)
        {
            m_prefab = m_Obj;
            _useKey  = key;
            preAmount = pre;
            maxCount = max;
        }



        [SerializeField] internal List<GameObject> useGos  = new List<GameObject>();
        [SerializeField] internal List<GameObject> idleGos = new List<GameObject>();

        CycleTime cycle = new CycleTime();


        void Start()
        {
            if (Prefab != null)
                Preload();
        }



        public void Preload()
        {
            for (int i = 0; i < preAmount; i++)
            {
                var go = CreateIdle();
                go.SetActive(false);
                idleGos.Add(go);
            }
        }

        public void Delete(GameObject oldGo)
        {
            lock (m_Lock)
            {
                if (this.gameObject != null)
                {
                    oldGo.SetActive(false);
                    oldGo.transform.SetParent(this.transform);

                    if (!idleGos.Contains(oldGo))
                    {
                        idleGos.Add(oldGo);
                        useGos.Remove(oldGo);
                    }
                }
            }
        }

        public void DeleteAll()
        {
            while (useGos.Count > 0)
            {
                Destroy(useGos[0]);
                useGos.RemoveAt(0);
            }
            while (idleGos.Count > 0)
            {
                Destroy(idleGos[0]);
                idleGos.RemoveAt(0);
            }
        }

        public GameObject Creat()
        {
            cycle.Replace();

            lock (m_Lock)
            {
                GameObject go = null;

                do
                {
                    if (idleGos.Count <= 0)
                    {
                        go = CreateIdle();
                    }
                    else
                    {
                        go = idleGos[0];
                        idleGos.RemoveAt(0);
                    }
                } 
                while (go == null);


                go.SetActive(true);
                useGos.Add(go);

                return go;
            }
        }


        private GameObject CreateIdle()
        {
            var go    = Instantiate(Prefab, this.transform) as GameObject;
            var ipool = go.transform.GetComponent<IPool>();
            ipool?.CreatInit();

            return go;
        }


        public void Check()
        {
            CheckInUesAndIdle();
        }

      
        void CheckInUesAndIdle()
        {
            if (cycle.Update())
            {
                if (idleGos.Count > 0 && idleGos.Count + useGos.Count > maxCount)
                {
                    Debug.Log("对象池超出池数量限制的对象加入删除队列：" + idleGos.Count);

                    while (idleGos.Count > 0 && idleGos.Count + useGos.Count > maxCount)
                    {
                        lock (m_Lock)
                        {
                            var tempGo = idleGos[0];
                            idleGos.RemoveAt(0);
                            Destroy(tempGo);
                        }
                    }
                }
            }
        }


       internal class CycleTime
        {
            float idleTime    = 0;
            float destroyTime = 5.0f;

            internal bool Update()
            {
                if (Time.time - idleTime > destroyTime)
                {
                    idleTime = Time.time;
                    return true;
                }
                return false;
            }

            internal void Replace()
            {
                idleTime = Time.time;
            }
        }
    }

}
