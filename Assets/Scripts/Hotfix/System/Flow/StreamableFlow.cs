using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class StreamableFlow : IFlowTask
{
    public int layer => 0;
    public int order => 15;

    public async Task Logic()
    {
        Debug.Log("¼ÓÔØStreamable");

        Streamable.Register<Ss>();
      //  Streamable.Register<TR>();

        await Streamable.LoadPakeage();
    }
}
