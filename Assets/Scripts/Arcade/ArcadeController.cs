using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject ball;
    public GameObject gameArea;
    public GameObject[] powerUpsPrefabs;
    public GameObject player;

    [Header("Parâmetros")]
    public float leftEnemyAreaLimit;
    public float rightEnemyAreaLimit;
    public float backEnemyAreaLimit;
    public float frontEnemyAreaLimit;
    public List<GameObject> enemiesLeft;

    public float playerLeftAreaLimit;
    public float playerRightAreaLimit;
    public float playerFrontAreaLimit;
    public float playerBackAreaLimit;

    [SerializeField]
    [Header("Inimigos")]
    private List<GameObject> enemies;

    private float lastBoostEmissionTime = 0;

    private void Awake()
    {
        GameManager.Instance.isPlaying = true;

        //posicionando player
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = GameObject.Find("PlayerInitialPosition").transform.position;
        player.transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("IAEnemy").ToList();
        ball = GameObject.FindGameObjectsWithTag("Ball").First();
        player.GetComponent<PlayerController>().Reset();
    }

    // Update is called once per frame
    void Update()
    {
        enemiesLeft = enemies.Where(x => !x.GetComponent<EnemyController>().isDead).ToList();

        if(enemiesLeft != null && enemiesLeft.Count() == 0)
        {
            GameManager.Instance.victory = true;
            StartCoroutine(CarregarGameOver(2));

        }

        if (!GameManager.Instance.isPlaying)
        {
            GameManager.Instance.victory = false;
            StartCoroutine(CarregarGameOver(4));
        }

        if (Time.timeSinceLevelLoad - lastBoostEmissionTime > Random.Range(7,18))
        {
            lastBoostEmissionTime = Time.timeSinceLevelLoad;

            if (Random.value > 0.7)
                GerarPowerUp();
        }
        
    }

    private void GerarPowerUp()
    {
        // posição aleatória na área do player
        var randomXLimit = Random.Range(playerBackAreaLimit, playerFrontAreaLimit);
        var randomZLimit = Random.Range(playerLeftAreaLimit, playerRightAreaLimit);

        // instanciando um powerup aleatório
        var position = new Vector3(randomXLimit, 1.5f, randomZLimit);
        var boost = Instantiate(powerUpsPrefabs[Random.Range(0, powerUpsPrefabs.Length)], gameArea.transform);
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
            var aliveEnemies = enemies.Where(x => !x.GetComponent<EnemyController>().isDead).ToList();
            if(aliveEnemies != null && aliveEnemies.Count > 0)
            {
                var closest = aliveEnemies.OrderBy(x => x.GetComponent<EnemyController>().ballDistance).FirstOrDefault();
                if (closest != null)
                    closest.GetComponent<EnemyController>().canCatchBall = true;
            }
        }
        else
            enemies.ForEach(x => x.GetComponent<EnemyController>().canCatchBall = false);
    }

    IEnumerator CarregarGameOver(int seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        //GameManager.Instance.isPlaying = false;
        if(GameManager.Instance.victory == false)
            Destroy(player);

        SceneManager.LoadScene("02_GameOver", LoadSceneMode.Single);
    }
}
