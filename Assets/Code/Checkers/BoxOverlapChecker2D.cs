using System.Collections.Generic;
using UnityEngine;

namespace Code.Checkers
{
    public class BoxOverlapChecker2D : OverlapChecker2D
    {
        [SerializeField] private Vector3 boxSize;
        [SerializeField] private float boxSizeRatio;
        
        public Vector2 SetBoxSize(Vector2 outBoxSize) => boxSize = outBoxSize;
        
        public bool TryGetOverlapData<T>(List<T> outputList, bool includeInactive = false) where T : MonoBehaviour
        {
            outputList.Clear();
            
            Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPoint.position, boxSize * (0.5f * boxSizeRatio), 0,targetMask);

            foreach (var foundCollider in colliders)
            {
                if (foundCollider.TryGetComponent(out T component) == false)
                {
                    component = foundCollider.GetComponentInParent<T>(includeInactive);
                    if (component == null)
                        component = foundCollider.GetComponentInChildren<T>(includeInactive);
                }

                if (component != null && outputList.Contains(component) == false)
                {
                    outputList.Add(component);
                }
            }
            
            return colliders.Length > 0;
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(viewDrawGizmos == false || checkPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.matrix = checkPoint.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, boxSize * boxSizeRatio);
        }
        #endif
    }
}