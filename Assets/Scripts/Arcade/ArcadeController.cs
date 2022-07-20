using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcadeController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject ball;

    [Header("Parâmetros")]
    public float leftEnemyAreaLimit = -7.0f;
    public float rightEnemyAreaLimit = 7.0f;
    public float backEnemyAreaLimit = -11.0f;
    public float frontEnemyAreaLimit = -2.0f;

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
    }
}
