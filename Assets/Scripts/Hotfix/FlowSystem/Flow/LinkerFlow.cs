using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Flow;

public class LinkerFlow : IFlowTask
{
    public int layer => 0;

    public int order => 2;

    public async Task Logic()
    {
        await AssetLoader.instance.Init();
    }
}
