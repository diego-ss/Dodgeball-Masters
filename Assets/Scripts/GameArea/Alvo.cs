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
        if (collision.transform.CompareTag("Ball"))
        {
            var ballScript = collision.transform.GetComponent<Bola>();

            if (ballScript.canDamage)
                animator.SetTrigger("die");
        }
    }
}
