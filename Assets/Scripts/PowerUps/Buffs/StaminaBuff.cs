using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/StaminaBuff")]
public class StaminaBuff : PowerUpEffect
{
    public float amount;

    public override void Aplicar(GameObject target)
    {
        //criar um script separado para health e stamina ?
        target.GetComponent<PlayerController>().stamina += amount;
    }
}
