using UnityEngine;

public class EnemyCanvas : MonoBehaviour
{
    private GameObject playerRef;

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");    
    }

    // Update is called once per frame
    void Update()
    {
        //a barra de vida olha para o player
        transform.LookAt(playerRef.transform);
    }
}
