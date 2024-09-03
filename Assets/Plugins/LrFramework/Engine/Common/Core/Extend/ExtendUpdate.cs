using Table;
using Sailfish;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExtendUpdate : MonoBehaviour
{

    private static ExtendUpdate _instance;
    public static ExtendUpdate instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("ExtendUpdate").AddComponent<ExtendUpdate>();
                // _instance.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            }
            return _instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        if (_instance == null)
            Debug.Log("实例化：" + instance);
    }

    public static string currentSceneName = "";
    public static string lastSceneName = "";

    void Awake()
    {

        lastSceneName = SceneManager.GetActiveScene().name;
        currentSceneName = lastSceneName;

        Debug.Log("当前场景：" + currentSceneName);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg1 == LoadSceneMode.Single)
        {
            Debug.Log("改变场景：" + currentSceneName);
            currentSceneName = arg0.name;
            if (this != null)
                StopAllCoroutines();
        }
    }









    private void LateUpdate()
    {

        ///----延时脚本Update更新数据---- end
        CallTimerAfter();
    }


    void CallTimerAfter()
    {
        if (currentSceneName != lastSceneName)
        {
            lastSceneName = currentSceneName;
        }
    }


    private void OnDestroy()
    {
        StopAllCoroutines();

        foreach (var item in IEnumeratorHelper.valuePairs)
        {
            item.Value.Clear();
        }
        IEnumeratorHelper.valuePairs.Clear();

        _instance = null;
    }


}




public static partial class Extend
{

    #region 数组互转
    public static List<T> ExportKeys<T, U>(this Dictionary<T, U> targets)
    {
        List<T> temp = new List<T>();
        foreach (var item in targets)
        {
            temp.Add(item.Key);
        }
        return temp;
    }

    public static List<U> ExportValues<T, U>(this Dictionary<T, U> targets)
    {
        List<U> temp = new List<U>();
        foreach (var item in targets)
        {
            temp.Add(item.Value);
        }
        return temp;
    }

    public static List<T> ToList<T>(this T[] ts)
    {
        List<T> temp = new List<T>();
        for (int i = 0; i < ts.Length; i++)
        {
            temp.Add(ts[i]);
        }
        return temp;
    }
    #endregion


    #region 查找继承x基类的所有子类
    public static System.Type[] GetChildTypes(this System.Type parentType)
    {
        List<System.Type> lstType = new List<System.Type>();
        Assembly assem = Assembly.GetAssembly(parentType);
        foreach (System.Type tChild in assem.GetTypes())
        {
            if (tChild.BaseType == parentType)
            {
                lstType.Add(tChild);
            }
        }
        return lstType.ToArray();
    }
    #endregion


    #region 查找激活或者不激活的子物体
    public static T FindChildType<T>(this Transform target, string Url)
    {
        return target.Find(Url).GetComponent<T>();
    }


    public static Transform FindChildSelf(this Transform target, string Url, bool isActivate = true)
    {
        if (isActivate)
        {
            int id = 0;
            Transform targetTemp = target;
            string targetName = targetTemp.name;
            string[] vs = Url.Split('/');

            do
            {
                for (int i = 0; i < targetTemp.childCount; i++)
                {
                    if (vs[id] == targetTemp.GetChild(i).name)
                    {
                        targetTemp = targetTemp.GetChild(i);
                        id++;
                        break;
                    }
                }

                if (targetTemp.name != targetName)
                {
                    targetName = targetTemp.name;
                }
                else
                {
                    targetTemp = null;
                    break;
                }
            }
            while (vs.Length > id);

            return targetTemp;
        }
        else
        {
            return target.Find(Url).transform;
        }
    }
    #endregion


