using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public bool canCatchBall;
    public float ballDistance;

    [Header("Parâmetros")]
    public float speed;
    public float throwForce;

    [Header("Referências")]
    public Image staminaFill;
    public Image healthFill;

    private bool canHold = true;
    private bool reloadingStamina;

    private const float totalHealth = 5f;
    private float health;
    private float runSpeed;
    private float stamina;
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
        health = totalHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (canCatchBall)
        {
            //movimenta até a bola
        }
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
        //staminaFill.fillAmount = stamina / 100.0f;

        //if (stamina <= 15)
        //    staminaFill.color = Color.red;
        //else
        //    staminaFill.color = staminaFillColor;
    }

    private void AtualizarHealth()
    {
        //// ajustando o UI da imagem conforme a saúde
        //healthFill.fillAmount = health / totalHealth;

        //if (health <= 1)
        //    healthFill.color = Color.red;
        //else
        //    healthFill.color = healthFillColor;
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

            //verificando se a bola pode causar dano e se não foi o próprio jogador que atirou
            if (ball.canDamage && ball.whoThrows.gameObject != this.gameObject)
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
                }
            }

        }
    }
}
