using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject ball;

    [Header("Parâmetros")]
    public float leftEnemyAreaLimit;
    public float rightEnemyAreaLimit;
    public float backEnemyAreaLimit;
    public float frontEnemyAreaLimit;
    public float enemiesLeft;

    [SerializeField]
    [Header("Inimigos")]
    private List<GameObject> enemies; 

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
            var closest = enemies.OrderBy(x => x.GetComponent<EnemyController>().ballDistance).First();
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
