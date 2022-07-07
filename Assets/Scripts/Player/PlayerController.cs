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
        var direcao = new Vector3(inputX, 0, inputZ);

        if(inputX != 0 || inputZ != 0)
        {
            var camRot = Camera.main.transform.rotation;
            camRot.x = 0;
            camRot.z = 0;
            transform.Translate(0, 0, speed * Time.deltaTime);
            animator.SetBool("andarFrente", true);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direcao) * camRot, 5 * Time.deltaTime);
        } else
        {
            animator.SetBool("andarFrente", false);

        }
    }

}
