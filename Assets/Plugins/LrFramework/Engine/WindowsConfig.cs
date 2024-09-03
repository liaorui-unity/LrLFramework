//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2022-05-25 09:41:36
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


namespace Sailfish
{
    public class WindowsConfig : MonoBehaviour
    {


        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
       
        
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
      
        
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint=true);


        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(uint uFlags);


        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName,string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);





        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwd, int cmdShow);

        const uint SWP_SHOWWINDOW = 0x0040;

        const int GWL_STYLE = -16;
        const int WS_BORDER = 1;

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        //隐藏标题栏图标
        const int WS_POPUP = 0x800000;
        const int WS_SYSMENU = 0x80000;
  
        //最大最小化
        const int SW_SHOWMINIMIZED = 2;//(最小化窗口)
        const int SW_SHOWMAXIMIZED = 3;//最大化窗口
         
        //去除标题栏保留边框
        const int WS_CAPTION = 0x00C00000;
        const int WS_THICKFRAME = 0x00040000;



        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, int dwData);

        private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref Rect pRect, int dwData);


        public static void GetScreenRect()
        {

            int monCount = 0;

            MonitorEnumProc callback = (IntPtr hDesktop, IntPtr hdc, ref Rect prect, int d) =>
            {
                Debug.Log(prect.left+"_"+ prect.right);
                return ++monCount > 0;
            };
          

            if (!EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0))
                Console.WriteLine("An error occured while enumerating monitors");


          

            Debug.Log(monCount);
        }


        public enum SystemMetric
        {
            //其他类型查看：https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics
            SM_CMONITORS = 80,

        }

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);


        public static void GetScreenMode()
        {
            int screenNum = GetSystemMetrics(SystemMetric.SM_CMONITORS);
            Debug.Log(screenNum);
        }

        



        public static void SetWindow(IntPtr intPtr, bool isOn)
        {
            ShowWindow(intPtr, isOn? SW_SHOW: SW_HIDE);
        }

        public IntPtr GetWind(string key)
        {
            string exeName = key;
            var intPr = FindWindow(null, exeName);
            Debug.Log(intPr);
            return intPr;
        }

 




        public  void SetHideScreen()
        {
            var forWindow = GetWind("Unity Secondary Display");
            ShowWindow(forWindow, 0);
        }

    }
}
