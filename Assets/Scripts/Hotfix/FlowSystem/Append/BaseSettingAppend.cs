using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flow;

public class BaseSettingAppend :IFlowAppend
{
    public void Postfix()
    {
        Debug.Log("BaseSettingAppend Postfix");
        //设置屏幕分辨率
        Screen.SetResolution(1920, 1080, true);
        //设置帧率
        Application.targetFrameRate = 60;
        //设置屏幕不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //设置屏幕方向
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void Prefix()
    {
        Debug.Log("BaseSettingAppend Prefix");
    }
}