    #region UGUI坐标和世界坐标的转换
    public static Vector3 UIToWorldPos(this Transform selfTrans, Canvas canvas, Vector3 targetPos)
    {
        Vector3 scr = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetPos);
        scr.z = 0;
        scr.z = Mathf.Abs(Camera.main.transform.position.z - selfTrans.position.z);
        return Camera.main.ScreenToWorldPoint(scr);
    }


    public static void WorldToUIPos(this Transform selfTrans, Canvas canvas, Vector3 targetPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Camera.main.WorldToScreenPoint(targetPos), canvas.worldCamera, out pos);
        RectTransform rect = selfTrans.transform as RectTransform;
        rect.anchoredPosition = pos;
    }

    public static void WorldToUIPos(this Transform selfTrans, Camera camera, Canvas canvas, Vector3 targetPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, camera.WorldToScreenPoint(targetPos), canvas.worldCamera, out pos);
        RectTransform rect = selfTrans.transform as RectTransform;
        rect.anchoredPosition = pos;
    }

    public static bool IsNullOrEmpty(this string str)
    { 
        if (string.IsNullOrEmpty(str))
        {
            return true;
        }
        return false;
    }
    #endregion


    #region 类的协程函数
    public static Coroutine StartIEnumerator(IEnumerator enumerator)
    {
        return IEnumeratorHelper.RunIEnumerator(enumerator);
    }
    public static Coroutine StartIEnumerator<T>(this T target, IEnumerator enumerator)
    {
        return IEnumeratorHelper.RunIEnumerator(target.GetHashCode(), enumerator);
    }



    public static void StopIEnumerator<T>(this T target, Coroutine enumerator)
    {
        IEnumeratorHelper.StopIEnumerator(enumerator);
    }

    public static void StopAllIEnumerator<T>(this T target)
    {
        IEnumeratorHelper.StopAllIEnumerator(target.GetHashCode());
    }
    #endregion


    #region 数组内随机取值
    public static List<T> RandomArray<T>(this T[] ts, int getNumber)
    {
        List<int> randomIDs = new List<int>();
        for (int i = 0; i < ts.Length; i++)
        {
            randomIDs.Add(i);
        }

        List<T> temps = new List<T>();
        for (int i = 0; i < getNumber; i++)
        {
            int id = Random.Range(0, randomIDs.Count);
            temps.Add(ts[randomIDs[id]]);
            randomIDs.RemoveAt(id);
        }

        return temps;
    }

    public static List<T> RandomArray<T>(this T[] ts)
    {
        List<int> randomIDs = new List<int>();
        for (int i = 0; i < ts.Length; i++)
        {
            randomIDs.Add(i);
        }

        List<T> temps = new List<T>();
        for (int i = 0; i < ts.Length; i++)
        {
            int id = Random.Range(0, randomIDs.Count);
            temps.Add(ts[randomIDs[id]]);
            randomIDs.RemoveAt(id);
        }
        return temps;
    }

    public static List<T> RandomArrayCount<T>(this T[] ts, int count)
    {
        List<int> randomIDs = new List<int>();
        for (int i = 0; i < ts.Length; i++)
        {
            randomIDs.Add(i);
        }

        List<T> temps = new List<T>();
        for (int i = 0; i < count; i++)
        {
            int id = Random.Range(0, randomIDs.Count);
            temps.Add(ts[randomIDs[id]]);
            randomIDs.RemoveAt(id);
        }
        return temps;
    }

    public static List<T> RandomArrayCount<T>(this List<T> ts, int count)
    {
        List<int> randomIDs = new List<int>();
        for (int i = 0; i < ts.Count; i++)
        {
            randomIDs.Add(i);
        }

        List<T> temps = new List<T>();
        for (int i = 0; i < count; i++)
        {
            int id = Random.Range(0, randomIDs.Count);
            temps.Add(ts[randomIDs[id]]);
            randomIDs.RemoveAt(id);
        }
        return temps;
    }



    public static List<int> RandomArrayID<T>(this List<T> ts, int count)
    {
        List<int> randomIDs = new List<int>();
        for (int i = 0; i < ts.Count; i++)
        {
            randomIDs.Add(i);
        }

        List<int> temps = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int id = Random.Range(0, randomIDs.Count);
            temps.Add(id);
            randomIDs.RemoveAt(id);
        }
        return temps;
    }

    public static List<int> FindIndexs<T>(this List<T> ts, System.Predicate<T> match)
    {
        List<T> childList = ts.FindAll(match);

        List<int> indexs = new List<int>();

        int id = 0;
        while (ts.Count > id)
        {
            if (childList.Contains(ts[id]))
            {
                indexs.Add(id);
            }
            id++;
        }

        return indexs;
    }



    #endregion

    public static int ClampLoop<T>(this T[] ts, int count)
    {
        if (ts.Length <= count) count = 0;
        return count;
    }

    public static int ClampLoop<T>(this List<T> ts, int count)
    {
        if (ts.Count <= count) count = 0;
        return count;
    }


    public static byte[] IntToByte(this object target, int index)
    {
        byte[] b = new byte[2];
        int b1 = index / 256;
        int b2 = index % 256;

        b[0] = byte.Parse(b1.ToString());
        b[1] = byte.Parse(b2.ToString());
        return b;
    }




    #region 判断随机概率是否触发
    public static bool RandomValue<T>(this T target, float oddsValue)
    {
        float minRange = 0;
        float maxRange = 100;
        float middleValue = oddsValue / 2.0f;

        int randomId = Random.Range((int)middleValue, 100 - (int)middleValue);


        if (randomId - Mathf.Round(oddsValue / 2.0f) < 0)
        {
            minRange = 0;
            maxRange = oddsValue;
        }
        else
        {
            minRange = (int)(randomId - Mathf.Round(oddsValue / 2.0f));
        }


        if (randomId + Mathf.Round(oddsValue / 2.0f) > 100)
        {
            maxRange = 100;
            minRange = 100 - oddsValue;
        }
        else
        {
            maxRange = (int)(randomId + Mathf.Round(oddsValue / 2.0f));
        }


        float value = Random.Range(0.1f, 99.9f);
        if (minRange <= value && maxRange > value)
        {
            return true;
        }
        return false;
    }

    public static int RandomValue<T>(this T target, float[] oddsValues)
    {

        List<int> ranges = new List<int>();
        List<int> rangeInts = new List<int>();
        Dictionary<int, int> keyValues = new Dictionary<int, int>();

        int num = 0;
        for (int i = 0; i < oddsValues.Length; i++)
        {
            int j = 0;
            while (j < oddsValues[i])
            {
                keyValues.Add(num, i);

                num++;
                j++;
            }
        }

        foreach (var item in keyValues.Keys)
        {
            rangeInts.Add(item);
        }

        ranges = rangeInts.ListRandom();

        int value = Random.Range(0, num);

        return keyValues[ranges[value]];
    }

    public static int RandomValue<T>(this T target, int[] oddsValues)
    {

        List<int> ranges = new List<int>();
        List<int> rangeInts = new List<int>();
        Dictionary<int, int> keyValues = new Dictionary<int, int>();

        int num = 0;
        for (int i = 0; i < oddsValues.Length; i++)
        {
            int j = 0;
            while (j < oddsValues[i])
            {
                keyValues.Add(num, i);

                num++;
                j++;
            }
        }

        foreach (var item in keyValues.Keys)
        {
            rangeInts.Add(item);
        }

        ranges = rangeInts.ListRandom();

        int value = Random.Range(0, num);

        return keyValues[ranges[value]];
    }

    public static List<T> ListRandom<T>(this List<T> target)
    {
        List<T> ts = new List<T>();
        for (int i = 0; i < target.Count; i++)
        {
            ts.Add(target[i]);
        }

        List<T> temps = new List<T>();

        while (ts.Count > 0)
        {
            int id = Random.Range(0, ts.Count);
            temps.Add(ts[id]);
            ts.RemoveAt(id);
        }

        return temps;
    }

    #endregion


    #region reset一个物体并设置父物体
    public static void Reset(this Transform target)
    {
        target.transform.localScale = Vector3.one;
        target.transform.localPosition = Vector3.zero;
        target.transform.localEulerAngles = Vector3.zero;
    }

    public static void Reset(this Transform target, Transform parent)
    {
        target.transform.SetParent(parent);
        target.transform.localScale = Vector3.one;
        target.transform.localPosition = Vector3.zero;
        target.transform.localEulerAngles = Vector3.zero;
    }
    #endregion


    public static void SetRepeatActive<T>(this T target, bool value) where T : Component
    {
        if (value)
        {
            if (target.gameObject.activeSelf)
            {
                target.gameObject.SetActive(false);

                //Timer .instance.CallLater(() =>
                // {
                //     target.gameObject.SetActive(true);
                // });
            }
            else
                target.gameObject.SetActive(true);
        }
        else
        {
            target.gameObject.SetActive(false);
        }
    }






    /// <summary>
    /// 获取 target 下所有的 T 组件
    /// </summary>
    public static T[] GetTransformChildTypes<T>(this Transform target) where T : Component
    {
        var ts = new T[target.childCount];

        for (int i = 0; i < target.childCount; i++)
        {
            ts[i] = target.GetChild(i).GetComponent<T>();
        }
        return ts;
    }


    public static Transform[] GetTransformChildTypes(this Transform target)
    {
        var ts = new Transform[target.childCount];

        for (int i = 0; i < target.childCount; i++)
        {
            ts[i] = target.GetChild(i).GetComponent<Transform>();
        }
        return ts;
    }



    public static void ArrayActiveOnlyID<T>(this T[] target, int id) where T : Component
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i].gameObject.SetActive(id == i);
        }
    }



    public static void ArrayActiveOnlyID(this GameObject[] target, int id)
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i].SetActive(i == id);
        }
    }



    public static TimeExtend ActiveThenDelayedClose(this GameObject target, float delayedTime)
    {
        target.SetActive(true);
        var time = ExtendUpdate.instance.UpdateTime(delayedTime).OnComelete(() =>
          {
              target.SetActive(false);
          });
        return time;
    }

}


