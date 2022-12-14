using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerHealth : Health
{
    [SerializeField] private ScriptableInt m_PlayerHealth;
    [SerializeField] private Rigidbody2D m_Rb;
    [SerializeField] private float m_KnockbackForce;
    [SerializeField] private VisualEffect m_BloodParticles;
    [SerializeField] private VisualEffect m_DespawnParticles;

    public override void AddHP(int _amount)
    {
        m_PlayerHealth.Value += _amount;

        m_PlayerHealth.Value = Mathf.Clamp(m_PlayerHealth.Value, 0, MaxHP);
    }
    public override void GetDamage(int _value, Vector3 _knockbackDir)
    {
        //CameraManager.Instance.Shake();

        float defaultX = Mathf.Sign(_knockbackDir.x);
        if (defaultX > 0)
            defaultX = 1;
        else if (defaultX < 0)
            defaultX = -1;

        // play blood particles
        m_BloodParticles.SetVector3("Force", new Vector3((_knockbackDir.normalized.x + defaultX) * 5f, 2f, 2f));
        m_BloodParticles.Play();

        // play sound
        //SoundManager.Instance.PlaySound(SoundManager.Instance.HitHumanoidSound);

        // Do knockback
        m_Rb.AddForce(_knockbackDir * m_KnockbackForce, ForceMode2D.Impulse);

        // set health 
        m_PlayerHealth.Value -= _value;



        if (m_PlayerHealth.Value <= 0)
        {
            m_DespawnParticles.transform.SetParent(null);
            m_BloodParticles.transform.SetParent(null);
            m_DespawnParticles.Play();
            E_TriggerDeath?.Invoke();
        }
    }
}
