using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGround : MonoBehaviour
{
    private ArcadeController arcadeController;

    // Start is called before the first frame update
    void Start()
    {
        arcadeController = GameObject.Find("ArcadeController").GetComponent<ArcadeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //avisando que a bola caiu na área dos inimigos
        if (other.CompareTag("Ball"))
        {
            arcadeController.OrientarInimigos(other.transform, true);
        }
    }
}
