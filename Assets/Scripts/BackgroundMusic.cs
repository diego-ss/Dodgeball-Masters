using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;

    [Header("Músicas de fundo")]
    public AudioClip[] audioClips;
    public AudioClip currentClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            //atualiza o current clip com uma nova música
            PegarMusicaAleatoria();
            audioSource.PlayOneShot(currentClip);

        }
    }

    void PegarMusicaAleatoria()
    {
        var list = audioClips.ToList();

        if (currentClip != null)
            list = list.Where(m => m != currentClip).ToList();

        var randomIndex = Random.Range(0, list.Count);
        currentClip = list[randomIndex];
    }
}
