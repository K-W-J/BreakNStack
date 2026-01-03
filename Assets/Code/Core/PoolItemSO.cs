using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(fileName = "PoolItemData", menuName = "SO/PoolItem", order = 0)]
    public class PoolItemSO : ScriptableObject
    {
        public int initCount;
        public GameObject prefab;
    }
}