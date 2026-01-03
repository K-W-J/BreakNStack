using System.Collections.Generic;
using UnityEngine;

namespace Code.Core
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private List<PoolItemSO> poolItems;
        private Dictionary<PoolItemSO, Stack<IPoolable>> _poolIDict;

        private void Awake()
        {
            _poolIDict = new Dictionary<PoolItemSO, Stack<IPoolable>>();
            
            foreach (var poolItem in poolItems)
            {
                _poolIDict.Add(poolItem, new Stack<IPoolable>());
                
                for (int i = 0; i < poolItem.initCount; i++)
                {
                    GameObject poolObject = Instantiate(poolItem.prefab, transform);
                    poolObject.SetActive(false);
                    
                    _poolIDict[poolItem].Push(poolObject.GetComponent<IPoolable>());
                }
            }
        }

        public T Pop<T>(PoolItemSO poolItem) where T : IPoolable
        {
            IPoolable item;
            if (_poolIDict[poolItem].Count == 0)
            {
                GameObject poolObject = Instantiate(poolItem.prefab, transform);
                item = poolObject.GetComponent<IPoolable>();
                item.SetUpPool(this);
            }
            else
            {
                item = _poolIDict[poolItem].Pop();
                item.GameObject.SetActive(true);
            }
            
            item.ResetItem();
            return (T)item;
        }
        
        public void Push(IPoolable poolable)
        {
            poolable.GameObject.SetActive(false);

            if (_poolIDict.TryGetValue(poolable.PoolItem, out Stack<IPoolable> stack))
            {
                stack.Push(poolable);
            }
        }
    }
}