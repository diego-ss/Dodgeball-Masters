using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private static AudioSource sceneEffectsAudioSource;

    public PowerUpEffect powerUpEffect;
    public AudioClip catchedClip;

    private float startLifeTime;
    
    private void Start()
    {
        startLifeTime = Time.timeSinceLevelLoad;

        if (sceneEffectsAudioSource == null)
            sceneEffectsAudioSource = GameObject.Find("EffectsAudioSource").GetComponent<AudioSource>();
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
            sceneEffectsAudioSource.PlayOneShot(catchedClip);
            Destroy(gameObject);
            powerUpEffect.Aplicar(other.gameObject);
        }

    }
}
