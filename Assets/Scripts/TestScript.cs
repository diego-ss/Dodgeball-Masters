using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.Translate(new Vector3(0,0,0.04f));
        if (Input.GetKey(KeyCode.S))
            transform.Translate(new Vector3(0, 0, -0.04f));
        if (Input.GetKey(KeyCode.D))
            transform.Translate(new Vector3(0.04f, 0, 0));
        if (Input.GetKey(KeyCode.A))
            transform.Translate(new Vector3(-0.04f, 0, 0));
    }
}
