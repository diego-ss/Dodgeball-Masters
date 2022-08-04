using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/HealthBuff")]
public class HealthBuff : PowerUpEffect
{
    public float amount;

    public override void Aplicar(GameObject target)
    {
        //criar um script separado para health e stamina ?
        target.GetComponent<PlayerController>().health += amount;
    }
}
