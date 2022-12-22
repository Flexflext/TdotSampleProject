using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static UnityEngine.InputSystem.InputAction;

public class Player_Combat : MonoBehaviour
{
    [Header("Shooting")] 
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private int maxBullets;
    [SerializeField] private bool regenerateBullets;
    [SerializeField] private float timeBetweenBulletRegen;
    [SerializeField] private int curBullets;
    private bool coolDown;
    private float curTime;
    private float curBulletTime;
    private bool fireHold;

    [Header("Meele")] 
    [SerializeField] private LayerMask layersToIgnore;
    [SerializeField] private int meeleDamage;
    [SerializeField] private float attackRadius;
    [SerializeField] [Range(0,1)] private float moveSpeedMultiplier;
    [SerializeField] private float timeBetweenMeeleAttacks;
    private bool meeleCoolDown;
    private float curMeeleTime;
    private bool meeleHold;
    
    private float curSpeedMultiplier = 1;


    [Header("References")]
    [SerializeField] private Transform gfxParent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform shotPosition;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private VisualEffect muzzleFlash;
    private Rigidbody2D rb;
    private PlayerController playerController;

    private CollisionCheck cc;


    private Coroutine shootingCoroutine;
    private Coroutine meeleCoroutine;

    private bool canAttack => !playerController.isDashing && !playerController.canWallHang && !coolDown && !meeleCoolDown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CollisionCheck>();
        playerController = GetComponent<PlayerController>();
        InitializeInput();
        curBullets = maxBullets;
        curBulletTime = timeBetweenBulletRegen;
    }

    private void Update()
    {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;

            if (curTime <= 0)
            {
                coolDown = false;

                if (fireHold)
                {
                    OnFire(new CallbackContext());
                }
            }
        }
        
        if (curMeeleTime > 0)
        {
            curMeeleTime -= Time.deltaTime;

            if (curMeeleTime <= 0)
            {
                meeleCoolDown = false;
            }
        }

        if (regenerateBullets)
        {
            if (curBulletTime > 0)
            {
                curBulletTime -= Time.deltaTime;

                if (curBulletTime <= 0)
                {
                    curBulletTime = timeBetweenBulletRegen;
                    if (curBullets < maxBullets)
                    {
                        curBullets++;
                    }
                }
            }
        }

        

        rb.velocity =  new Vector2(rb.velocity.x * curSpeedMultiplier, rb.velocity.y);

        if (meeleHold)
        {
            OnMeele(new CallbackContext());
        }
    }

    private void InitializeInput()
    {
        InputActionMap map = GetComponent<PlayerInput>().currentActionMap;

        map.FindAction("Fire").performed += OnFire;
        map.FindAction("Fire").canceled += OnFireCanceled;

        map.FindAction("Meele").performed += OnMeele;
        map.FindAction("Meele").canceled += OnMeeleCancled;
    }

    private void OnFire(CallbackContext _ctx)
    {
        if (curBullets <= 0) return;
        if (!canAttack) return;

        if(shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        fireHold = true;
        coolDown = true;
        curTime = timeBetweenAttacks;

        curBullets--;

        muzzleFlash.Play();

        animator.SetBool("Shooting", true);
        
        Projectile bullet = Instantiate(bulletPrefab, shotPosition.position, Quaternion.identity).GetComponent<Projectile>();

        bullet.Launch(gfxParent.right,10, layersToIgnore);

        shootingCoroutine = StartCoroutine(C_WaitTillStanceSwitch());
        
    }

    private void OnFireCanceled(CallbackContext _ctx)
    {
        fireHold = false;
    }


    private void OnMeele(CallbackContext _ctx)
    {
        if (!canAttack) return;
        if (meeleCoroutine != null)
        {
            StopCoroutine(meeleCoroutine);
        }

        meeleHold = true;

        meeleCoolDown = true;
        curMeeleTime = timeBetweenMeeleAttacks;

        if (cc.m_IsGrounded)
        {
            curSpeedMultiplier = moveSpeedMultiplier;
        }
        
        animator.SetBool("Meele", true);
        meeleCoroutine = StartCoroutine(C_WaitTillMeeleStop());
    }

    private void OnMeeleCancled(CallbackContext _ctx)
    {
        meeleHold = false;
    }

    public void DoMeeleAttack()
    {
        Collider2D[] damageObjs = Physics2D.OverlapCircleAll(shotPosition.position, attackRadius, ~(1 << 6));

        for (int i = 0; i < damageObjs.Length; i++)
        {
            IDamagable damage = damageObjs[i].GetComponent<IDamagable>();
            
            if (damage != null)
            {
                damage.GetDamage(meeleDamage, gfxParent.right);
            }
        }
    }

    private IEnumerator C_WaitTillStanceSwitch()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Shooting", false);
    }
    
    private IEnumerator C_WaitTillMeeleStop()
    {
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Meele", false);
        curSpeedMultiplier = 1;
    }
}
