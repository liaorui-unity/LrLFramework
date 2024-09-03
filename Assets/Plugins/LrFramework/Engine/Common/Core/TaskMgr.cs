//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	public class TaskMgr :MonoSingleton<TaskMgr>
	{
        List<ItemTask> stackTasks = new List<ItemTask>();
        List<ItemTask> sortTasks = new List<ItemTask>();

        ItemTask sortRunTask;
        ItemTask stackRunTask;


        public void Update()
        {
            //----------队列任务--------------
            if (stackTasks.Count > 0)
            {
                if (!stackTasks[0].IsGaming)
                {
                    stackRunTask = stackTasks[0];
                    stackRunTask.Start();
                }
            }

            if (stackRunTask != null)
            {
                if (!stackRunTask.IsGaming)
                {
                    stackTasks.RemoveAt(0);
                    stackRunTask = null;
                }
            }
            //----------队列任务--------------





            //----------按layer排序任务--------------

            if (sortTasks.Count>0)
            {
                if (!sortTasks[0].IsGaming)
                {
                    sortRunTask = sortTasks[0];
                    sortRunTask.Start();
                }
            }

            if (sortRunTask != null)
            {
                if (!sortRunTask.IsGaming)
                {
                    sortTasks.RemoveAt(0);
                    sortRunTask = null;
                }
            }

            //----------按layer排序任务--------------
        }


        public void AddTask(ItemTask task)
        {
            stackTasks.Add(task);
        }


        public void AddSortTask(ItemTask task)
        {
            sortTasks.Add(task);

            if (sortTasks.Count > 1)
            {
                sortTasks.Sort((x, y) =>
                {
                    if (x.layer > y.layer) return -1;
                    return 0;
                });
            }
        }


        private void OnDestroy()
        {
            sortRunTask = null;
            stackRunTask = null;

            sortTasks.Clear();
            stackTasks.Clear();
        }
    }




    public interface ITask
    {
        void Init(params object[] vs);
        void Input();
        void Start();
        void End();
        void Msg(params object[] vs);
    }


    public class ItemTask: ITask
    {
        public int layer;

        bool isGaming = false;
        public bool IsGaming { get { return isGaming; } }
  

        public virtual void Init(params object[] vs)
        {
            if (vs != null)
            {
                this.layer = (int)vs[0];
                TaskMgr.instance.AddSortTask(this);
            }
            else
            {
                TaskMgr.instance.AddTask(this);
            }
        }



        public virtual void Input()
        {  
        }


        public virtual void Msg(params object[] vs)
        {
        }


        public virtual void Start()
        {
            isGaming = true;
        }

        public virtual void End()
        {
            isGaming = false;
        }
    }
}
