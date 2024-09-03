//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2020-09-31 18:00:47
//=======================================================
using System.Collections.Generic;
using UnityEngine;

namespace Sailfish
{
    /// <summary>
    /// 提示函数（循环时间提示函数）
    /// </summary>
    public class PromptMethod
    {
        bool p_IsInit = false;
        bool p_IsWhile = false;
        bool p_IsRuning = false;

        IPrompt current;
        List<IPrompt> prompts = new List<IPrompt>();



        public bool IsEmpty()
        {
            return prompts.Count <= 0;
        }

        public void Play()
        {
            if (!p_IsRuning)
            {
                p_IsRuning = true;
                current = prompts[0];

                for (int i = 0; i < prompts.Count; i++)
                {
                    prompts[i].Replace();
                }

                if (!p_IsInit)
                {
                    p_IsInit = true;
                    for (int i = 0; i < prompts.Count - 1; i++)
                    {
                        prompts[i].SetNext(prompts[i + 1]);
                    }
                }

                CallUnit.updateCall.AddListener(Update);
            }
        }

        public void Clear()
        {
            p_IsInit = false;
            p_IsRuning = false;
            current = null;
            prompts.Clear();
            Debug.Log("清理");

            CallUnit.updateCall.RemoveListener(Update);
        }

        public void Stop()
        {
            p_IsRuning = false;
            current = null;
            CallUnit.updateCall.RemoveListener(Update);
        }

        public void AddEvent(float m_Timer, System.Action m_Action)
        {
            if (p_IsWhile)
            {
                if (current == null)
                {
                    current = new PromptWhile();
                }
                current.InitPrompt(m_Timer, m_Action);
                if (!prompts.Contains(current)) prompts.Add(current);
            }
            else
            {
                current = new PromptData();
                current.InitPrompt(m_Timer, m_Action);
                prompts.Add(current);
           
            }
        }

        public void Wait()
        {
            current = new PromptWait();
            current.InitPrompt();
            prompts.Add(current);
        }

        public void Wait(System.Action action)
        {
            current = new PromptWait();
            current.InitPrompt(action);
            prompts.Add(current);
        }


        public void While(bool isWhile)
        {
            p_IsWhile = isWhile;
            current = null;
        }


        public void Next()
        {
            current.Skip();
        }


        public void Update()
        {
            if (p_IsRuning == false)
            {
                return;
            }

            if (current == null)
            {
                p_IsRuning = false;
                return;
            }

            current.Update();
            if (current.IsDone)
            {
                current.TriggerFunc();
                current = current.GetNext();
            }
        }



        public interface IPrompt
        {
            bool IsDone { get; set; }

            IPrompt GetNext();

            void Skip();
            void Update();
            void Replace();
            void TriggerFunc();
            void SetNext(IPrompt prompt);
            void InitPrompt(params object[] vs);
        }


        public class Prompt : IPrompt
        {
            public float timer;
            public virtual void InitPrompt(params object[] vs)
            {

            }


            protected bool m_isDone = false;
            public bool IsDone
            {
                get { return m_isDone; }
                set { m_isDone = value; }
            }


            IPrompt prompt;
            public virtual void SetNext(IPrompt prompt)
            {
                this.prompt = prompt;
            }

            public virtual IPrompt GetNext()
            {
                return prompt;
            }


            public virtual void Replace()
            {
                timer = 0;
                m_isDone = false;
            }

            public void Skip()
            {
                m_isDone = true;
            }

            public virtual void Update()
            {

            }

            public virtual void TriggerFunc()
            {
             
            }
        }


        public class PromptData : Prompt
        {

            public float m_Timer;
            public System.Action m_Action;


            public override void InitPrompt(params object[] vs)
            {
                this.m_Timer = (float)vs[0];
                this.m_Action = (System.Action)vs[1];
            }


            public override void Update()
            {
                timer += Time.deltaTime;
                if (m_Timer <= timer)
                {
                    timer = 0;
                    m_isDone = true;
                }
            }

            public override void TriggerFunc()
            {
                if (m_isDone)
                {
                    timer = 0;
                    m_isDone = false;
                    m_Action?.Invoke();
                }
            }
        }


        public class PromptWait : Prompt
        {
            System.Action action;

            public override void InitPrompt(params object[] vs)
            {
                if (vs != null)
                    action = (System.Action)vs[0];
            }

            public override void Update()
            {
                action?.Invoke();
            }
        }


        public class PromptWhile : Prompt
        {
            public List<IPrompt> prompts = new List<IPrompt>();


            int nextID;
            IPrompt current
            {
                get { return prompts[nextID]; }
            }


            public override void InitPrompt(params object[] vs)
            {
                IPrompt prompt = new PromptData();
                prompt.InitPrompt(vs);
                prompts.Add(prompt);
            }


            public override void Update()
            {
                current.Update();
                if (current.IsDone)
                {
                    current.TriggerFunc();
                    current.Replace();
                    nextID++;

                    if (nextID >= prompts.Count)
                    {
                        nextID = 0;
                        current.Replace();
                    }
                }
            }


            public override void Replace()
            {
                nextID = 0;
                base.Replace();
            }
        }
    }
}



