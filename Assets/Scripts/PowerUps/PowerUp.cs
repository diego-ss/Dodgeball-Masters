using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect powerUpEffect;

    private AudioSource audioSource;
    private float startLifeTime;
    
    private void Start()
    {
        startLifeTime = Time.timeSinceLevelLoad;
        audioSource = GetComponent<AudioSource>();
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
            audioSource.Play();
            Destroy(gameObject, 0.2f);
            powerUpEffect.Aplicar(other.gameObject);
        }

    }
}
