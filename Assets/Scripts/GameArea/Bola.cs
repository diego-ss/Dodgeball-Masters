using UnityEngine;

public class Bola : MonoBehaviour
{
    public bool canHold = true;
    public bool canDamage = false;
    public GameObject whoThrows;

    private Rigidbody rb;
    private GameObject ballOrigin;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Capturar(GameObject ballOrigin, GameObject owner)
    {
        //reseta a localposition para ficar junto ao elemento pai
        transform.localPosition = Vector3.zero;
        this.whoThrows = owner;
        this.ballOrigin = ballOrigin;
        //seta o rigidbody como kinematic para evitar gravidade
        rb.isKinematic = true;
        //desativa o collider
        GetComponent<SphereCollider>().enabled = false;        
    }

    private void Update()
    {
        if(ballOrigin != null)
            transform.position = ballOrigin.transform.position;
    }

    public void Arremessar(Vector3 forceVector)
    {
        //desativando a possibilidade de coleta
        canHold = false;
        canDamage = true;
        ballOrigin = null;
        //retorna as configurações do rigidbody e collider
        rb.isKinematic = false;
        GetComponent<SphereCollider>().enabled = true;
        //inputa força na bola
        rb.AddForce(forceVector, ForceMode.Impulse);

        //TODO - Eliminar a bola? E se sair das bordas?
        //TODO - e as bolas que os canhões atiram?
    }

    private void OnCollisionEnter(Collision collision)
    {
        //restaurando possibilidade de capturar e retirando dano
        if (collision.transform.CompareTag("BarreiraQuadra"))
        {
            canHold = true;
            canDamage = false;
            whoThrows = null;
        }
    }
}
