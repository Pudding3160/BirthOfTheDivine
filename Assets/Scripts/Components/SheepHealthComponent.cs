using System;
using Events;
using Managers;
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
            GameEventManager.Instance.resourceEvents.OnRewardBlood(blood);
            Destroy(gameObject);
        }

        private void Activate()
        {
            enabled = true;
            blood = Mathf.Abs(Mathf.CeilToInt((ResourceManager.Instance.requiredBlood -
                                               ResourceManager.Instance.currentOfferedBlood) / 5));
        }
    }
}