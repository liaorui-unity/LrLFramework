//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class Debuger : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Init()
	{
		if (instance == null)
		{
			var go = new GameObject("Debuger");
            instance = go.AddComponent<Debuger>();
			DontDestroyOnLoad(go);
        }
	}

	private static Debuger instance;

	static string pref = $"{Application.productName}_Log";

	public static bool isLog  => PlayerPrefs.GetInt(pref) == 1;

	public static void Log(string str)
	{
		if (isLog)
        {
#if UNITY_EDITOR
            str = $"<color=yellow>[自主输出]  </color>{str}";
            Debug.Log(str);
#else
			Debug.Log(str);
#endif
		}
	}

	public static void LogWarning(string str)
	{
		Debug.LogWarning(str);
	}

	public static void LogError(string str)
	{
		Debug.LogError(str);
	}

	public static void LogGreen(string str)
	{
#if UNITY_EDITOR
        str = $"<color=green>[主动报错]  </color>{str}";
        Debug.Log(str);
#else
		Debug.LogError(str);
#endif
	}

	public static void LogException(System.Exception exception)
	{
		Debug.LogException(exception);
	}


    private void Update()
    {
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				if (PlayerPrefs.GetInt(pref) == 1)
				{
					Debug.Log("log功能 => 关闭");
                    PlayerPrefs.SetInt(pref, 0);
                }
                else if (PlayerPrefs.GetInt(pref) == 0)
                {
                    Debug.Log("log功能 => 打开");
                    PlayerPrefs.SetInt(pref, 1);
                }
            }
		}
    }
}

