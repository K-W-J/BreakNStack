using Unity.Cinemachine;
using UnityEngine;

namespace Code.Feedbacks
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class ImpulseFeedback : Feedback
    {
        [SerializeField] private float impulsePower;
        private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public override void CreateFeedback()
        {
            Debug.Assert(_impulseSource != null, "impulseSource is null"); //논리적으로는 null일은 없지만 그래도 혹시 모르니...
            _impulseSource.GenerateImpulse(impulsePower);
        }

        public override void StopFeedback()
        {
            
        }
    }
}