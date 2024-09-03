using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pool
{
    [RequireComponent(typeof(PoolType))]
    public class PoolLoadType : MonoBehaviour
    {
       public enum SortType
        {
            AssetBundle = 0,
            Addressable
        }

        public SortType type = SortType.AssetBundle;
        PoolType sort;

        #region Type => AssetBundle

        /// <summary>
        /// Type => AssetBundle  ����Դ����
        /// </summary>
        public string assetbundle;
        public string Assetbundle
        {
            get { return assetbundle; }
        }

        public string assetbundlePrefab;
        public string AssetbundlePrefab
        {
            get { return assetbundlePrefab; }
        } 

        #endregion


        #region Type => Addressable

        /// <summary>
        /// Type => Addressable  ����Դ����
        /// </summary>
        public AssetReference assetReference;
        public AssetReference AssetReference
        {
            get { return assetReference; }
        } 

        #endregion


 
        private void Awake()
        {
            sort = this.GetComponent<PoolType>();
            sort.m_prefab = null;
        }


        private async void Start()
        {
            if (type == SortType.Addressable)
            {
                var go = (GameObject)await AssetReference.InstantiateAsync();
                go.transform.SetParent(this.transform);
                go.SetActive(false);
                sort.m_prefab = go;
                sort.Preload();
            }
            else
            {
                Debug.Log("AssetBundle ���ط�ʽ��δʵ��");
            }
        }


        private void OnDestroy()
        {
           if (type == SortType.Addressable)
            {
                AssetReference.ReleaseInstance(sort.m_prefab);
                AssetReference.ReleaseAsset();
            }
        }
    }
}
