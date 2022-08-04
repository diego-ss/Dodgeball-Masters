using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float speed;
    public float throwForce;
    public float staminaDecay;
    public bool canCatchBall = false;
    public float ballDistance;
    public bool isDead;

    [SerializeField]
    private float health;
    [SerializeField]
    private float stamina;

    [Header("Referências")]
    private Image healthFill;
    private GameObject arcadeController;

    [Header("Config. de aúdio")]
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip gettingHitClip;
    public AudioClip walkingClip;
    public AudioClip dyingClip;

    private bool canHold = true;
    private bool reloadingStamina;

    private const float totalHealth = 5f;
    private float runSpeed;
    private const float rollStaminaCost = 25f;
    private Color healthFillColor;
    private Vector3 lookAtActual;
    private GameObject playerRef;

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private BoxCollider boxCollider;
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

        arcadeController = GameObject.Find("ArcadeController");
        playerRef = GameObject.Find("PlayerChar");
        healthFill = transform.Find("Canvas").Find("healthImage").GetComponent<Image>();
        healthFillColor = healthFill.color;

        //capturando posição da bola na mão
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

        stamina = Random.Range(0.0f, 100f);
        health = totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            //a barra de vida olha para o player
            healthFill.transform.parent.LookAt(playerRef.transform);

            // caso o som de andar não esteja ativo, ativa ele
            if (audioSource.clip != walkingClip)
            {
                audioSource.clip = walkingClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            if (canCatchBall && ballReference == null)
            {
                //movimenta até a bola
                var bola = arcadeController.GetComponent<ArcadeController>().ball;
                SeguirBola(bola);
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
            //espera de 2 a 4 segundos desde o momento que pega a bola para arremessar
            if (Time.timeSinceLevelLoad - timeTriggerThrow > Random.Range(1, 4))
                Arremessar();
        }
    }

    private void LateUpdate()
    {
        AtualizarStamina();
        AtualizarHealth();
        VerificarRolar();
    }

    private void VerificarRolar()
    {
        //desativando ou ativando os colliders de acordo com a necessidade
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward"))
        {
            sphereCollider.enabled = false;
            boxCollider.enabled = true;
            capsuleCollider.enabled = true;
        }
        else
        {
            //caso esteja rolando, ativa um collider menor
            sphereCollider.enabled = true;
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;
        }
    }

    private void AtualizarStamina()
    {
        // se a stamina chega a zero, não deixa correr até que recarregue pelo menos 15
        if (stamina == 0)
            reloadingStamina = true;

        if (stamina > 80)
            reloadingStamina = false;

        // ajustando o UI da imagem conforme a stamina
        // adicionando uma perturbação no carregamento da stamina para que não seja constante
        // tentativa de quebrar a sincronia nas corridas
        if (Random.value > 0.4)
        {
            stamina += Time.deltaTime * 10;
            stamina = Mathf.Clamp(stamina, 0, 100);
        }
    }

    private void AtualizarHealth()
    {
        // ajustando o UI da imagem conforme a saúde
        healthFill.fillAmount = health / totalHealth;

        if (health <= 1)
            healthFill.color = Color.red;
        else
            healthFill.color = healthFillColor;
    }


    private void SeguirBola(GameObject bola)
    {
        // pegando a posição da bola
        var refPosition = bola.transform.position;
        //zerando y para o personagem não subir
        refPosition.y = 0;
        //olhando para a bola
        transform.LookAt(refPosition);
    }

    private void Arremessar()
    {
        //direção forward do personagem
        var direction = transform.forward;
        var sortedForce = throwForce + Random.value * 1.5f; // fator de imprevisibilidade
        direction.y = 0.4f;
        direction.z *= sortedForce;
        direction.x *= sortedForce;
        ballReference.Arremessar(direction);
        timeTriggerThrow = null;
        //disponibilizando para pegar novas bolas
        canHold = true;
        ballReference = null;
    }

    private void AndarOuCorrer()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            animator.SetBool("andarFrente", true);
            runSpeed = 1.3f * speed;

            //corre enquanto tem energia e não está recarregando
            if (stamina > 0 && !reloadingStamina)
            {
                // acelerando o audio clip para simular corrida
                if (audioSource.clip == walkingClip)
                    audioSource.pitch = 1.2f;

                //diminuindo stamina
                stamina -= (staminaDecay * Time.deltaTime);
                animator.SetBool("correr", true);
                transform.Translate(0,0,runSpeed * Time.deltaTime);
            }
            else
            {
                // acelerando o audio clip para simular corrida
                if (audioSource.clip == walkingClip)
                    audioSource.pitch = 1f;

                animator.SetBool("correr", false);
                transform.Translate(0,0,Time.deltaTime* speed);
            }
        }
        else
        {
            animator.SetBool("andarFrente", false);
        }
    }

    private void Rolar()
    {
        //rolagem
        if (stamina >= rollStaminaCost && (!animator.GetCurrentAnimatorStateInfo(0).IsName("RollForward")))
        {
            animator.SetTrigger("rolar");
            transform.Translate(0, 0, speed * 1.2f * Time.deltaTime);
            stamina -= rollStaminaCost;
        }
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
                if (Random.value > 0.3f)
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

            //verificando se a bola pode causar dano e se não foi o próprio jogador que atirou
            if (ball.canDamage && ball.whoThrows != null && ball.whoThrows.gameObject != this.gameObject)
            {
                health--;
                audioSource.PlayOneShot(gettingHitClip);

                if (health > 0)
                {
                    //limpa as animações em execução
                    animator.Rebind();
                    animator.SetTrigger("atingido");
                }
                else
                {
                    audioSource.PlayOneShot(dyingClip);

                    //desativa colliders e ativa animação de morte
                    animator.SetTrigger("morte");
                    sphereCollider.enabled = false;
                    capsuleCollider.enabled = false;
                    boxCollider.enabled = false;
                    canHold = false;
                    //elimina as forças que exerciam influência no personagem
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    isDead = true;
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
