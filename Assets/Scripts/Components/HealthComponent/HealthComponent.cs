using System;
using Events;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections;

namespace Components.HealthComponent
{
    public class HealthComponent : MonoBehaviour, IHealthComponent
    {
        [SerializeField]
        protected int _currentHealth;
        [SerializeField]
        protected int _maxHealth;
        [CanBeNull] private EnemyController enemyController;

        public bool isDead;

        public Animator animator;


        private void Awake()
        {
            enemyController = GetComponent<EnemyController>();
            animator = GetComponent<Animator>();
        }

        public virtual void Heal(int amount)
        {
            _currentHealth += amount;
            ClampHealth();
        }

        public virtual void TakeDamage(int amount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= amount;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            if (_currentHealth > 0) return;

            if (isDead) return;

            isDead = true;

            StartCoroutine(DeathRoutine());
        }

        public virtual void ClampHealth()
        {
            if (_currentHealth <= _maxHealth) return;
            _currentHealth = _maxHealth;
        }

        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        public int GetCurrentHealth()
        {
            return _currentHealth;
        }


        IEnumerator DeathRoutine()
        {
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            var controller = GetComponent<EnemyController>();
            if (controller != null) controller.enabled = false;

            var melee = GetComponent<EnemyMeleeAttack>();
            if (melee != null) melee.enabled = false;

            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            animator.SetTrigger("Death");

            yield return new WaitForSeconds(1.5f);

            if (gameObject.tag == "Enemy")
            {
                GameObject[] allOrbs =
                GameObject.FindGameObjectsWithTag("Orb");

                foreach (GameObject o in allOrbs)
                {
                    Destroy(o);
                }
            }

            Destroy(gameObject);
        }
    }
}