using System.Collections.Generic;
using UnityEngine;

namespace Code.Checkers
{
    public class SphereOverlapChecker2D : OverlapChecker2D
    {
        [SerializeField] private float radius;

        public bool SphereOverlapCheck()
        {
            bool isOverlap = Physics.CheckSphere(checkPoint.position,
                radius, targetMask);
            
            if (isOverlap)
            {
                OnTargetEnterEvent.Invoke();
            }
            
            return isOverlap;
        }
        
        public bool TryGetOverlapData<T>(List<T> outputList, float outRadius, bool includeInactive = false) where T : MonoBehaviour
        {
            int count = Physics.OverlapSphereNonAlloc(checkPoint.position, outRadius,
                Results, targetMask);
            
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
            Gizmos.DrawWireSphere(checkPoint.position, radius);
        }
        #endif
    }
}