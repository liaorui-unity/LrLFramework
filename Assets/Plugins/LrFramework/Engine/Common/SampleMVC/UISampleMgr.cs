//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：#CreateTime#
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using LogInfo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sailfish
{
    public class UISampleMgr : MonoBehaviour
    {
        static UISampleMgr instance;
        public bool isDontDestroy = false;

    

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }

            instance = this;

            if (isDontDestroy)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }


        #region 特性数据

        internal static Dictionary<System.Type, Sample> samples = new Dictionary<System.Type, Sample>();

        public void AddLoaderInfo(Sample info)
        {
            if (!samples.ContainsKey(info.mScript))
            {
                samples.Add(info.mScript, info);
            }
        }
        public Sample GetLoaderInfo(System.Type type)
        {
            Sample info = null;
            if (!samples.TryGetValue(type, out info))
            {//可能没有提取特性
                this.ExtractAttribute(type.Assembly);
                if (!samples.TryGetValue(type, out info))
                {
                    return null;
                }
            }
            return info;
        }
        public void RemoveAllLoaderInfo()
        {
            samples.Clear();
        }

        /// <summary>
        /// 提取特性
        /// </summary>
        private void ExtractAttribute(System.Reflection.Assembly assembly)
        {
            float start_time = Time.realtimeSinceStartup;
            //外部程序集
            List<System.Type> types = AttributeUtils.FindType<UISamplePanel>(assembly, true);


            if (types != null)
            {
                foreach (System.Type type in types)
                {
                    Sample ui_attr = AttributeUtils.GetClassAttribute<Sample>(type);
                    if (ui_attr == null) continue;
                    ui_attr.mScript = type;
                    this.AddLoaderInfo(ui_attr);
                }
            }
            Info.Log("UIManager:ExtractAttribute 提取特性用时:" + (Time.realtimeSinceStartup - start_time));
        }

        #endregion



        internal static Dictionary<System.Type, UISamplePanel> panels = new Dictionary<System.Type, UISamplePanel>();

        public static Func<string,GameObject> OverrideLoadPanelFunction;

        internal static UISampleMgr AddPanel(System.Type type, UISamplePanel sample)
        {
            panels[type] = sample;
            return instance;
        }
        
        public static void Show<T>(bool isOverride = true, params object[] vs) where T : UISamplePanel
        {
            Debug.Log(panels.ContainsKey(typeof(T)));
            if (!panels.TryGetValue(typeof(T), out UISamplePanel panel))
            {
                var sample = instance.GetLoaderInfo(typeof(T));
                GameObject samplePanel = null;

                if (OverrideLoadPanelFunction != null && isOverride)
                {
                    samplePanel = OverrideLoadPanelFunction.Invoke(sample.mPath);
                }
                else
                {
                    samplePanel = Instantiate(Resources.Load(sample.mPath)) as GameObject;
                }

                samplePanel.transform.SetParent(instance.transform);
                samplePanel.transform.localScale       = Vector3.one;
                samplePanel.transform.localEulerAngles = Vector3.zero;
                samplePanel.transform.localPosition    = Vector3.zero;


                panel = samplePanel.GetComponent<T>() ?? samplePanel.AddComponent<T>();
                panels[typeof(T)] = panel;
            }

            panel.Setup(vs);
            panel.gameObject.SetActive(true);
        }
        public static void ShowNotOverride<T>( params object[] vs) where T : UISamplePanel
        {
            if (!panels.TryGetValue(typeof(T), out UISamplePanel panel))
            {
                var sample = instance.GetLoaderInfo(typeof(T));

                var samplePanel = Instantiate(Resources.Load(sample.mPath)) as GameObject;
               
                samplePanel.transform.SetParent(instance.transform);
                samplePanel.transform.localScale       = Vector3.one;
                samplePanel.transform.localEulerAngles = Vector3.zero;
                samplePanel.transform.localPosition    = Vector3.zero;

                panel = samplePanel.GetComponent<T>() ?? samplePanel.AddComponent<T>();
                panels.Add(typeof(T), panel);
            }

            panel.Setup(vs);
            panel.gameObject.SetActive(true);
        }

        public static void Hide<T>()
        {
            panels[typeof(T)].gameObject.SetActive(false);
        }


        private void OnDestroy()
        {
            panels.Clear();
            samples.Clear();

            instance = null;
        }
    }
}
