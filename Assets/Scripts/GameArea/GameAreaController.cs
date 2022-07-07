using UnityEngine;

public class GameAreaController : MonoBehaviour
{
    private GameObject ball;
 
    // Start is called before the first frame update
    void Start()
    {
        ball = transform.Find("Ball").gameObject;
        InicializarBola();
    }

    void InicializarBola()
    {
        ball.transform.localPosition = new Vector3(5f, 10f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
