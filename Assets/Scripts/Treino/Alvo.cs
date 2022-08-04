using UnityEngine;

public class Alvo : MonoBehaviour
{
    private Animator animator;
    private bool died;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //detectando quando bola atinge
        if (collision.transform.CompareTag("Ball"))
        {
            var ballScript = collision.transform.GetComponent<Bola>();

            //se a bola pode atingir, ativa animação de morte
            if (ballScript.canDamage)
            {
                audioSource.Play();
                animator.SetTrigger("die");

                if(!died)
                    GameManager.Instance.trainingScore++;

                died = true;
            }

            ballScript.canHold = true;
        }
    }
}
