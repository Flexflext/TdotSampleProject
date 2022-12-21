using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player_Combat : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Transform gfxParent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform shotPosition;
    [SerializeField] private GameObject bulletPrefab;


    private void Start()
    {
        InitializeInput();
    }

    private void InitializeInput()
    {
        InputActionMap map = GetComponent<PlayerInput>().currentActionMap;

        map.FindAction("Fire").performed += OnAttack;
        //map.FindAction("Fire").canceled += OnAttack;
    }

    private void OnAttack(CallbackContext _ctx)
    {
        animator.SetBool("Shooting", true);

        Debug.Log("HUHU");
        Projectile bullet = Instantiate(bulletPrefab, shotPosition.position, Quaternion.identity).GetComponent<Projectile>();

        bullet.Launch(gfxParent.right,10, LayerMask.NameToLayer("Player"));

        StopAllCoroutines();
        StartCoroutine(C_WaitTillSwitch());
        
    }

    private IEnumerator C_WaitTillSwitch()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Shooting", false);
    }
}
