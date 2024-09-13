using System.Collections;
using System.Collections.Generic;
using LayerAndSorting;
using UnityEngine;

[CreateAssetMenu(fileName = "LsSave.asset", menuName = "Layer/LsInfoSave")]
public class LsInfoSave : ScriptableObject
{
    public List<ChangeData> changeDatas;

    public ChangeData FindData(GameObject selectGo)
    {
       return changeDatas.Find(_ => _?.mainGO == selectGo);
    }

    public void Save(GameObject selectGo, List<LsInfo> findInfos)
    {
        if(findInfos == null || findInfos.Count == 0)
        {
            return;
        }

        var mainInfo = changeDatas.Find(_ => _?.mainGO == selectGo);
        if (mainInfo == null)
        {
            mainInfo = new ChangeData() { mainGO = selectGo };
            mainInfo . savetime = System.DateTime.Now.ToString();
            mainInfo . backSteps                 = new List<ChangeStep>();
            mainInfo . forwardSteps              = new List<ChangeStep>();
            changeDatas.Add(mainInfo);
        }

        var step = new ChangeStep() { infos = new List<ChangeInfo>() };
        foreach (var item in findInfos)
        {
            if (item.info == null)
                continue;

            step.infos.Add(new ChangeInfo()
            {
                Layer         = item . Layer,
                OrderInLayer  = item . OrderInLayer,
                SortLayerName = item . SortLayerName,
                mainGo        = item . mainGo,
                sortType      = item . sortType
            });
        }
        if (mainInfo.forwardSteps == null)
        {
            mainInfo.forwardSteps = new List<ChangeStep>();
        }

        if (mainInfo.currentStep != null)
        { 
            mainInfo.forwardSteps.Add(mainInfo.currentStep);
        }
        mainInfo.currentStep = step;
    }

    public void Next(GameObject selectGo)
    {
        var mainInfo = changeDatas.Find(_ => _?.mainGO == selectGo);
        if (mainInfo != null)
        {
            if (mainInfo.backSteps.Count > 0)
            {
                var step = mainInfo.backSteps[mainInfo.backSteps.Count - 1];
                for (int i = 0; i < step.infos.Count; i++)
                {
                    var info = step.infos[i];
                    if (info.mainGo == null)
                        continue;

                    LsInfo ls = new LsInfo(info.sortType, info.mainGo, 0);
                    ls.SortLayerName = info.SortLayerName;
                    ls.OrderInLayer  = info.OrderInLayer;
                    ls.Layer = info.Layer;
                }
                mainInfo.backSteps.Remove(step);

                if (mainInfo.forwardSteps == null)
                {
                    mainInfo.forwardSteps = new List<ChangeStep>();
                }

                mainInfo.forwardSteps.Add(mainInfo.currentStep);
                mainInfo.currentStep = step;
            }
        }
    }

    public void Back(GameObject selectGo)
    {
        var mainInfo = changeDatas.Find(_ => _?.mainGO == selectGo);
        if (mainInfo != null)
        {
            if (mainInfo.forwardSteps.Count > 0)
            {
                var step = mainInfo.forwardSteps[mainInfo.forwardSteps.Count - 1];
                for (int i = 0; i < step.infos.Count; i++)
                {
                    var info = step.infos[i];
                    if (info.mainGo == null)
                        continue;

                    LsInfo ls = new LsInfo(info.sortType, info.mainGo,0);
                    ls . SortLayerName = info . SortLayerName;
                    ls . OrderInLayer  = info . OrderInLayer;
                    ls . Layer         = info . Layer;
                }
                mainInfo.forwardSteps.Remove(step);

                if (mainInfo.backSteps == null)
                { 
                    mainInfo.backSteps = new List<ChangeStep>();
                }

                mainInfo.backSteps.Add(mainInfo.currentStep);
                mainInfo.currentStep = step;
            }
        }
    }

    public void Clear()
    {
        changeDatas.Clear();
    }

}

[System.Serializable]
public class ChangeData
{
    public string savetime;

    public int step
    {
        get 
        { 
            int value = 0;
            if (forwardSteps != null)
            {
                value += forwardSteps.Count;
            }
            if (backSteps != null)
            {
                value += backSteps.Count;
            }
            if (currentStep != null)
            { 
                value += 1;
            }
            return value;
        }
    }
    public GameObject mainGO;
    public ChangeStep currentStep;
    public List<ChangeStep> forwardSteps;
    public List<ChangeStep> backSteps;
}

[System.Serializable]
public class ChangeStep
{
    public List<ChangeInfo> infos;
}

[System.Serializable]
public class ChangeInfo
{
    public string SortLayerName;
    public int    OrderInLayer;
    public int    Layer;
    public GameObject mainGo;
    public SortType sortType;
}