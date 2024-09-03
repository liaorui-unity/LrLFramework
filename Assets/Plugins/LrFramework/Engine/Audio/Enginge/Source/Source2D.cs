using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class Source2D : Source
    {
 
        public override void StopClip()
        {
            if (isRuning)
            {
                ObjectPool.Release(this);
                AudioMgr.Instance?.PlayClipComplete(this);
            }

            base.StopClip();
        }
    }
}