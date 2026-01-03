using Blade.Core;
using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Effects
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private PoolManager pool;
        [SerializeField] private GameEventChannelSO effectChannel;

        private void Awake()
        {
            effectChannel.AddListener<PlayEffectEvent>(HandlePlayEffect);
        }

        private void OnDestroy()
        {
            effectChannel.RemoveListener<PlayEffectEvent>(HandlePlayEffect);
        }

        private void HandlePlayEffect(PlayEffectEvent evt)
        {
            ParticlePlayer particlePlayer = pool.Pop<ParticlePlayer>(evt.effectItem);
            particlePlayer.transform.position = evt.position;
        }
    }
}