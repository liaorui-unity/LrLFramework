using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TableFlow : IFlowTask
{
    public int layer => 0;

    public int order => 16;


    public Task Logic()
    {
        Debug.Log("加载Table ");
        Table.TableLib.InitTable();
        return Task.Delay(1000);
    }
}
