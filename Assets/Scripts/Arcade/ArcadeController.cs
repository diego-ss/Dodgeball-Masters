using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject ball;
    public GameObject staminaBoostPrefab;
    public GameObject gameArea;

    [Header("Parâmetros")]
    public float leftEnemyAreaLimit;
    public float rightEnemyAreaLimit;
    public float backEnemyAreaLimit;
    public float frontEnemyAreaLimit;
    public float enemiesLeft;

    public float playerBoostAreaLeftLimit;
    public float playerBoostAreaRightLimit;
    public float playerBoostAreaFrontLimit;
    public float playerBoostAreaBackLimit;

    [SerializeField]
    [Header("Inimigos")]
    private List<GameObject> enemies;

    private float lastBoostEmissionTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.isPlaying = true;
        enemies = GameObject.FindGameObjectsWithTag("IAEnemy").ToList();
        ball = GameObject.FindGameObjectsWithTag("Ball").First();
    }

    // Update is called once per frame
    void Update()
    {
        enemiesLeft = enemies.Where(x => !x.GetComponent<EnemyController>().isDead).Count();

        if(enemiesLeft == 0)
        {
            GameManager.Instance.victory = true;
            StartCoroutine(CarregarGameOver(2));
        }

        if (!GameManager.Instance.isPlaying)
        {
            GameManager.Instance.victory = false;
            StartCoroutine(CarregarGameOver(2));
        }

        if(Time.timeSinceLevelLoad - lastBoostEmissionTime > 10)
        {
            lastBoostEmissionTime = Time.timeSinceLevelLoad;

            if (Random.value > 0.7)
                GerarBoost();
        }
        
    }

    private void GerarBoost()
    {
        var randomXLimit = Random.Range(playerBoostAreaBackLimit, playerBoostAreaFrontLimit);
        var randomZLimit = Random.Range(playerBoostAreaLeftLimit, playerBoostAreaRightLimit);
        var position = new Vector3(randomXLimit, 1.5f, randomZLimit);
        var boost = Instantiate(staminaBoostPrefab, gameArea.transform);
        boost.transform.position = position;
    }

    public void OrientarInimigos(Transform bola, bool ballIsOnEnemyGround)
    {
        //dizendo ao inimigo se ele pode ou não perseguir a bola.
        //só vai atrás da bola o inimigo mais próximo
        enemies.ForEach(x => {
            x.GetComponent<EnemyController>().ballDistance = Vector3.Distance(x.transform.position, bola.position);
            x.GetComponent<EnemyController>().canCatchBall = false;
            }
        );

        if (ballIsOnEnemyGround)
        {
            var closest = enemies.Where(x=>!x.GetComponent<EnemyController>().isDead).OrderBy(x => x.GetComponent<EnemyController>().ballDistance).First();
            if(closest != null)
                closest.GetComponent<EnemyController>().canCatchBall = true;
        }
        else
            enemies.ForEach(x => x.GetComponent<EnemyController>().canCatchBall = false);
    }

    IEnumerator CarregarGameOver(int seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        GameManager.Instance.isPlaying = false;
        SceneManager.LoadScene("03_GameOver");
    }
}
