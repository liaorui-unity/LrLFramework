
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class Source3D : Source
    {   
        private AudioListener _listener;
        public AudioListener listener
        {
            get
            {
                if (_listener == null)
                {
                    _listener = GameObject.FindObjectOfType<AudioListener>();
                }
                return _listener;
            }
        }

        [SerializeField] private float radius    = 6;
        [SerializeField] private float maxRadius = 60;

   
        private void OnEnable()
        {
            updateCall = ComputeSpatialBlend;
        }

        private void OnDisable()
        {
            updateCall = null;
        }


        public void SetRadius(float radius, float maxRadius)
        {
            this.radius    = radius;
            this.maxRadius = maxRadius;
        }


        public void SetSource(Transform transform)
        {
            transform.SetParent(transform);
            transform.localPosition    = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.localScale       = Vector3.one;
        }

        float currentDis = 0;
        float lastDis    = 0;

        public virtual void ComputeSpatialBlend()
        {
            if (listener == null)
                return;

            currentDis = Vector3.Distance(transform.position, listener.transform.position);

            if (lastDis != currentDis)
            {
                if (currentDis > radius)
                    mainSource.spatialBlend = Mathf.Clamp01(currentDis / maxRadius);
                else
                    mainSource.spatialBlend = 0;
                lastDis = currentDis;
            }
        }



        public override void StopClip()
        {
            if (isRuning)
            {
                ObjectPool.Release(this);
                AudioMgr.Instance.PlayClipComplete(this);
            }

            base.StopClip();
            _listener = null;
        }
    }

}
