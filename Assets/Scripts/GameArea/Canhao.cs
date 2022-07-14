using System.Collections;
using UnityEngine;

public class Canhao : MonoBehaviour
{
    public GameObject ballPrefab;

    private GameObject ballOrigin;
    private Bola ballScript;
    private bool canShoot;

    float initTime;

    // Start is called before the first frame update
    void Start()
    {
        ballOrigin = transform.Find("Armor").Find("Cannon").Find("BallOrigin").gameObject;
        initTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        var time = Mathf.Ceil(Time.timeSinceLevelLoad);

        if (!canShoot && time != 0 && time % 3 == 0) 
        {
            var ball = Instantiate(ballPrefab);
            ballScript = ball.GetComponent<Bola>();
            ballScript.Capturar(ballOrigin, gameObject);
            canShoot = true;
        }

        if (canShoot && time != 0 && time % 7 == 0)
        {
            ballScript.Arremessar(new Vector3(3f, 0.2f, 0f));
            canShoot = false;
        }
    }
}
