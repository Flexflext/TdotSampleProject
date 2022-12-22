using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private Player_Combat playerCombat;

    public void Meele()
    {
        playerCombat.DoMeeleAttack();
    }

}
