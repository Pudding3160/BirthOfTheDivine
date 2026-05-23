using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;

    private Animator animator;
    private const string IS_IDLE = "is_idle";
    private const string IS_WALKING = "is_walking_anim";
    private const string IS_ATTACKING = "is_attacking_anim";

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 10f;
    public float chargeSpeed = 7f;

    [Header("Ranges")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Attack Timing")]
    public float attackCooldown = 2f;

    public bool isAttacking;
    public bool jumpAttack;
    public bool slashAttack;
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float dist = Vector2.Distance(transform.position, player.position);
        //Debug.Log(dist);

        if (dist > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(DoAttackDecision());
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator DoAttackDecision()
    {
        if (isAttacking)
            yield break;

        isAttacking = true;
        rb.linearVelocity = Vector2.zero;


        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > 7f)
        {
            if (Random.value > 0.5f)
                yield return StartCoroutine(JumpToPositionAOE());
            else
                yield return StartCoroutine(DashSlashThrough());
        }
        else if (dist > 4f)
        {
            if (Random.value > 0.5f)
                yield return StartCoroutine(ChargeDashHeavy());
            else
                yield return StartCoroutine(RunChargeHeavy());
        }
        else if (dist > 2f)
        {
            yield return StartCoroutine(ChargeDashHeavy());
        }
        else
        {
            yield return StartCoroutine(SmallWalkAttack());
        }

        lastAttackTime = Time.time;

    }

    // 1. charge -> dash -> heavy attack
    IEnumerator ChargeDashHeavy()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool(IS_WALKING, false);

        animator.SetTrigger("ChargeAttack");

        yield return new WaitForSeconds(1f);


        animator.SetBool(IS_WALKING, true);

        Vector2 dir = (player.position - transform.position).normalized;

        float dashDistance = 5f;
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + dir * dashDistance;

        float dashTime = 0.2f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dashTime;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1.5f);

        isAttacking = false;
    }

    // 2. dash through player
    IEnumerator DashSlashThrough()
    {
        isAttacking = true;
        slashAttack = true;

        animator.SetBool(IS_WALKING, false);
        animator.SetTrigger("ChargeAttack");

        Vector2 dir = (player.position - transform.position).normalized;

        float dashDistance = 3f;
        Vector2 startPos = transform.position;
        Vector2 targetPos = (Vector2)player.position + dir * dashDistance;

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.15f);

        float t = 0f;
        float duration = 0.25f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("HeavyHit");

        yield return new WaitForSeconds(0.15f);

        animator.SetBool(IS_WALKING, true);

        slashAttack = false;
        isAttacking = false;
    }

    // 3. walk to player -> small attack
    IEnumerator SmallWalkAttack()
    {
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool(IS_WALKING, false);

        yield return new WaitForSeconds(0.15f);

        animator.SetTrigger("Attack");

        yield return null;

        float initialDist = Vector2.Distance(transform.position, player.position);

        bool shouldChase = initialDist > attackRange;

        if (shouldChase)
        {
            while (isAttacking)
            {
                float dist = Vector2.Distance(transform.position, player.position);

                if (dist <= attackRange)
                    break;

                Vector2 dir = (player.position - transform.position).normalized;
                rb.linearVelocity = dir * walkSpeed;

                yield return null;
            }
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    // 4. run -> charge -> heavy attack
    IEnumerator RunChargeHeavy()
    {
        isAttacking = true;
        slashAttack = true;


        Vector2 targetPos = player.position;

        float stopDistance = 0.5f;

        float sprintSpeed = 14f;

        animator.SetBool(IS_WALKING, true);

        while (Vector2.Distance(transform.position, targetPos) > stopDistance)
        {
            Vector2 dir =
                (targetPos - (Vector2)transform.position).normalized;

            rb.MovePosition(
                Vector2.MoveTowards(
                    rb.position,
                    targetPos,
                    sprintSpeed * Time.fixedDeltaTime
                )
            );

            yield return new WaitForFixedUpdate();
        }

        animator.SetBool(IS_WALKING, false);
        animator.SetTrigger("ChargeAttack");

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        animator.SetTrigger("HeavyHit");

        Vector2 attackDir =
            (player.position - transform.position).normalized;

        rb.linearVelocity = attackDir * chargeSpeed;

        yield return new WaitForSeconds(0.25f);

        rb.linearVelocity = Vector2.zero;

        animator.SetBool(IS_WALKING, true);

        yield return new WaitForSeconds(0.6f);

        slashAttack = false;
        isAttacking = false;
    }

    // 5. jump to position aoe
    IEnumerator JumpToPositionAOE()
    {
        isAttacking = true;

        animator.SetBool(IS_WALKING, false);
        animator.SetTrigger("ChargeAttack");

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(2f);

        float duration = 0.6f;
        float t = 0f;

        Vector2 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            Vector2 targetPos = player.position;

            Vector2 flatPos = Vector2.Lerp(startPos, targetPos, t);

            float height = 2f;
            float arc = Mathf.Sin(t * Mathf.PI) * height;

            transform.position = new Vector2(flatPos.x, flatPos.y + arc);

            yield return null;
        }

        transform.position = player.position;
        jumpAttack = true;

        yield return new WaitForSeconds(0.4f);

        animator.SetBool(IS_WALKING, true);

        jumpAttack = false;
        isAttacking = false;
    }
}
