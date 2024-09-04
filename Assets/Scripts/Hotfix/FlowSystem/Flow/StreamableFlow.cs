using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Flow;

public class StreamableFlow : IFlowTask
{
    public int layer => 0;
    public int order => 15;

    public async Task Logic()
    {
  
        await Streamable.LoadPakeage();
    }
}
