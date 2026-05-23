using System;
using Components;
using UnityEngine;

namespace Enemy
{
    public class EnemyBullet : MonoBehaviour
    {
        public int damage;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var hc = other.GetComponent<PlayerHealthComponent>();
            hc?.TakeDamage(damage);
        }
    }
}