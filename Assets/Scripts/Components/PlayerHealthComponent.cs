using System;
using Events;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components
{
    public class PlayerHealthComponent : HealthComponent.HealthComponent
    {
        private PlayerController _playerController;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            //_maxHealth = PlayerStatManager.Instance.maxHealth;
            _currentHealth = PlayerStatManager.Instance.maxHealth;
            GameEventManager.Instance.levelEvents.LevelTimerFinished += DisableHealth;
            GameEventManager.Instance.sceneEvents.SceneLoaded += () => enabled = true;
        }

        private void DisableHealth()
        {
            PlayerStatManager.Instance.maxHealth = _maxHealth;
            PlayerStatManager.Instance.health = _currentHealth;
            enabled = false;
        }

        public override void Die()
        {
            if (_currentHealth > 0) return;
            _sr.color = Color.red;
        }

        public override void TakeDamage(int amount)
        {
            if (_playerController.invincibleTimeBuffer > 0) return;
            base.TakeDamage(amount);
            _playerController.MakeInvincible();
            Random.InitState(DateTime.Now.Millisecond);
            GameEventManager.Instance.resourceEvents.OnTakeBlood(Random.Range(3, 8));
            GameEventManager.Instance.uiEvents.OnUpdateHealth(_currentHealth);
        }
    }
}
