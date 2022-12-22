using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player_Combat : MonoBehaviour
{
    [Header("Shooting")] 
    [SerializeField] private float timeBetweenAttacks;
    private bool coolDown;
    private float curTime;

    [Header("Meele")] 
    [SerializeField] private LayerMask layersToIgnore;
    [SerializeField] private int meeleDamage;
    [SerializeField] private float attackRadius;
    [SerializeField] [Range(0,1)] private float moveSpeedMultiplier;
    [SerializeField] private float timeBetweenMeeleAttacks;
    private bool meeleCoolDown;
    private float curMeeleTime;
    
    private float curSpeedMultiplier = 1;


    [Header("References")]
    [SerializeField] private Transform gfxParent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform shotPosition;
    [SerializeField] private GameObject bulletPrefab;
    private Rigidbody2D rb;
    private PlayerController playerController;

    private Coroutine shootingCoroutine;
    private Coroutine meeleCoroutine;

    private bool canAttack => !playerController.isDashing && !playerController.canWallHang && !coolDown && !meeleCoolDown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        InitializeInput();
    }

    private void Update()
    {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;

            if (curTime <= 0)
            {
                coolDown = false;
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

        rb.velocity =  new Vector2(rb.velocity.x * curSpeedMultiplier, rb.velocity.y);
    }

    private void InitializeInput()
    {
        InputActionMap map = GetComponent<PlayerInput>().currentActionMap;
        map.FindAction("Fire").performed += OnFire;
        map.FindAction("Meele").performed += OnMeele;
    }

    private void OnFire(CallbackContext _ctx)
    {
        if (!canAttack) return;
        if(shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        coolDown = true;
        curTime = timeBetweenAttacks;
        
        animator.SetBool("Shooting", true);
        
        Projectile bullet = Instantiate(bulletPrefab, shotPosition.position, Quaternion.identity).GetComponent<Projectile>();

        bullet.Launch(gfxParent.right,10, layersToIgnore);

        shootingCoroutine = StartCoroutine(C_WaitTillStanceSwitch());
        
    }

    private void OnMeele(CallbackContext _ctx)
    {
        if (!canAttack) return;
        if (meeleCoroutine != null)
        {
            StopCoroutine(meeleCoroutine);
        }

        meeleCoolDown = true;
        curMeeleTime = timeBetweenMeeleAttacks;
        
        curSpeedMultiplier = moveSpeedMultiplier;
        
        animator.SetBool("Meele", true);
        meeleCoroutine = StartCoroutine(C_WaitTillMeeleStop());
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
        yield return new WaitForSeconds(0.3f);
        DoMeeleAttack();
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Meele", false);
        curSpeedMultiplier = 1;
    }
}
