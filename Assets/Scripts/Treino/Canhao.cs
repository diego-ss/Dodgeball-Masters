using System.Collections;
using UnityEngine;

public class Canhao : MonoBehaviour
{
    [Header("Referências")]
    public GameObject ballPrefab;

    [Header("Parâmetros")]
    [Range(-100f, 100f)]
    public float rotateSpeed;
    [Range(0.1f, 5.0f)]
    public float shootForce;
    [Range(3f, 10f)]
    public float timeToShootAgain;
    public float maxRotateAngle;

    [Header("Config. de aúdio")]
    private AudioSource audioSource;
    public AudioClip fireClip;
    public AudioClip movementClip;
    public AudioClip reloadingClip;

    private GameObject ballOrigin;
    private Transform cannonArmor;
    private LineRenderer lineRenderer;

    private Bola ballScript;
    private int ballToThrow;
    private float lastShotTime;

    private const float yPerturbation = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //captura o objeto de referência para instanciar a bola
        ballOrigin = transform.Find("Armor").Find("Cannon").Find("BallOrigin").gameObject;
        //"mira" do canhão"
        cannonArmor = transform.Find("Armor");

        //linha de referência ao raycast
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        //material Default-Line
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply")); ;

        //direção aleatória
        maxRotateAngle = Random.Range(35, 60);
        rotateSpeed *= Random.value > 0.5 ? 1 : -1;
    }

    private void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            /* Limitando a rotação do canhão de acordo com o parâmetro estabelecido
             * Caso o canhão alcance um dos limites, a velocidade é invertida
             */
            var rotY = Mathf.Ceil(cannonArmor.rotation.eulerAngles.y);
            if ((rotY <= (Mathf.Ceil(maxRotateAngle + Mathf.Abs(rotateSpeed))) && rotY >= maxRotateAngle) || (rotY <= (360f - maxRotateAngle) && rotY >= (360f - maxRotateAngle - Mathf.Abs(rotateSpeed))))
                rotateSpeed *= -1;

            if(audioSource.clip != movementClip)
            {
                audioSource.clip = movementClip;
                audioSource.Play();
            }

            cannonArmor.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));

            RaycastHit hitInfo;
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Player");

            //espera três segundos para carregar o canhão caso ele não tenha bola
            if (ballToThrow == 0 && Time.timeSinceLevelLoad - lastShotTime > timeToShootAgain)
                CarregarCanhao();

            //desenhando o raio de contato com os colliders
            if (Physics.Raycast(new Ray(ballOrigin.transform.position, ballOrigin.transform.forward), out hitInfo, Mathf.Infinity))
            {
                lineRenderer.SetPosition(0, ballOrigin.transform.position);
                lineRenderer.SetPosition(1, ballOrigin.transform.position + ballOrigin.transform.forward * hitInfo.distance);
            }

            //caso tenha bola e o player seja atingido pelo raio, atira a bola
            if (ballToThrow > 0 && Physics.Raycast(new Ray(ballOrigin.transform.position, ballOrigin.transform.forward), out hitInfo, Mathf.Infinity, layerMask))
            {
                //marcação do instante último tiro 
                lastShotTime = Time.timeSinceLevelLoad;
                AtirarBola(hitInfo);
            }
        }

    }

    /// <summary>
    /// Aplica força a uma bola já carregada no canhão
    /// </summary>
    /// <param name="hitInfo">referência para o objeto referenciado na mira</param>
    private void AtirarBola(RaycastHit hitInfo)
    {
        //calculando força a partir do ponto de contato
        var force = hitInfo.point - ballOrigin.transform.position;
        //alinhando força um pouco mais pra cima
        force.y += yPerturbation;
        force.Normalize();
        force *= shootForce;
        audioSource.PlayOneShot(fireClip);
        ballScript.Arremessar(force);
        ballToThrow--;
    }

    /// <summary>
    /// Carrega uma bola no canhão caso ele esteja vazio
    /// </summary>
    private void CarregarCanhao()
    {
        //instanciando prefab da bola
        audioSource.PlayOneShot(reloadingClip);
        var ball = Instantiate(ballPrefab, ballOrigin.transform.localPosition, ballOrigin.transform.rotation);
        ballScript = ball.GetComponent<Bola>();
        ballScript.Capturar(ballOrigin, gameObject);
        ballToThrow++;
    }
}
