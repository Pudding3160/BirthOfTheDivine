using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRangedAI : MonoBehaviour
{
    private Rigidbody2D rb;

    private Animator animator;
    private bool isDead;

    [Header("References")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public GameObject meleeChampionPrefab;
    public Transform[] spawnPoints;

    [Header("Projectile Settings")]
    public float projectileSpeed = 10f;

    [Header("Attack Timings")]
    public float coneCooldown = 3f;
    public float burstCooldown = 5f;
    public float summonCooldown = 8f;

    [Header("Cone Attack")]
    public float coneAngle = 45f;

    private float lastConeTime;
    private float lastBurstTime;
    private float lastSummonTime;

    [Header("Movement")]
    public float burstSpeed = 12f;
    public float burstTime = 0.25f;

    [Header("Rapid Fire")]
    public float fireRate = 0.1f;
    public float rapidFireDuration = 2f;
    public float rapidProjectileSpeed = 10f;
    private bool isRapidFiring;
    public float aimSmoothSpeed = 3f;

    private Vector2 smoothedDirection;

    [Header("Orbit Settings")]
    public GameObject orbitPrefab;
    public int orbitCount = 5;
    public float radius = 2.5f;
    private float currentRadius;
    public float orbitSpeed = 2f;

    [Header("Burst Orbit Mode")]
    public float burstRadius = 5f;
    public float burstOrbitSpeed = 6f;
    public float burstDuration = 1.5f;

    private bool isBursting;

    [Header("Optional")]
    public bool rotateClockwise = true;

    private List<GameObject> orbs = new List<GameObject>();

    [Header("Cooldown")]
    public float abilityCooldown = 5f;
    private float lastAbilityTime;

    private bool isUsingAbility;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        SpawnOrbs();
        currentRadius = radius;
    }

    void Update()
    {

        if (player == null) return;

        Vector2 targetDir =
            (player.position - firePoint.position).normalized;

        smoothedDirection =
            Vector2.Lerp(
                smoothedDirection,
                targetDir,
                Time.deltaTime * (aimSmoothSpeed * 0.5f)
            );

        if (orbs.Count == 0) return;

        if (!isBursting && Time.time >= lastBurstTime + burstCooldown)
        {
            StartCoroutine(BurstOrbit());
            lastBurstTime = Time.time;
        }

        float targetRadius = isBursting ? burstRadius : radius;
        float currentSpeed = isBursting ? burstOrbitSpeed : orbitSpeed;

        currentRadius =
            Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * 5f);

        float direction = rotateClockwise ? -1f : 1f;

        for (int i = 0; i < orbs.Count; i++)
        {
            if (orbs[i] == null) continue;

            float angle =
                Time.time * currentSpeed * direction +
                (i * Mathf.PI * 2f / orbitCount);

            Vector2 offset =
                new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentRadius;

            orbs[i].transform.position =
                (Vector2)transform.position + offset;
        }

        if (Time.time >= lastConeTime + coneCooldown)
        {
            StartCoroutine(ConeAttack());
            lastConeTime = Time.time;
        }

        if (Time.time >= lastBurstTime + burstCooldown)
        {
            StartCoroutine(Burst360Attack());
            lastBurstTime = Time.time;
        }

        if (Time.time >= lastSummonTime + summonCooldown)
        {
            SummonMeleeChampions();
            lastSummonTime = Time.time;
        }

        if (isUsingAbility) return;

        if (Time.time >= lastAbilityTime + abilityCooldown)
        {
            StartCoroutine(BurstRapidFire());
            lastAbilityTime = Time.time;
        }
    }

    void SpawnOrbs()
    {
        for (int i = 0; i < orbitCount; i++)
        {
            float angle = i * Mathf.PI * 2f / orbitCount;

            Vector2 pos =
                (Vector2)transform.position +
                new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            GameObject orb = Instantiate(orbitPrefab, pos, Quaternion.identity);

            orbs.Add(orb);
        }
    }

    IEnumerator BurstOrbit()
    {
        isBursting = true;

        yield return new WaitForSeconds(burstDuration);

        isBursting = false;
    }

    // ================= BURST =================
    IEnumerator BurstRapidFire()
    {
        if (isUsingAbility) yield break;

        isUsingAbility = true;
        isRapidFiring = true;

        Vector2 dir = smoothedDirection;

        float chaseEndTime = Time.time + burstTime;

        while (Time.time < chaseEndTime)
        {
            rb.linearVelocity = dir * burstSpeed;
            RapidFireShot();
            yield return new WaitForSeconds(fireRate);
        }

        rb.linearVelocity = Vector2.zero;

        float lockEndTime = Time.time + rapidFireDuration;

        while (Time.time < lockEndTime)
        {
            RapidFireShot();
            yield return new WaitForSeconds(fireRate);
        }

        rb.linearVelocity = Vector2.zero;

        isRapidFiring = false;
        isUsingAbility = false;
    }

    // ================= RAPID FIRE =================
    void RapidFireShot()
    {
        if(!isRapidFiring) return;
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 baseDir =
            (player.position - firePoint.position).normalized;

        float baseAngle =
            Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        int bulletCount = Random.Range(3, 6);

        float spread = 25f;

        for (int i = 0; i < bulletCount; i++)
        {
            float t = (bulletCount == 1)
                ? 0
                : (i / (float)(bulletCount - 1)) * 2f - 1f;

            float angleOffset =
                t * spread + Random.Range(-3f, 3f);

            float finalAngle = baseAngle + angleOffset;

            Vector2 dir =
                new Vector2(
                    Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                    Mathf.Sin(finalAngle * Mathf.Deg2Rad)
                );

            GameObject proj = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

            Rigidbody2D prb = proj.GetComponent<Rigidbody2D>();

            if (prb != null)
            {
                prb.linearVelocity = dir * rapidProjectileSpeed;
            }

            proj.transform.rotation =
                Quaternion.Euler(0, 0, finalAngle);

            animator.SetBool("is_idle", true);
        }
    }

    // ================= CONE =================
    IEnumerator ConeAttack()
    {
        if (isDead) yield break;

        animator.SetBool("is_idle", false);
        //animator.SetTrigger("Cast");

        yield return new WaitForSeconds(0.4f);

        Vector2 baseDir =
            (player.position - firePoint.position).normalized;

        float angleStep = coneAngle / 2f;

        for (int i = -1; i <= 1; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

            SpawnProjectile(dir);
        }

        animator.SetBool("is_idle_anim", true);
    }

    // ================= 360 BURST =================
    IEnumerator Burst360Attack()
    {
        if (isDead) yield break;

        //animator.SetTrigger("Cast");

        yield return new WaitForSeconds(0.5f);

        int bullets = 8;

        for (int i = 0; i < bullets; i++)
        {
            float angle = i * (360f / bullets);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            SpawnProjectile(dir);
        }
    }

    // ================= SUMMON =================
    void SummonMeleeChampions()
    {
        if (meleeChampionPrefab == null || spawnPoints.Length == 0)
            return;

        int index = Random.Range(0, spawnPoints.Length);

        Instantiate(
            meleeChampionPrefab,
            spawnPoints[index].position,
            Quaternion.identity
        );
    }

    // ================= PROJECTILE =================
    void SpawnProjectile(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = direction.normalized * projectileSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}