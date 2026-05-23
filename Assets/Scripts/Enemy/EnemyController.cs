using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int damage;
    public int bloodReward;

    private Animator animator;
    private const string IS_WALKING = "is_walking_anim";
    private const string IS_CHARGING = "is_charging_anim";

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var healthComponent = other.gameObject.GetComponent<IHealthComponent>();
        healthComponent?.TakeDamage(damage);
    }

    public enum EnemyType
    {
        Melee,
        Ranged
    }

    [Header("Enemy Type")]
    public EnemyType enemyType;

    [Header("References")]
    public Transform player;
    private LookAtPlayer lookAtPlayer;

    private Rigidbody2D rb;

    [Header("General Movement")]
    public float moveSpeed = 3f;
    public float detectionRadius = 8f;

    [Header("Melee Settings")]
    public float meleeAttackRange = 1.5f;

    [Header("Ranged Settings")]
    public float preferredDistance = 8f;
    public float dodgeDistance = 5f;
    public float dodgeSpeed = 12f;
    public float dodgeCooldown = 0.5f;

    [Header("Charged Ranged Attack")]
    public float chargedAttackCooldown = 4f;
    public float chargeTime = 1.2f;

    private float lastChargedAttackTime;
    private bool isChargingAttack;
    public bool chargedAttack;

    [Header("Attack")]
    public float attackCooldown = 1f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    private float lastAttackTime;
    private float lastDodgeTime;

    private bool isDodging;
    private Vector2 dodgeDirection;

    public EnemyMeleeAttack _enemyMeleeAttack;

    void Start()
    {
        lookAtPlayer = GetComponent<LookAtPlayer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _enemyMeleeAttack = GetComponent<EnemyMeleeAttack>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        //if (distance > detectionRadius)
        //{
        //    rb.linearVelocity = Vector2.zero;
        //    return;
        //}

        switch (enemyType)
        {
            case EnemyType.Melee:
                HandleMeleeEnemy(distance);
                break;

            case EnemyType.Ranged:
                HandleRangedEnemy(distance);
                break;
        }
    }

    void HandleMeleeEnemy(float distance)
    {
        if (distance > meleeAttackRange)
        {
            MoveTowardsPlayer();
        }
    }


    void HandleRangedEnemy(float distance)
    {
        if (isDodging)
            return;

        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 awayFromPlayer = (transform.position - player.position).normalized;

        if (distance > preferredDistance)
        {
            rb.linearVelocity = toPlayer * moveSpeed;
        }
        else if (distance < preferredDistance)
        {
            rb.linearVelocity = awayFromPlayer * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (Time.time >= lastDodgeTime + dodgeCooldown)
        {
            StartDodge(awayFromPlayer);
        }

        if (!isChargingAttack && Time.time >= lastChargedAttackTime + chargedAttackCooldown)
        {
            StartCoroutine(ChargedBigShot());
            lastChargedAttackTime = Time.time;
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            RangedAttack();
            lastAttackTime = Time.time;
        }
    }

    IEnumerator ChargedBigShot()
    {
        animator.SetBool(IS_WALKING, false);

        yield return null;

        isChargingAttack = true;
        animator.SetBool(IS_CHARGING, true);

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("ChargeAttack");

        yield return new WaitForSeconds(chargeTime);

        if (player == null)
        {
            isChargingAttack = false;
            yield break;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.transform.localScale = Vector3.one * 2.5f;

        Rigidbody2D projectileRb =
            projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            Vector2 direction =
                (player.position - firePoint.position).normalized;

            projectileRb.linearVelocity =
                direction * (projectileSpeed * 0.7f);
        }

        animator.SetTrigger("Attack");

        chargedAttack = true;

        yield return new WaitForSeconds(0.4f);

        animator.SetBool(IS_WALKING, true);

        chargedAttack = false;
        isChargingAttack = false;

    }

    void SpawnProjectile(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * projectileSpeed * 1.2f;
        }
    }

    void StartDodge(Vector2 awayDirection)
    {

        animator.SetBool(IS_WALKING, false);

        if (isChargingAttack) return;

        isDodging = true;
        lastDodgeTime = Time.time;

        animator.SetTrigger("Dodge");

        Vector2 perpendicular =
            Random.value > 0.5f
            ? new Vector2(-awayDirection.y, awayDirection.x)
            : new Vector2(awayDirection.y, -awayDirection.x);

        dodgeDirection = (awayDirection + perpendicular).normalized;

        if (lookAtPlayer != null)
        {
            lookAtPlayer.LockLookDirection(dodgeDirection);
        }

        Invoke(nameof(StopDodge), 0.2f);

        animator.SetBool(IS_WALKING, true);
    }

    void StopDodge()
    {
        isDodging = false;

        if (lookAtPlayer != null)
        {
            lookAtPlayer.UnlockLookDirection();
        }
    }

    void RangedAttack()
    {
        if (isChargingAttack) return;

        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            Vector2 direction =
                (player.position - firePoint.position).normalized;

            projectileRb.linearVelocity = direction * projectileSpeed;
        }
    }

    void MoveTowardsPlayer()
    {
        if (_enemyMeleeAttack.isAttacking) return;

        Vector2 direction =
            (player.position - transform.position).normalized;

        if (isChargingAttack) return;

        rb.linearVelocity = direction * moveSpeed;

        animator.SetBool(IS_WALKING, true);

    }

    void Update()
    {
        if (isDodging)
        {
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, preferredDistance);
    }
}
