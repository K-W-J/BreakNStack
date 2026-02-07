using Code.Modules;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Checkers
{
    public class OverlapChecker2D : MonoBehaviour, IModule
    {
        public UnityEvent OnTargetEnterEvent;
        
        [SerializeField] protected Transform checkPoint;
        [SerializeField] protected LayerMask targetMask;
        [SerializeField] protected int maxResultCount;
        [Space]
        [SerializeField] protected bool viewDrawGizmos;
        
        protected Collider[] Results;
        
        public virtual void InitializeComponent(ModuleOwner owner)
        {
            Results = new Collider[maxResultCount];
        }
    }
}