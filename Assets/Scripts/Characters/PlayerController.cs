using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float speed;
    public float throwForce;
    public float cameraShakeDuration;
    public float cameraShakeForce;
    public float rollStaminaCost;

    [Header("Config. de aúdio")]
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip gettingHitClip;
    public AudioClip walkingClip;
    public AudioClip dyingClip;
    public AudioClip rollClip;

    private bool canHold = true;
    private float lastDamageTime;
    private float runSpeed;
    private Vector3 throwDirection;

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private GameObject rightHand;
    private Health health;
    private Stamina stamina;

    private Bola ballReference;
    
    // Start is called before the first frame update
    void Awake()
    {
        //capturando componentes
        animator = transform.GetComponent<Animator>();
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
        capsuleCollider = transform.GetComponent<CapsuleCollider>();
        sphereCollider = transform.GetComponent<SphereCollider>();
        audioSource = transform.GetComponent<AudioSource>();

        ProcurarReferenciaMao(gameObject.transform);

        ConfiguracoesIniciais();
    }
    
    private void ConfiguracoesIniciais()
    {
        sphereCollider.enabled = false;
        capsuleCollider.enabled = true;
        canHold = true;
        ballReference = null;
        lastDamageTime = Time.timeSinceLevelLoad;

        transform.GetComponent<Animator>().Play("Idle");
    }

    public void Reset()
    {
        ConfiguracoesIniciais();
        transform.Find("Canvas").gameObject.SetActive(true);
        health.Resetar();
        stamina.Resetar();
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
            if (ball.canDamage && ball.whoThrows != null && ball.whoThrows.gameObject != this.gameObject && (ballReference == null || ball.gameObject != ballReference.gameObject) && Time.timeSinceLevelLoad - lastDamageTime > 1f)
            {
                lastDamageTime = Time.timeSinceLevelLoad;
                health.CausarDano();
                // aúdio do dano
                audioSource.PlayOneShot(gettingHitClip);

                if (health.health > 0)
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
    /// Verifica a possibilidade de arremessar a bola quando os botões forem pressionados
    /// </summary>
    private void VerificaArremesso()
    {
        //arremessa com a barra de espaço
        if (Input.GetKeyDown(KeyCode.Space) && ballReference != null)
        {
            ////direção do mouse
            var direction = transform.forward;
            direction.z *= throwForce;
            direction.x *= throwForce;
            direction.y = 0.4f;

            animator.SetBool("arremessar", true);
            throwDirection = direction;
        }
        // arremessa com o botão esquerdo do mouse
        else if (Input.GetMouseButton(0) && ballReference != null)
        {
            animator.SetBool("arremessar", true);

            //pegando um raio em direção ao mouse e adequando a força proporcional à altura
            var aimDirection = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            aimDirection.y += (rightHand.transform.position.y * 0.2f);
            throwDirection = aimDirection * throwForce;
        } else
        {
            animator.SetBool("arremessar", false);
        }
    }

    public void Arremessar()
    {
        StartCoroutine(SincronizarArremesso());
    }

    IEnumerator SincronizarArremesso()
    {
        var waitTime = 0.04f;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            waitTime = 0.12f;

        yield return new WaitForSeconds(waitTime);
        ballReference.Arremessar(throwDirection);
        //disponibilizando para pegar novas bolas
        canHold = true;
        ballReference = null;
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
        if (inputShift && stamina.stamina > 0 && !stamina.isReloading)
        {
            // acelerando o audio clip para simular corrida
            if (audioSource.clip == walkingClip)
                audioSource.pitch = 1.2f;

            //diminuindo stamina
            stamina.DiminuirStamina();
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
        if (stamina.stamina >= rollStaminaCost && inputControl && (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward")))
        {
            audioSource.PlayOneShot(rollClip);

            animator.SetTrigger("rolar");
            transform.Translate(0, 0, speed * 1.2f * Time.deltaTime);
            stamina.DiminuirStamina(rollStaminaCost);
        }

        //desativando ou ativando os colliders de acordo com a necessidade
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward"))
        {
            capsuleCollider.enabled = true;
            sphereCollider.enabled = false;
        }
        else
        {
            //caso esteja rolando, ativa um collider menor
            sphereCollider.enabled = true;
            capsuleCollider.enabled = false;
        }
    }
}
