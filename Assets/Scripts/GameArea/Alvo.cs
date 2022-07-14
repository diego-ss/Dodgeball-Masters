using UnityEngine;

public class Alvo : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //detectando quando bola atinge
        if (collision.transform.CompareTag("Ball"))
        {
            var ballScript = collision.transform.GetComponent<Bola>();

            //se a bola pode atingir, ativa animação de morte
            if (ballScript.canDamage)
                animator.SetTrigger("die");

            ballScript.canHold = true;
        }
    }
}
