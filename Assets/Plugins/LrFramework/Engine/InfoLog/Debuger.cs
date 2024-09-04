//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LogInfo;
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
#if UNITY_EDITOR
			Info.Init(System.Environment.CurrentDirectory);
#else
			Info.Init(Application.persistentDataPath);
#endif
        }
	}

	private static Debuger instance;

	static string pref = $"{Application.productName}_Log";
	public static bool isLog  => PlayerPrefs.GetInt(pref) == 1;


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

