using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Animator))]
public class Test : MonoBehaviour
{
    public Animator animator;


    ViewBinder binder;


    public void Start()
    {
        binder = GetComponent<ViewBinder>();

        var go = binder.TryGet<Transform>(ViewID.Main.go);



        Debug.Log(go);


        var info = animator.AddStateCheck("Tr");

        info.OnStateEnter = () =>
        {
            Debug.Log("Enter");
        };
        info.OnStateProgress = (p =>
        {
           // Debug.Log(p);
        });
        Start1();
    }

    public Object targetChild; // 需要获取路径的子物体

    void Start1()
    {
        if (targetChild != null)
        {
            

          //  string path = Object2Path(targetChild, transform);
            // if (path != null)
            // {
            //     Debug.Log("Path: " + path);
            // }
            // else
            // {
            //     Debug.LogWarning("The target child is not a descendant of this object.");
            // }
        }
        else
        {
            Debug.LogWarning("Target child is not set.");
        }
    }


    string GetRelativePath(Transform child, Transform parent)
    {
        if (child == parent)
        {
            return ""; // 不包含父物体自身的名称
        }
        else if (child.parent == null)
        {
            return null; // 子物体不在目标父物体的层级之下
        }
        else
        {
            string parentPath = GetRelativePath(child.parent, parent);
            if (parentPath == null)
            {
                return null; // 子物体不在目标父物体的层级之下
            }
            return string.IsNullOrEmpty(parentPath) ? child.name : parentPath + "/" + child.name;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            animator.SetTrigger("Sh");
        }
    }

    void F()
    {
        Debug.Log("F");
    }

}
