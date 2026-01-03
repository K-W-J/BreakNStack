using UnityEngine;

namespace Code.Core
{
    public interface IPoolable
    {
        public PoolItemSO PoolItem { get; }
        public GameObject GameObject { get; }
        public void SetUpPool(PoolManager pool);
        public void ResetItem();
    }
}