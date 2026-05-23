using Components;
using Components.HealthComponent;
using UnityEngine;
using UnityEngine.VFX;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        private VisualEffectObject vfx;
        private TrailRenderer trail;
        private Rigidbody2D _rb;
        private Vector2 _moveDirection;
        [SerializeField]
        protected float speed;
        [SerializeField] 
        private string objectToTargetTag;
        [SerializeField]
        protected int damage;
        [SerializeField] 
        private float TTL; // Time To Live - Racunalne mreze reference
        private float _timerBuffer;
        //particlethingo
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            trail = GetComponent<TrailRenderer>();

        }

        private void OnEnable()
        {
            
            if (trail != null)
            {
                trail.Clear();
            }
        }

        public void Initialize(Vector2 startingPosition, Vector2 direction)
        {
            
            transform.position = startingPosition;
            _moveDirection = direction;
            gameObject.SetActive(true);
            _timerBuffer = TTL;
        }
        
        private void Update()
        {
            _timerBuffer -= Time.deltaTime;
            if (_timerBuffer <= 0) gameObject.SetActive(false);
        }
        
        private void FixedUpdate()
        {
            Move();
        }
        
        private void Move()
        {
            _rb.MovePosition(_rb.position + _moveDirection * (speed * Time.fixedDeltaTime));
        }
        
        public virtual void Hit(HealthComponent healthComponent)
        {
            gameObject.SetActive(false);
            GetComponent<ParticleThingo>().SpawnParticle();
            healthComponent?.TakeDamage(damage);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Failsafes
            if (objectToTargetTag == "") return;
            if (other.CompareTag("Sheep"))
            {
                other.TryGetComponent(typeof(SheepHealthComponent), out var sheepHealthComponent);
                Hit((SheepHealthComponent)sheepHealthComponent);
                return;
            }
            if (!other.CompareTag(objectToTargetTag)) return;
            other.TryGetComponent(typeof(HealthComponent), out var healthComponent);
            Hit((HealthComponent)healthComponent);
        }

        
    }
}
