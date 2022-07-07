using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private float runSpeed;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        capsuleCollider = transform.GetComponent<CapsuleCollider>();
        sphereCollider = transform.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        VerificaMovimento();
    }

    private void VerificaMovimento()
    {
        //capturando A,D
        var inputX = Input.GetAxis("Horizontal");
        //capturando W,S
        var inputZ = Input.GetAxis("Vertical");


        var direcao = new Vector3(inputX, 0, inputZ);

        if(inputX != 0 || inputZ != 0)
        {
            var camRot = Camera.main.transform.rotation;
            camRot.x = 0;
            camRot.z = 0;
            animator.SetBool("andarFrente", true);

            VerificaAndarOuCorrer();
            VerificaRolar();

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direcao) * camRot, 5 * Time.deltaTime);
        }
        else
        {
            animator.SetBool("andarFrente", false);
        }
    }

    private void VerificaAndarOuCorrer()
    {
        runSpeed = 1.3f * speed;

        //capturando shift
        var inputShift = Input.GetKey(KeyCode.LeftShift);

        //corrida (acontece mesmo andando, por isso o translate está aqui
        if (inputShift)
        {
            transform.Translate(0, 0, runSpeed * Time.deltaTime);
            animator.SetBool("correr", true);
        }
        else
        {
            animator.SetBool("correr", false);
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    private void VerificaRolar()
    {
        //capturando control
        var inputControl = Input.GetKeyDown(KeyCode.LeftControl);

        //rolagem
        if (inputControl)
        {
            animator.SetTrigger("rolar");
            transform.Translate(0, 0, speed * 1.2f * Time.deltaTime);
        }

        //desativando ou ativando os colliders de acordo com a necessidade
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward"))
        {
            sphereCollider.enabled = false;
            capsuleCollider.enabled = true;
        }
        else
        {
            //caso esteja rolando, ativa um collider menor
            sphereCollider.enabled = true;
            capsuleCollider.enabled = false;
        }
    }

}
