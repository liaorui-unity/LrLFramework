//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：#CreateTime#
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sailfish
{

    [AttributeUsage(AttributeTargets.Class)]
    public class Sample : Attribute
    {
        public Type   mScript;
        public string mPath;
    }


    public abstract class UISamplePanel: MonoBehaviour
    {
        internal UISampleMgr uIMgr;

        void Awake()
        {
            uIMgr      = UISampleMgr.AddPanel(this.GetType(), this);
            viewBinder = this.GetComponentInChildren<ViewBinder>(true);
        }



        public ViewBinder viewBinder;


        public abstract void Update();

       


        #region UI Panel与外部的事件传输
        public void InitMethod()
        {
            this.AddMethodFuncs();
        }

        public void RemoveMethod()
        {
            if (UICall.GetInfoCount(this.GetHashCode()) > 0)
            {
                this.RemoveMethodFuncs();
            }
        }
        #endregion


        void Start()
        {    //--Panel自带配置--
            InitMethod();
            //--Panel自带配置--

            OnStart();
        }

        private void OnDisable()
        {
            UnRegister();
        }

        private void OnEnable()
        {
            Register();
        }

        private void OnDestroy()
        {
            //--移除Panel自带配置-- 
            RemoveMethod();
            //--移除Panel自带配置--

            OnDestroyed();
        }

        public virtual void  OnStart() { }
        public virtual void  OnDestroyed() { }
        public abstract void Setup(params object[] vs);
        public abstract void Register();
        public abstract void UnRegister();
    }
}
