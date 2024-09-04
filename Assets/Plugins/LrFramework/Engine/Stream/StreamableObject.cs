using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StreamableObject 
{
    public virtual void Apply() 
    {
        Streamable.Save(this);
    }
}

