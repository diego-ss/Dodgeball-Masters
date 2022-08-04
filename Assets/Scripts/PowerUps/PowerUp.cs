using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect powerUpEffect;
    private float startLifeTime;
    
    private void Start()
    {
        startLifeTime = Time.timeSinceLevelLoad;    
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad - startLifeTime > 7f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            powerUpEffect.Aplicar(other.gameObject);
        }

    }
}
