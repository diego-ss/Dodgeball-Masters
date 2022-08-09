using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGround : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        //avisando que a bola caiu na área do player
        if (other.CompareTag("Ball"))
        {
            arcadeController.OrientarInimigos(other.transform, false);
        }
    }
}
