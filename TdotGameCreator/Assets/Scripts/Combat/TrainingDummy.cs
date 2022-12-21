using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour, IDamagable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void GetDamage(int _value, Vector3 _knockbackDir)
    {
        anim.SetTrigger("Damage");
    }
}
