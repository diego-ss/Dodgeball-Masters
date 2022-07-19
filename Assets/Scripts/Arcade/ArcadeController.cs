using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcadeController : MonoBehaviour
{

    private List<GameObject> enemies; 

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.isPlaying = true;
        enemies = GameObject.FindGameObjectsWithTag("IAEnemy").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OrientarInimigos(Transform bola)
    {
        //dizendo ao inimigo se ele pode ou não perseguir a bola.
        //só vai atrás da bola o inimigo mais próximo
        enemies.ForEach(x => {
            x.GetComponent<EnemyController>().ballDistance = Vector3.Distance(x.transform.position, bola.position);
            x.GetComponent<EnemyController>().canCatchBall = false;
            }
        );
        var nextedEnemy = enemies.OrderBy(x=>x.GetComponent<EnemyController>().ballDistance).First();
        nextedEnemy.GetComponent<EnemyController>().canCatchBall = true;
    }
}