public class IEHelper
{

    bool isUntil = false;
    public bool Until()
    {
        if (isUntil)
        {
            isUntil = false;
            return true;
        }
        return false;
    }


    bool isWhile = true;
    public bool While()
    {
        if (isWhile == false)
        {
            isWhile = true;
            return false;
        }
        return true;
    }



    public void SatisfyWhile()
    {
        isWhile = false;
    }


    public void SatisfyUntil()
    {
        isUntil = true;
    }
}

/// <summary>
/// 自定义协程工具
/// </summary>
public class IEnumeratorHelper
{
    public static Dictionary<int, List<Coroutine>> valuePairs = new Dictionary<int, List<Coroutine>>();


    public static Coroutine RunIEnumerator(IEnumerator coroutine)
    {
        Coroutine cor = ExtendUpdate.instance.StartCoroutine(coroutine);
        return cor;
    }


    public static Coroutine RunIEnumerator(int hashID, IEnumerator coroutine)
    {
        Coroutine cor = ExtendUpdate.instance.StartCoroutine(coroutine);

        if (valuePairs.ContainsKey(hashID))
        {
            valuePairs[hashID].Add(cor);
        }
        else
        {
            List<Coroutine> coroutines = new List<Coroutine>();
            coroutines.Add(cor);
            valuePairs.Add(hashID, coroutines);
        }
        return cor;
    }

