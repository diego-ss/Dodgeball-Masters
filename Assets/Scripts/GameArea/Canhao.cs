using System.Collections;
using UnityEngine;

public class Canhao : MonoBehaviour
{
    [Header("Refer�ncias")]
    public GameObject ballPrefab;

    [Header("Par�metros")]
    [Range(-1.5f, 1.5f)]
    public float rotateSpeed;
    [Range(0, 90)]
    public float maxRotateAngle;
    [Range(0.1f, 5.0f)]
    public float shootForce;
    [Range(3f, 10f)]
    public float timeToShootAgain;

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
        //captura o objeto de refer�ncia para instanciar a bola
        ballOrigin = transform.Find("Armor").Find("Cannon").Find("BallOrigin").gameObject;
        //"mira" do canh�o"
        cannonArmor = transform.Find("Armor");

        //linha de refer�ncia ao raycast
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        //dire��o aleat�ria
        rotateSpeed *= Random.value > 0.5 ? 1 : -1;
    }

    private void Update()
    {
        /* Limitando a rota��o do canh�o de acordo com o par�metro estabelecido
         * Caso o canh�o alcance um dos limites, a velocidade � invertida
         */
        var rotY = Mathf.Ceil(cannonArmor.rotation.eulerAngles.y);
        if ((rotY <= (Mathf.Ceil(maxRotateAngle + Mathf.Abs(rotateSpeed))) && rotY >= maxRotateAngle) || (rotY <= (360f - maxRotateAngle) && rotY >= (360f - maxRotateAngle - Mathf.Abs(rotateSpeed))))
            rotateSpeed *= -1;

        cannonArmor.Rotate(new Vector3(0, rotateSpeed, 0));

        RaycastHit hitInfo;
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Player");

        //espera tr�s segundos para carregar o canh�o caso ele n�o tenha bola
        if (ballToThrow == 0 && Time.timeSinceLevelLoad - lastShotTime > timeToShootAgain)
            CarregarCanhao();

        //desenhando o raio de contato com os colliders
        if(Physics.Raycast(new Ray(ballOrigin.transform.position, ballOrigin.transform.forward), out hitInfo, Mathf.Infinity)){
            lineRenderer.SetPosition(0, ballOrigin.transform.position);
            lineRenderer.SetPosition(1, ballOrigin.transform.position + ballOrigin.transform.forward * hitInfo.distance);
        }

        //caso tenha bola e o player seja atingido pelo raio, atira a bola
        if (ballToThrow > 0 && Physics.Raycast(new Ray(ballOrigin.transform.position, ballOrigin.transform.forward), out hitInfo, Mathf.Infinity, layerMask))
        {
            //marca��o do instante �ltimo tiro 
            lastShotTime = Time.timeSinceLevelLoad;
            AtirarBola(hitInfo);
        }
    }

    /// <summary>
    /// Aplica for�a a uma bola j� carregada no canh�o
    /// </summary>
    /// <param name="hitInfo">refer�ncia para o objeto referenciado na mira</param>
    private void AtirarBola(RaycastHit hitInfo)
    {
        //calculando for�a a partir do ponto de contato
        var force = hitInfo.point - ballOrigin.transform.position;
        //alinhando for�a um pouco mais pra cima
        force.y += yPerturbation;
        force.Normalize();
        force *= shootForce;
        ballScript.Arremessar(force);
        ballToThrow--;
    }

    /// <summary>
    /// Carrega uma bola no canh�o caso ele esteja vazio
    /// </summary>
    private void CarregarCanhao()
    {
        //instanciando prefab da bola
        var ball = Instantiate(ballPrefab, ballOrigin.transform.localPosition, ballOrigin.transform.rotation);
        ballScript = ball.GetComponent<Bola>();
        ballScript.Capturar(ballOrigin, gameObject);
        ballToThrow++;
    }
}
