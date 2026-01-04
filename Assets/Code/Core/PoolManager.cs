using System.Collections.Generic;
using UnityEngine;

namespace Code.Core
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private List<PoolItemSO> poolItems;
        private Dictionary<PoolItemSO, (Stack<IPoolable> poolStack, Transform parent)> _poolIDict;
        

        private void Awake()
        {
            _poolIDict = new Dictionary<PoolItemSO, (Stack<IPoolable>, Transform)>();
            
            foreach (var poolItem in poolItems)
            {
                GameObject parent = new GameObject
                {
                    name = poolItem.name,
                    transform = {parent = transform}
                };

                _poolIDict.Add(poolItem, (new Stack<IPoolable>(), parent.transform));
                
                for (int i = 0; i < poolItem.initCount; i++)
                {
                    GameObject poolObject = Instantiate(poolItem.prefab, parent.transform);
                    IPoolable item = poolObject.GetComponent<IPoolable>();
                    item.SetUpPool(this);
                    
                    _poolIDict[poolItem].poolStack.Push(item);
                    poolObject.SetActive(false);
                }
            }
        }

        public T Pop<T>(PoolItemSO poolItem) where T : IPoolable
        {
            IPoolable item;
            if (_poolIDict.TryGetValue(poolItem, out var pools))
            {
                if (pools.poolStack.Count == 0)
                {
                    GameObject poolObject = Instantiate(poolItem.prefab, pools.parent);
                    item = poolObject.GetComponent<IPoolable>();
                    item.SetUpPool(this);
                }
                else
                {
                    item = _poolIDict[poolItem].poolStack.Pop();
                    item.GameObject.SetActive(true);
                }
            }
            else
            {
                return default;
            }
            
            item.ResetItem();
            return (T)item;
        }
        
        public void Push(IPoolable poolable)
        {
            if (_poolIDict.TryGetValue(poolable.PoolItem, out var pools))
            {
                pools.poolStack.Push(poolable);
                poolable.GameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("PoolItem not found");
            }
        }
    }
}