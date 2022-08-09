using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float speed;
    public float throwForce;
    public float health;
    public float stamina;
    public float staminaDecay;
    public float cameraShakeDuration;
    public float cameraShakeForce;

    [Header("Referências")]
    private Image staminaFill;
    private Image healthFill;
    public Animation dancingAnimation;

    [Header("Config. de aúdio")]
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip gettingHitClip;
    public AudioClip walkingClip;
    public AudioClip dyingClip;
    public AudioClip rollClip;

    private bool canHold = true;
    private float lastDamageTime;
    private bool reloadingStamina;
    private const float totalHealth = 5f;
    private float runSpeed;
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
        audioSource = transform.GetComponent<AudioSource>();
        staminaFill = transform.Find("Canvas").Find("staminaFill").GetComponent<Image>();
        healthFill = transform.Find("Canvas").Find("healthFill").GetComponent<Image>();

        ProcurarReferenciaMao(gameObject.transform);

        stamina = 100;

        lastDamageTime = Time.timeSinceLevelLoad;
        staminaFillColor = staminaFill.color;
        healthFillColor = healthFill.color;
        health = totalHealth;

        transform.GetComponent<Animator>().Play("Idle");
        sphereCollider.enabled = false;
        capsuleCollider.enabled = true;

        transform.Find("Canvas").gameObject.SetActive(true);

    }

    public void Reset()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.isPlaying)
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
        if (collision.transform.CompareTag("Ball"))
        {

            var ball = collision.transform.GetComponent<Bola>();

            //verificando se a bola pode causar dano e se não foi o próprio jogador que atirou e se o último dano foi a mais de 1 segundo
            if (ball.canDamage && ball.whoThrows != null && ball.whoThrows.gameObject != this.gameObject && Time.timeSinceLevelLoad - lastDamageTime > 1f)
            {
                lastDamageTime = Time.timeSinceLevelLoad;
                health--;
                // aúdio do dano
                audioSource.PlayOneShot(gettingHitClip);

                if (health > 0)
                {
                    //limpa as animações em execução
                    animator.Rebind();
                    animator.SetTrigger("atingido");
                    //balança a camera
                    Camera.main.DOShakeRotation(cameraShakeDuration, cameraShakeForce, fadeOut: true);
                }
                else
                {
                    // aúdio de morte
                    audioSource.PlayOneShot(dyingClip);

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


    /// <summary>
    /// Busca a referência do objeto ballReference, o qual aloja a bola quando capturada
    /// </summary>
    /// <param name="parent"></param>
    void ProcurarReferenciaMao(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "ballReference")
                rightHand = child.gameObject;
            else
                ProcurarReferenciaMao(child);
        }
    }

    /// <summary>
    /// Atualiza a UI e os valores da Stamina
    /// </summary>
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

    /// <summary>
    /// Atualiza a UI da vida do player
    /// </summary>
    private void AtualizarHealth()
    {
        // ajustando o UI da imagem conforme a saúde
        healthFill.fillAmount = health/totalHealth;

        if (health <= 1)
            healthFill.color = Color.red;
        else
            healthFill.color = healthFillColor;
    }

    /// <summary>
    /// Verifica a possibilidade de arremessar a bola quando os botões forem precionados
    /// </summary>
    private void VerificaArremesso()
    {
        //arremessa com o botão fire ou espaço
        if ((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)) && ballReference != null)
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

    /// <summary>
    /// Analisa se as entradas para movimentar o personagem
    /// </summary>
    private void VerificaMovimento()
    {
        //capturando A,D
        var inputX = Input.GetAxis("Horizontal");
        //capturando W,S
        var inputZ = Input.GetAxis("Vertical");

        var direcao = new Vector3(inputX, 0, inputZ);

        if((inputX != 0 || inputZ != 0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            // caso o som de andar não esteja ativo, ativa ele
            if(audioSource.clip != walkingClip)
            {
                audioSource.clip = walkingClip;
                audioSource.loop = true;
                audioSource.Play();
            }

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
            // se parou de andar, para o audioclip
            if (audioSource.clip == walkingClip)
                audioSource.clip = null;

            animator.SetBool("andarFrente", false);
        }
    }

    /// <summary>
    /// Corre caso SHIFT seja pressionado
    /// </summary>
    private void VerificaAndarOuCorrer()
    {
        runSpeed = 1.3f * speed;

        //capturando shift
        var inputShift = Input.GetKey(KeyCode.LeftShift);

        //corrida (acontece mesmo andando, por isso o translate está aqui
        if (inputShift && stamina > 0 && !reloadingStamina)
        {
            // acelerando o audio clip para simular corrida
            if (audioSource.clip == walkingClip)
                audioSource.pitch = 1.2f;

            //diminuindo stamina
            stamina -= (staminaDecay * Time.deltaTime);
            transform.Translate(0, 0, runSpeed * Time.deltaTime);
            animator.SetBool("correr", true);
        }
        else
        {
            // desacelerando o audio clip 
            if (audioSource.clip == walkingClip)
                audioSource.pitch = 1;

            animator.SetBool("correr", false);
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Rolar caso CTRL seja pressionado
    /// </summary>
    private void VerificaRolar()
    {
        //capturando control
        var inputControl = Input.GetKeyDown(KeyCode.LeftControl);

        //rolagem
        if (stamina >= rollStaminaCost && inputControl && (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward")))
        {
            audioSource.PlayOneShot(rollClip);

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
}
