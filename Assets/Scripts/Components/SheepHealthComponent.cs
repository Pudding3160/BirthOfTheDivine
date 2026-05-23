using System;
using Events;
using UnityEngine;

namespace Components
{
    public class SheepHealthComponent : HealthComponent.HealthComponent
    {
        public int blood;

        private void Start()
        {
            GameEventManager.Instance.miscEvents.ActivateSheep += Activate;
        }

        public override void Die()
        {
            if (_currentHealth > 0) return;
            
            Destroy(gameObject);
        }

        private void Activate()
        {
            enabled = true;
            Debug.Log("Beee");
        }
    }
}