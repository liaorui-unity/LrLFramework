using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sailfish;

public class UISampleView
{
    public ViewBinder    view;
    public UISamplePanel window;
    public void Set(bool action)
    {
        view.gameObject.SetActive(action);
    }

    public UISampleView(UISamplePanel window)
    {
        this.window = window;
        view = window.viewBinder;
        Logic();
    }


    public virtual void Logic()
    {

    }
}