    public static void StopAllIEnumerator(int hodeID)
    {
        if (valuePairs.Count > 0)
        {
            if (valuePairs.ContainsKey(hodeID))
            {
                List<Coroutine> coroutines = valuePairs[hodeID];
                for (int i = 0; i < coroutines.Count; i++)
                {
                    if (coroutines[i] != null)
                        ExtendUpdate.instance.StopCoroutine(coroutines[i]);
                }
                valuePairs.Remove(hodeID);
            }
        }

    }

    public static void StopIEnumerator(Coroutine coroutine)
    {
        ExtendUpdate.instance.StopCoroutine(coroutine);
    }



}

/// <summary>
/// 自定义曲线工具
/// </summary>
public class Curve
{
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 middlePos;


    /// <summary>
    /// 创建曲线轨迹
    /// </summary>
    public void CreatCurve(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.middlePos = (startPos + endPos) * 0.5f;
    }


    /// <summary>
    /// 设置中间点的位置的系数
    /// </summary>
    public void SetAddMiddle(Vector3 middlePos, float ratio)
    {
        this.middlePos = Vector3.Lerp(this.startPos, middlePos, ratio);
        lastPos = this.startPos;
    }

    /// <summary>
    /// 设置中间点的位置
    /// </summary>
    public void SetAddMiddle(Vector3 middlePos)
    {
        this.middlePos = middlePos;
    }


    private Vector3 GetBezierPoint3D(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, float t)
    {
        float cx = 3 * (control1.x - anchor1.x);
        float bx = 3 * (control2.x - control1.x) - cx;
        float ax = anchor2.x - anchor1.x - cx - bx;
        float cy = 3 * (control1.y - anchor1.y);
        float by = 3 * (control2.y - control1.y) - cy;
        float ay = anchor2.y - anchor1.y - cy - by;
        float cz = 3 * (control1.z - anchor1.z);
        float bz = 3 * (control2.z - control1.z) - cz;
        float az = anchor2.z - anchor1.z - cz - bz;
        return new Vector3((ax * (t * t * t)) + (bx * (t * t)) + (cx * t) + anchor1.x, (ay * (t * t * t)) + (by * (t * t)) + (cy * t) + anchor1.y, (az * (t * t * t)) + (bz * (t * t)) + (cz * t) + anchor1.z);
    }

