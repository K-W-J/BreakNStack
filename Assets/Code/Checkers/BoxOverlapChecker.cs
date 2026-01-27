using System.Collections.Generic;
using UnityEngine;

namespace Code.Checkers
{
    public class BoxOverlapChecker : OverlapChecker
    {
        [SerializeField] private Vector3 boxSize;
        
        public bool BoxOverlapCheck()
        {
            bool isOverlap = Physics.CheckBox(checkPoint.position,
                boxSize * 0.5f, checkPoint.rotation, targetMask);
            
            if (isOverlap)
            {
                OnTargetEnterEvent.Invoke();
            }
            
            return isOverlap;
        }
        
        public bool TryGetOverlapData<T>(List<T> outputList, Vector2 outBoxSize, bool includeInactive = false) where T : MonoBehaviour
        {
            int count = Physics.OverlapBoxNonAlloc(checkPoint.position, outBoxSize * 0.6f,
                Results, checkPoint.rotation, targetMask);
            
            for (int i = 0; i < count; i++)
            {
                if (Results[i].TryGetComponent(out T component) == false)
                {
                    component = Results[i].GetComponentInParent<T>(includeInactive);
                    if (component == null)
                        component = Results[i].GetComponentInChildren<T>(includeInactive);
                }

                if (component != null && outputList.Contains(component) == false)
                {
                    outputList.Add(component);
                }
            }
            
            return outputList.Count > 0;
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(viewDrawGizmos == false || checkPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.matrix = checkPoint.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
        #endif
    }
}