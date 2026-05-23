using System;
using Bullets;
using Events;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    [SerializeField]
    private BulletPooling bulletPooling;
    public float movementSpeed;
    private Vector2 moveDirection;
    [SerializeField]
    private float invincibleTime;
    [HideInInspector]
    public float invincibleTimeBuffer;

    [Header("Attacking")] 
    [SerializeField] 
    private float attackRate;
    private float attackRateBuffer;
    private bool isFiring;
    private Vector2 shootDirection;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private float lastDashTime;
    private bool isDashing;

    private TrailRenderer dashTrail;

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletPooling = GetComponent<BulletPooling>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        dashTrail=GetComponent<TrailRenderer>();    
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        animator = GetComponent<Animator>(); 

        GameEventManager.Instance.inputEvents.MovePressed += UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed += Attack;
        GameEventManager.Instance.levelEvents.LevelTimerFinished += DisableControlsOnLevelTimerEnd;
        GameEventManager.Instance.sceneEvents.SceneLoaded += EnableControlsOnSceneChanged;
        
        //invincibleTime = PlayerStatManager.Instance.invincibilityTimer;
        movementSpeed = PlayerStatManager.Instance.speed;
        attackRate = PlayerStatManager.Instance.fireRate;
        
        invincibleTimeBuffer = invincibleTime;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.inputEvents.MovePressed -= UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed -= Attack;
        GameEventManager.Instance.levelEvents.LevelTimerFinished -= DisableControlsOnLevelTimerEnd;
        GameEventManager.Instance.sceneEvents.SceneLoaded -= EnableControlsOnSceneChanged;
        if (dashTrail) dashTrail.Clear();

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void UpdatePlayerMoveDirection(InputAction.CallbackContext direction)
    {
        moveDirection = direction.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (isDashing) return;

        rb.MovePosition(rb.position + moveDirection * (movementSpeed * Time.fixedDeltaTime));
    }

    private void Update()
    {
        UpdateAnimations();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time >= lastDashTime + dashCooldown)
            {
                StartCoroutine(Dash());
                StartCoroutine(DashTrailCrt()); 
            }
        }

        invincibleTimeBuffer -= Time.deltaTime;
        attackRateBuffer -= Time.deltaTime;

        if (isFiring) Shoot();
    }

    public void MakeInvincible()
    {
        invincibleTimeBuffer = invincibleTime;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        shootDirection = context.ReadValue<Vector2>();
        isFiring = shootDirection.magnitude > .1f;
    }

    public void Shoot()
    {
        // Can't shoot yet
        if (attackRateBuffer > 0) return;
        var bullet = bulletPooling.GetPooledObject();
        // Failsafe
        if (!bullet) return;
        bullet.Initialize(transform.position, shootDirection);
        GetComponent<ParticleThingo>()?.SpawnParticle();
        attackRateBuffer = attackRate;
    }

    private void DisableControlsOnLevelTimerEnd()
    {
        GameEventManager.Instance.inputEvents.MovePressed -= UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed -= Attack;
        moveDirection = Vector2.zero;
        isFiring = false;
    }

    private void EnableControlsOnSceneChanged()
    {
        GameEventManager.Instance.inputEvents.MovePressed += UpdatePlayerMoveDirection;
        GameEventManager.Instance.inputEvents.AttackPressed += Attack;
        transform.position = new Vector3(0, 0, transform.position.z);
    }
    IEnumerator DashTrailCrt()
    {
        if (dashTrail) dashTrail.enabled = true;
        var elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;

            yield return new WaitForSeconds(0.25f);
        }
        if (dashTrail) dashTrail.enabled = false;
       
    }
    
    private void UpdateAnimations()
    {
        var speed = moveDirection.magnitude;

        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveY", moveDirection.y);
        animator.SetFloat("Speed", speed);

        if (speed < 0.1f)
            animator.SetFloat("Speed", 0);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        
        lastDashTime = Time.time;

        var inputDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (inputDir == Vector2.zero)
            inputDir = moveDirection;

        var elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;

            rb.MovePosition(rb.position + inputDir * (dashSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }
        
        isDashing = false;
    }
}
