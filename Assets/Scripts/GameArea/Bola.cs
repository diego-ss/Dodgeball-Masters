using UnityEngine;

public class Bola : MonoBehaviour
{
    public bool canHold = true;
    public bool canDamage = false;

    private Rigidbody rb;
    private GameObject playerHand;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Capturar(GameObject playerHand)
    {
        //reseta a localposition para ficar junto ao elemento pai
        transform.localPosition = Vector3.zero;
        this.playerHand = playerHand;
        //seta o rigidbody como kinematic para evitar gravidade
        rb.isKinematic = true;
        //desativa o collider
        GetComponent<SphereCollider>().enabled = false;
        canDamage = true;

    }

    private void Update()
    {
        if(playerHand != null)
            transform.position = playerHand.transform.position;
    }

    public void Arremessar(Vector3 forceVector)
    {
        playerHand = null;
        //retorna as configurações do rigidbody e collider
        rb.isKinematic = false;
        GetComponent<SphereCollider>().enabled = true;
        //inputa força na bola
        rb.AddForce(forceVector, ForceMode.Impulse);
        //desativando a possibilidade de coleta
        canHold = false;

        //TODO - Eliminar a bola? E se sair das bordas?
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("BarreiraQuadra"))
        {
            canHold = true;
            canDamage = false;
        }
    }
}
