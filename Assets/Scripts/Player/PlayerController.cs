using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        VerificaMovimento();
    }

    private void VerificaMovimento()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputZ = Input.GetAxis("Vertical");
        var inputShift = Input.GetKey(KeyCode.LeftShift);
        var inputControl = Input.GetKeyDown(KeyCode.LeftControl);
        var direcao = new Vector3(inputX, 0, inputZ);

        if(inputX != 0 || inputZ != 0)
        {
            var camRot = Camera.main.transform.rotation;
            camRot.x = 0;
            camRot.z = 0;
            animator.SetBool("andarFrente", true);

            if (inputShift)
            {
                transform.Translate(0, 0, speed * 1.3f * Time.deltaTime);
                animator.SetBool("correr", true);
            }
            else
            {
                animator.SetBool("correr", false);
                transform.Translate(0, 0, speed * Time.deltaTime);
            }

            if (inputControl)
            {
                animator.SetTrigger("rolar");
                transform.Translate(0, 0, speed * 1.2f * Time.deltaTime);
            }


            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direcao) * camRot, 5 * Time.deltaTime);
        }
        else
        {
            animator.SetBool("andarFrente", false);

        }
    }

}
