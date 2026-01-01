using Code.Modules;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Agents
{ 
    public class Agent : ModuleOwner
    {
        [field:SerializeField] public bool CanDealDamage { get; private set; }
        public bool IsDead { get; private set; }
        
        public UnityEvent OnDeath;
        public UnityEvent OnHit;
    }
}