    /// <summary>
    /// 点随着系数输出路径坐标（0-1）
    /// </summary>
    public Vector3 UpdateCurvePos(float timeRadio)
    {
        float timeT = timeRadio - ((int)timeRadio);

        currentPos = GetBezierPoint3D(startPos, middlePos, endPos, endPos, timeT);

        Vector3 forwardDir = currentPos - lastPos;
        Quaternion lookAtRot = Quaternion.LookRotation(forwardDir);
        resultEuler = lookAtRot.eulerAngles;

        //lastPos = currentPos;


        Vector3 dir = currentPos - lastPos;

        float dotValue = Vector3.Dot(Vector3.up, dir.normalized);
        //获得夹角值
        angle = Mathf.Acos(dotValue) * Mathf.Rad2Deg;



        return GetBezierPoint3D(startPos, middlePos, endPos, endPos, timeT);
    }

    /// <summary>
    /// 点随着系数输出路径坐标（0-1）Loop
    /// </summary>
    public Vector3 UpdateCurvePos01(float timeRadio)
    {
        timeRadio = Mathf.Clamp(timeRadio, 0, 0.98f);

        float timeT = timeRadio - ((int)timeRadio);


        currentPos = GetBezierPoint3D(startPos, middlePos, endPos, endPos, timeT);

        Vector3 forwardDir = currentPos - lastPos;
        Quaternion lookAtRot = Quaternion.LookRotation(forwardDir);
        resultEuler = lookAtRot.eulerAngles;

        //lastPos = currentPos;


        Vector3 dir = currentPos - lastPos;

        float dotValue = Vector3.Dot(Vector3.up, dir.normalized);
        //获得夹角值
        angle = Mathf.Acos(dotValue) * Mathf.Rad2Deg;



        return GetBezierPoint3D(startPos, middlePos, endPos, endPos, timeT);
    }

    Vector3 currentPos = Vector3.zero;
    Vector3 lastPos = Vector3.zero;

    Vector3 resultEuler = Vector3.zero;
    public Vector3 TargetEuler { get { return resultEuler; } }

    public float angle;
}




/// <summary>
/// 设置层级的函数
/// </summary>
public class AutoLayers
{
    public class LayerData
    {
        public int layer;
        public string layerName;
        public List<GameObject> mainObjs = new List<GameObject>();

        public void AddMainObjects(GameObject m_objects)
        {
            mainObjs.Add(m_objects);
        }

        public void SetLayer(int nLayer)
        {
            for (int i = 0; i < mainObjs.Count; i++)
            {
                mainObjs[i].layer = nLayer;
            }
        }

        public void ResetLayer()
        {
            for (int i = 0; i < mainObjs.Count; i++)
            {
                mainObjs[i].layer = layer;
            }
        }

        public void Clear()
        {
            mainObjs.Clear();
        }
    }

    public Dictionary<int, LayerData> layerDatas = new Dictionary<int, LayerData>();
    public List<int> layerIDs = new List<int>();

    public AutoLayers(Transform target)
    {

        CallUnit.destroyCall.AddListener(Clear);

        Transform[] targets = target.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < targets.Length; i++)
        {
            int layerID = targets[i].gameObject.layer;

            if (layerDatas.ContainsKey(layerID))
            {
                layerDatas[layerID].AddMainObjects(targets[i].gameObject);
            }
            else
            {
                LayerData data = new LayerData();
                data.layer = targets[i].gameObject.layer;
                data.layerName = LayerMask.LayerToName(targets[i].gameObject.layer);
                data.AddMainObjects(targets[i].gameObject);
                layerIDs.Add(data.layer);
                layerDatas.Add(layerID, data);
            }
        }
    }

    public void SetLayers(string layerName)
    {
        for (int i = 0; i < layerDatas.Count; i++)
        {
            layerDatas[layerIDs[i]].SetLayer(LayerMask.NameToLayer(layerName));
        }
    }

    public void ResetLayers()
    {
        for (int i = 0; i < layerDatas.Count; i++)
        {
            layerDatas[layerIDs[i]].ResetLayer();
        }
    }

    public void Clear()
    {
        CallUnit.destroyCall.RemoveListener(Clear);

        layerIDs.Clear();

        foreach (var item in layerDatas.Values)
        {
            item.Clear();
        }
        layerDatas.Clear();
    }
}



