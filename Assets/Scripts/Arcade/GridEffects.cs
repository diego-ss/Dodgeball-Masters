using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEffects : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume -= Time.deltaTime * 0.2f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
            audioSource.volume = 1f * rb.velocity.magnitude;
        else
            audioSource.volume = 1f;
        
        audioSource.Play();
    }
}
