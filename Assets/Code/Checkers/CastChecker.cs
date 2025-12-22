using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Checkers
{
    public enum CastType
    {
        Ray,
        Box,
        Sphere,
    }
    
    public class CastChecker2D : MonoBehaviour
    {
        public UnityEvent OnTargetEnterEvent;
        
        [SerializeField] private Transform checkPoint;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private float castLength;
        [SerializeField] private int maxCount;
        [Space]
        [SerializeField] private Vector2 boxSize;
        [SerializeField] private float radius;
        [Space]
        [SerializeField] private bool showBoxCast;
        [SerializeField] private bool showSphereCast;
        [Space]
        //[SerializeField] private GameObject[] exceptObjects;
        
        private RaycastHit2D[] _results;
        
        public void Awake()
        {
            _results = new RaycastHit2D[maxCount];
        }
        
        public bool CastCheck(CastType castType = CastType.Ray)
        {
            bool isOverlap;

            switch (castType)
            {
                case CastType.Box:
                    isOverlap = Physics2D.BoxCast(checkPoint.position, boxSize * 0.5f, 0f,transform.up, targetMask);
                    break; 
                case CastType.Sphere:
                    isOverlap = Physics2D.CircleCast(checkPoint.position, radius,transform.up,castLength, targetMask);
                    break;
                default: //CastType.Ray
                    isOverlap = Physics2D.Raycast(checkPoint.position, transform.forward, castLength, targetMask);
                    break;
            }

            if (isOverlap)
            {
                OnTargetEnterEvent.Invoke();
            }
            
            return isOverlap;
        }
        
        public GameObject[] GetCastData(CastType castType = CastType.Ray)
        {
            switch (castType)
            {
                case CastType.Box:
                    _results = Physics2D.BoxCastAll(checkPoint.position, boxSize * 0.5f,
                        0f,transform.up,  castLength, targetMask);
                    break;
                case CastType.Sphere:
                    _results = Physics2D.CircleCastAll(checkPoint.position, radius,
                        transform.up,  castLength, targetMask);
                    break;
                default: //CastType.Ray
                    _results = Physics2D.RaycastAll(checkPoint.position, 
                        transform.up, castLength, targetMask);
                    break;
            }
            
            int count = _results.Count(s => s.collider != null);

            GameObject[] targets = new GameObject[count];

            for (int i = 0; i < count; i++)
            {
                targets[i] = _results[i].collider.gameObject;
            }

            if (count > 1)
            {
                Array.Sort(targets, (a, b) =>
                    Vector3.Distance(checkPoint.position, a.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
            }

            return targets;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(checkPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(checkPoint.position, transform.up * castLength);

            if (showBoxCast)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = checkPoint.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.up * castLength, boxSize);
            }
            
            if (showSphereCast)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = checkPoint.localToWorldMatrix;
                Gizmos.DrawWireSphere(Vector3.up * castLength, radius);
            }
        }
#endif
    }
}