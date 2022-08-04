using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float speed;
    public float throwForce;
    public float health;
    public float stamina;

    [Header("Referências")]
    public Image staminaFill;
    public Image healthFill;

    private bool canHold = true;
    private bool reloadingStamina;
    private const float totalHealth = 5f;
    private float runSpeed;
    public float staminaDecay;
    private const float rollStaminaCost = 25f;
    private Color staminaFillColor;
    private Color healthFillColor;

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private GameObject rightHand;

    private Bola ballReference;
    
    // Start is called before the first frame update
    void Start()
    {
        //capturando componentes
        animator = transform.GetComponent<Animator>();
        capsuleCollider = transform.GetComponent<CapsuleCollider>();
        sphereCollider = transform.GetComponent<SphereCollider>();
        rightHand = gameObject.transform
            .Find("Armature")
            .Find("Root_M")
            .Find("Spine1_M")
            .Find("Spine2_M")
            .Find("Chest_M")
            .Find("Scapula_R")
            .Find("Shoulder_R")
            .Find("Elbow_R")
            .Find("Wrist_R")
            .Find("jointItemR")
            .Find("ballReference")
            .gameObject;

        stamina = 100;

        staminaFillColor = staminaFill.color;
        healthFillColor = healthFill.color;
        health = totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPlaying)
            VerificaMovimento();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.isPlaying)
            VerificaArremesso();
    }

    private void LateUpdate()
    {
        AtualizarStamina();
        AtualizarHealth();
    }

    private void AtualizarStamina()
    {
        // se a stamina chega a zero, não deixa correr até que recarregue pelo menos 15
        if (stamina == 0)
            reloadingStamina = true;

        if (stamina > 15)
            reloadingStamina = false;

        // ajustando o UI da imagem conforme a stamina
        stamina += Time.deltaTime * 10;
        stamina = Mathf.Clamp(stamina, 0, 100);
        staminaFill.fillAmount = stamina / 100.0f;

        if (stamina <= 15)
            staminaFill.color = Color.red;
        else
            staminaFill.color = staminaFillColor;
    }

    private void AtualizarHealth()
    {
        // ajustando o UI da imagem conforme a saúde
        healthFill.fillAmount = health/totalHealth;

        if (health <= 1)
            healthFill.color = Color.red;
        else
            healthFill.color = healthFillColor;
    }

    private void VerificaArremesso()
    {
        //arremessa com o botão esquerdo do mouse
        if (Input.GetMouseButton(0) && ballReference != null)
        {
            ////direção do mouse
            //var direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            //var y = direction.y * 2f;
            var direction = transform.forward;
            direction.z *= throwForce;
            direction.x *= throwForce;
            direction.y = 0.4f;

            ballReference.Arremessar(direction);
            //disponibilizando para pegar novas bolas
            canHold = true;
            ballReference = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //verificando se é uma bola
        if (other.transform.CompareTag("Ball"))
        {
            var ball = other.GetComponent<Bola>();

            //verificando se pode pegara
            if (canHold && ball.canHold)
            {
                //componente de script
                ballReference = ball;
                ball.Capturar(rightHand, gameObject);
                canHold = false;
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // verificando se uma bola colidiu
        if (collision.transform.CompareTag("Ball")) {
            
            var ball = collision.transform.GetComponent<Bola>();

            //verificando se a bola pode causar dano e se não foi o próprio jogador que atirou
            if (ball.canDamage && ball.whoThrows != null && ball.whoThrows.gameObject != this.gameObject)
            {
                health--;

                if (health > 0)
                {
                    //limpa as animações em execução
                    animator.Rebind();
                    animator.SetTrigger("atingido");
                }
                else
                {
                    //desativa colliders e ativa animação de morte
                    animator.SetTrigger("morte");
                    sphereCollider.enabled = false;
                    capsuleCollider.enabled = false;
                    //termina o jogo
                    GameManager.Instance.isPlaying = false;
                }
            }
            else if (canHold && ball.canHold)
            {
                //componente de script
                ballReference = ball;
                ball.Capturar(rightHand, gameObject);
                canHold = false;
            }
        }
    }

    private void VerificaMovimento()
    {
        //capturando A,D
        var inputX = Input.GetAxis("Horizontal");
        //capturando W,S
        var inputZ = Input.GetAxis("Vertical");

        var direcao = new Vector3(inputX, 0, inputZ);

        if((inputX != 0 || inputZ != 0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
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
        if (inputShift && stamina > 0 && !reloadingStamina)
        {
            //diminuindo stamina
            stamina -= (staminaDecay * Time.deltaTime);
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
        if (stamina >= rollStaminaCost && inputControl && (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward")))
        {
            animator.SetTrigger("rolar");
            transform.Translate(0, 0, speed * 1.2f * Time.deltaTime);
            stamina -= rollStaminaCost;
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

    //pode ser útil para verificar se alguma animação está rodando
    //public IEnumerator CheckAnimationCompleted(string CurrentAnim, Action Oncomplete)
    //{
    //    while (!animator.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnim))
    //        yield return null;
    //    if (Oncomplete != null)
    //        Oncomplete();
    //}

}
