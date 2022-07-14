using System.Collections;
using UnityEngine;

public class Canhao : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject ballPrefab;

    [Header("Parametros")]
    public float rotateSpeed;
    public float maxRotateAngle;

    private GameObject ballOrigin;
    private Transform cannonArmor;
    private Bola ballScript;
    private int ballToThrow;
    private float lastShotTime;

    // Start is called before the first frame update
    void Start()
    {
        //captura o objeto de referência para instanciar a bola
        ballOrigin = transform.Find("Armor").Find("Cannon").Find("BallOrigin").gameObject;
        //"mira" do canhão"
        cannonArmor = transform.Find("Armor");
    }

    private void Update()
    {
        var rotY = Mathf.Ceil(cannonArmor.rotation.eulerAngles.y);

        if ((rotY <= (Mathf.Ceil(maxRotateAngle + Mathf.Abs(rotateSpeed))) && rotY >= (Mathf.Ceil(maxRotateAngle))) || (rotY >= (360f - maxRotateAngle) && rotY <= 360f - maxRotateAngle + Mathf.Abs(rotateSpeed)))
            rotateSpeed *= -1;

        cannonArmor.Rotate(new Vector3(0, rotateSpeed, 0));

        RaycastHit hitInfo;
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Player");

        if (ballToThrow == 0 && Time.timeSinceLevelLoad - lastShotTime > 3)
            CarregarCanhao();

        if (ballToThrow > 0 && Physics.Raycast(new Ray(ballOrigin.transform.position, ballOrigin.transform.forward), out hitInfo, Mathf.Infinity, layerMask))
        {
            lastShotTime = Time.timeSinceLevelLoad;
            AtirarBola(hitInfo);
        }
    }

    private void AtirarBola(RaycastHit hitInfo)
    {
        var force = hitInfo.transform.position - ballOrigin.transform.position;
        force.Normalize();
        force *= 3f;
        force.y += cannonArmor.position.y;
        ballScript.Arremessar(force);
        ballToThrow--;
    }

    private void CarregarCanhao()
    {
        var ball = Instantiate(ballPrefab, ballOrigin.transform.localPosition, ballOrigin.transform.rotation);
        ballScript = ball.GetComponent<Bola>();
        ballScript.Capturar(ballOrigin, gameObject);
        ballToThrow++;
    }

    // LateUpdate is called once per frame
    void LateUpdate()
    {
        //var time = Mathf.Ceil(Time.timeSinceLevelLoad);

        //if (canShoot && time != 0 && time % 5 == 0)
        //{

        //    canShoot = false;
        //}

    }
}
