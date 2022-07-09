using UnityEngine;

public class Bola : MonoBehaviour
{
    public bool canHold = true;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Capturar()
    {
        //reseta a localposition para ficar junto ao elemento pai
        transform.localPosition = Vector3.zero;
        //seta o rigidbody como kinematic para evitar gravidade
        rb.isKinematic = true;
        //desativa o collider
        GetComponent<SphereCollider>().enabled = false;

    }

    public void Arremessar(float force)
    {
        //remove o parentesco
        transform.SetParent(null);
        //direção do mouse
        var direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        //retorna as configurações do rigidbody e collider
        rb.isKinematic = false;
        GetComponent<SphereCollider>().enabled = true;
        //inputa força na bola
        rb.AddForce(direction * force, ForceMode.Impulse);
        //desativando a possibilidade de coleta
        canHold = false;

        //TODO - Eliminar a bola? E se sair das bordas?
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("BarreiraQuadra"))
            canHold = true;
    }
}
