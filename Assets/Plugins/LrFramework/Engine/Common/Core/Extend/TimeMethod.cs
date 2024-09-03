//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2021-06-23 16:40:27
//=======================================================
using Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


    public static partial class Extend
    {
        public static TimeExtend UpdateCall<T>(this T target)
        {
            int id = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            TimeMgr.AddTimer(id, timeClass);

            timeClass.Init(target.GetHashCode(), 0.001f);
            return timeClass;
        }

        public static TimeExtend UpdateUnilt<T>(this T target)
        {
            int code = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            timeClass.Init(target.GetHashCode(), int.MaxValue);
            TimeMgr.AddTimer(code, timeClass);
            return timeClass;
        }

        public static TimeExtend UpdateUntil<T>(this T target, float delay)
        {
            int code = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            TimeMgr.AddTimer(code, timeClass);

            timeClass.Init(target.GetHashCode(), delay);
            return timeClass;
        }

        public static TimeExtend UpdateTime<T>(this T target, float delay, CallType callUpdate = CallType.Update)
        {
            int code = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            TimeMgr.AddTimer(code, timeClass);

            timeClass.Init(target.GetHashCode(), delay, callUpdate);
            return timeClass;
        }


        public static void StopThisOfTime<T>(this T target)
        {
            TimeMgr.Stop(target.GetHashCode());
        }

        public static void Stop(this TimeExtend target)
        {
            target.Recycle();
        }
    }



    public class TimeMgr
    {
        public static Dictionary<int, List<TimeExtend>> timerValues = new Dictionary<int, List<TimeExtend>>(); //记录所有的 ID 对应 时间延时脚本的数据类 

        public static void AddTimer(int typeID, TimeExtend extend)
        {
            if (timerValues.TryGetValue(typeID, out List<TimeExtend> timers))
            {
                timers.Add(extend);
            }
            else
            {
                timers = new List<TimeExtend>() { extend };
                timerValues.Add(typeID, timers);
            }
        }

        public static void ClearTimer(int code, TimeExtend extend)
        {
            if (timerValues.TryGetValue(code, out List<TimeExtend> timers))
            {
                timers.Remove(extend);
            }
        }

        public static void Stop(int code)
        {
            if (timerValues.TryGetValue(code, out List<TimeExtend> timers))
            {
                foreach (var item in timers)
                {
                    item.Recycle();
                }

                timers.Clear();
                timerValues.Remove(code);
            }
        }
    }
    /// <summary>
    /// 时间延迟类
    /// </summary>
    public class TimeExtend
    {
        private System.Func<float, bool> actionProcess;
        private System.Action<float> actionTime;
        private System.Action actionDone;

        private UnityEvent unityEvent;

        int hashCode = 0;

        float playTime;
        float delayTime;


        public void Init(int code, float delay, CallType type = CallType.Update)
        {
            playTime = 0;
            hashCode = code;
            delayTime = delay;

            unityEvent = CallUnit.GetCallEvent(type);
            unityEvent.AddListener(Update);

            if (type == CallType.Update
             || type == CallType.LateUpdate
             || type == CallType.FixedUpdate)
            {
                CallUnit.destroyCall.AddListener(Recycle);
            }
        }


        public void Update()
        {
            try
            {
                //返回true时，强行结束
                if (actionProcess != null)
                {
                    if (actionProcess.Invoke(Mathf.InverseLerp(0, delayTime, playTime)))
                    {
                        playTime = delayTime;
                    }
                }

                if (actionTime != null)
                {
                    actionTime.Invoke(delayTime);
                }

                //运行时间到，结束
                if (playTime >= delayTime)
                {
                    actionDone?.Invoke();
                    Recycle();
                }
                playTime += Time.deltaTime;
            }
            catch (System.Exception e)
            {
                Recycle();
                Debug.LogError($"Timer报错：{e.Message}————清空事件");
            }
        }


        public void Recycle()
        {
            ClassPool.Release(this);
            TimeMgr.ClearTimer(hashCode, this);
            unityEvent.RemoveListener(Update);
            CallUnit.destroyCall.RemoveListener(Recycle);


            playTime = 0;
            unityEvent = null;
            actionDone = null;
            actionProcess = null;
        }




        #region 事件委托回调
        /// <summary>
        /// 时间延时完成后执行
        /// </summary>
        public TimeExtend OnComelete(System.Action action)
        {
            actionDone = action;
            return this;
        }

        /// <summary>
        /// 时间延时过程中执行
        /// </summary>
        public TimeExtend OnProcess(System.Action<float> action)
        {
            actionProcess = (time) =>
            {
                action?.Invoke(time);
                return false;
            };
            return this;
        }


        /// <summary>
        /// 时间延时过程中执行
        /// </summary>
        public TimeExtend OnRunTime(System.Action<float> action)
        {
            actionTime = action;
            return this;
        }


        /// <summary>
        /// 时间延时过程中执行
        /// </summary>
        public TimeExtend OnUnity(System.Func<float, bool> action)
        {
            actionProcess = action;
            return this;
        }
        #endregion
    }

