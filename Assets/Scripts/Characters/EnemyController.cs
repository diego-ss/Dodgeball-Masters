using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float speed;
    public float throwForce;
    public GameObject ballToFollow;
    public float ballDistance;
    public bool isDead;
    public float rollStaminaCost;


    [Header("Referências")]
    private GameObject arcadeController;

    [Header("Config. de aúdio")]
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip gettingHitClip;
    public AudioClip walkingClip;
    public AudioClip dyingClip;
    public AudioClip rollClip;

    private bool canHold = true;
    private float lastDamageTime;
    private Vector3 throwDirection;

    private float runSpeed;
    private Vector3 lookAtActual;
    private GameObject playerRef;

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private BoxCollider boxCollider;
    private Health health;
    private Stamina stamina;
    private GameObject rightHand;
    private Bola ballReference;
    private float? timeTriggerThrow;

    // Start is called before the first frame update
    void Start()
    {
        //capturando componentes
        animator = transform.GetComponent<Animator>();
        capsuleCollider = transform.GetComponent<CapsuleCollider>();
        sphereCollider = transform.GetComponent<SphereCollider>();
        boxCollider = transform.GetComponent<BoxCollider>();
        audioSource = transform.GetComponent<AudioSource>();
        health = transform.GetComponent<Health>();
        stamina = transform.GetComponent<Stamina>();

        arcadeController = GameObject.Find("ArcadeController");
        playerRef = GameObject.FindGameObjectWithTag("Player");

        ProcurarReferenciaMao(transform);
    }

    /// <summary>
    /// busca a referência do objeto "ballReference"
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

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            // caso o som de andar não esteja ativo, ativa ele
            if (audioSource.clip != walkingClip)
            {
                audioSource.clip = walkingClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            if (ballToFollow && ballReference == null)
            {
                //movimenta até a bola
                SeguirBola();
            }
            else if (ballReference != null)
            {
                //se tem a bola, olha para o jogador e vai na direção dele
                transform.LookAt(playerRef.transform);

                if (timeTriggerThrow == null)
                    timeTriggerThrow = Time.timeSinceLevelLoad;
            }
            else
            {
                //posições aleatórias quando chega perto do ponto, o ponto é o centro ou a perturbação acontece
                if ((Mathf.Abs(Vector3.Distance(transform.position, lookAtActual)) < 0.8f || lookAtActual == Vector3.zero) || Random.value > 0.995f)
                {
                    var arcadeComponent = arcadeController.GetComponent<ArcadeController>();
                    var randomXLimit = Random.Range(arcadeComponent.backEnemyAreaLimit, arcadeComponent.frontEnemyAreaLimit);
                    var randomZLimit = Random.Range(arcadeComponent.leftEnemyAreaLimit, arcadeComponent.rightEnemyAreaLimit);

                    lookAtActual = new Vector3(randomXLimit, 0.016f, randomZLimit);

                }

                transform.LookAt(lookAtActual);
            }

            AndarOuCorrer();
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && ballReference != null)
        {
            var timeRef = Time.timeSinceLevelLoad - timeTriggerThrow;

            //verifica a distância para o player ou espera de 2 a 4 segundos desde o momento que pega a bola para arremessar
            if (((Mathf.Abs(Vector3.Distance(playerRef.transform.position, transform.position)) < 1f) && timeRef > 1f) || timeRef > Random.Range(1, 4))
                DefinirDirecaoArremesso();
        }
    }

    private void LateUpdate()
    {
        VerificarRolar();
    }

    /// <summary>
    /// desativa e ativa os colliders quando o inimigo rola
    /// </summary>
    private void VerificarRolar()
    {
        //desativando ou ativando os colliders de acordo com a necessidade
        if (!isDead && !animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward"))
        {
            capsuleCollider.enabled = true;
            sphereCollider.enabled = false;
            boxCollider.enabled = true;
        }
        else if (!isDead)
        {
            //caso esteja rolando, ativa um collider menor
            sphereCollider.enabled = true;
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;
        }
    }

    /// <summary>
    /// olha para a bola e vai na direção de la
    /// </summary>
    /// <param name="bola"></param>
    private void SeguirBola()
    {
        // pegando a posição da bola
        var refPosition = ballToFollow.transform.position;
        //zerando y para o personagem não subir
        refPosition.y = 0;
        //olhando para a bola
        transform.LookAt(refPosition);
    }

    /// <summary>
    /// define a direção da bola antes de arremessar
    /// </summary>
    public void DefinirDirecaoArremesso()
    {
        //direção forward do personagem
        var direction = transform.forward;
        var sortedForce = throwForce + Random.value * 1.5f; // fator de imprevisibilidade
        // altura do arremesso considera a altura do personagem e a distância para o player
        // proporcional à distância e inversamente proporcional à altura
        direction.y = (Random.Range(0.02f, 0.06f) * Mathf.Abs(Vector3.Distance(playerRef.transform.position, transform.position))) / (rightHand.transform.position.y * 1.5f) ;
        direction.z *= sortedForce;
        direction.x *= sortedForce;
        throwDirection = isDead ? direction * 0.01f : direction;
        animator.SetBool("arremessar", true);
    }

    public void Arremessar()
    {
        StartCoroutine(SincronizarArremesso());
    }

    IEnumerator SincronizarArremesso()
    {
        yield return new WaitForSeconds(0.1f);
        ballReference.Arremessar(throwDirection);
        timeTriggerThrow = null;
        //disponibilizando para pegar novas bolas
        canHold = true;
        ballReference = null;
        animator.SetBool("arremessar", false);
    }

    /// <summary>
    /// aplica a lógica de movimentação do personagem
    /// </summary>
    private void AndarOuCorrer()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            animator.SetBool("andarFrente", true);
            runSpeed = 1.3f * speed;

            //corre enquanto tem energia e não está recarregando
            if (stamina.stamina > 0 && !stamina.isReloading)
            {
                // acelerando o audio clip para simular corrida
                if (audioSource.clip == walkingClip)
                    audioSource.pitch = 1.2f;

                //diminuindo stamina
                stamina.DiminuirStamina();
                animator.SetBool("correr", true);
                transform.Translate(0, 0, runSpeed * Time.deltaTime);
            }
            else
            {
                // acelerando o audio clip para simular corrida
                if (audioSource.clip == walkingClip)
                    audioSource.pitch = 1f;

                animator.SetBool("correr", false);
                transform.Translate(0, 0, Time.deltaTime * speed);
            }
        }
        else
        {
            animator.SetBool("andarFrente", false);
        }
    }

    /// <summary>
    /// aplica a animação de rolagem
    /// </summary>
    private void Rolar()
    {
        //rolagem
        if (stamina.stamina >= rollStaminaCost && (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward")))
        {
            audioSource.PlayOneShot(rollClip);

            animator.SetTrigger("rolar");
            transform.Translate(0, 0, speed * 1.2f * Time.deltaTime);
            stamina.DiminuirStamina(rollStaminaCost);
        }
    }
    private void Morrer()
    {
        audioSource.PlayOneShot(dyingClip);

        //desativa colliders e ativa animação de morte
        animator.SetTrigger("morte");
        sphereCollider.enabled = false;
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        ballToFollow = null;
        canHold = false;
        //elimina as forças que exerciam influência no personagem
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        isDead = true;

        if (ballReference != null)
            DefinirDirecaoArremesso();
    }

    private void OnTriggerEnter(Collider other)
    {
        //verificando se é uma bola
        if (other.transform.CompareTag("Ball"))
        {
            var ball = other.GetComponent<Bola>();

            //se a bola causa dano, tenta desviar
            if (ball.canDamage && ball.whoThrows != null && !ball.whoThrows.CompareTag("IAEnemy"))
            {
                if (Random.value > 0.5f)
                    Rolar();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // verificando se uma bola colidiu
        if (collision.transform.CompareTag("Ball"))
        {

            var ball = collision.transform.GetComponent<Bola>();

            //verificando se a bola pode causar dano e se não foi o próprio jogador que atirou e se não foi um amigo ou se está no tempo de recuperação
            if (ball.canDamage && ball.whoThrows != null && (ball.whoThrows.gameObject != this.gameObject && !ball.whoThrows.CompareTag("IAEnemy")) && (ballReference == null || ball.gameObject != ballReference.gameObject) && Time.timeSinceLevelLoad - lastDamageTime > 1f)
            {
                lastDamageTime = Time.timeSinceLevelLoad;
                health.CausarDano();
                audioSource.PlayOneShot(gettingHitClip);

                if (health.health > 0)
                {
                    //limpa as animações em execução
                    animator.Rebind();
                    animator.SetTrigger("atingido");
                }
                else
                {
                    Morrer();
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
}
