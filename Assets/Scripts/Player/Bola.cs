using UnityEngine;

public class Bola : MonoBehaviour
{
    public bool canHold = true;
    public bool canDamage = false;
    public bool throwed = false;
    public GameObject whoThrows;

    [Header("AudioClips")]
    public AudioClip ballHitClip;
    public AudioClip ballCatchClip;
    public AudioClip ballThrowClip;

    private Rigidbody rb;
    private GameObject ballOrigin;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
        audioSource.PlayOneShot(ballCatchClip);
        GetComponent<TrailRenderer>().enabled = false;
    }

    private void LateUpdate()
    {
        if(ballOrigin != null)
            transform.position = ballOrigin.transform.position;

        if (throwed && Mathf.Abs(Vector3.Distance(transform.position, whoThrows.transform.position)) >= 0.5)
        {
            GetComponent<SphereCollider>().enabled = true;
            throwed = false;
        }
    }

    public void Arremessar(Vector3 forceVector)
    {
        //desativando a possibilidade de coleta
        canHold = false;
        canDamage = true;
        ballOrigin = null;
        //retorna as configurações do rigidbody e collider
        rb.isKinematic = false;
        //inputa força na bola
        throwed = true;
        rb.AddForce(forceVector, ForceMode.Impulse);
        audioSource.PlayOneShot(ballThrowClip);
        GetComponent<TrailRenderer>().enabled = true;


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

            audioSource.PlayOneShot(ballHitClip);
        }

    }
}
