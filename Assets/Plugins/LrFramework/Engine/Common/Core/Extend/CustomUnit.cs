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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;


public class CallUnit
{
    public static UnityEvent updateCall            = new UnityEvent();
    public static UnityEvent updateDontDestroyCall = new UnityEvent();

    public static UnityEvent lateUpdateCall = new UnityEvent();
    public static UnityEvent lateUpdateDontDestroyCall = new UnityEvent();

    public static UnityEvent fixedUpdateCall = new UnityEvent();
    public static UnityEvent fixedUpdateDontDestroyCall = new UnityEvent();

    public static UnityEvent destroyCall = new UnityEvent();

    public static UnityEvent GetCallEvent(CallType type)
    {
        switch (type)
        {
            case CallType.Update:
                return updateCall;
            case CallType.UpdateNotDestroy:
                return updateDontDestroyCall;
            case CallType.LateUpdate:
                return lateUpdateCall;
            case CallType.LateUpdateNotDestroy:
                return lateUpdateDontDestroyCall;
            case CallType.FixedUpdate:
                return fixedUpdateCall;
            case CallType.FixedUpdateeNotDestroy:
                return fixedUpdateDontDestroyCall;
            default:
                return updateCall;
        }
    }
}


public enum CallType
{
    Update,
    UpdateNotDestroy,
    LateUpdate,
    LateUpdateNotDestroy,
    FixedUpdate,
    FixedUpdateeNotDestroy,
}

	public class CustomUnit : MonoBehaviour 
	{
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (_instance == null)
            {
                Debug.Log("实例化：" + Instance);
                DontDestroyOnLoad(_instance.gameObject);
            }
        }


        private static CustomUnit _instance;

        public static string currentSceneName;

        public static CustomUnit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("CustomUnit").AddComponent<CustomUnit>();
                }
                return _instance;
            }
        }



        void Awake()
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.activeSceneChanged += SceneManager_SceneChange;
        }

        private void SceneManager_SceneChange(Scene arg0, Scene arg1)
        {
            Destroy();
            currentSceneName = arg1.name;
        }





        private void Update()
        {
            CallUnit.updateCall?.Invoke();
            CallUnit.updateDontDestroyCall?.Invoke();
        }

        private void LateUpdate()
        {
            CallUnit.lateUpdateCall?.Invoke();
            CallUnit.lateUpdateDontDestroyCall?.Invoke();
        }

        private void FixedUpdate()
        {
            CallUnit.fixedUpdateCall?.Invoke();
            CallUnit.fixedUpdateDontDestroyCall?.Invoke();
        }


        private void OnDestroy()
        {
            Destroy();
        }


        public static void Destroy(bool IsDont=false)
        {
        Debug.Log("  CallUnit.updateCall：" + CallUnit.updateCall.GetPersistentEventCount());

            CallUnit.destroyCall?.Invoke();

            CallUnit.updateCall = new UnityEvent() ;
            CallUnit.lateUpdateCall = new UnityEvent();
            CallUnit.fixedUpdateCall = new UnityEvent();
            CallUnit.destroyCall = new UnityEvent();              

            if (IsDont)
            {
                CallUnit.updateDontDestroyCall = new UnityEvent();
                CallUnit.lateUpdateDontDestroyCall = new UnityEvent();
                CallUnit.fixedUpdateDontDestroyCall = new UnityEvent();
            }
        }
    }

