using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    Vector3 objectPos;

    public bool canHold = true;
    public GameObject tempParent;
    public bool isHolding = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !isHolding)
        {
            isHolding = true;
            this.tempParent = other.gameObject.transform.Find("Mesh").Find("Body").Find("Head").gameObject;
            transform.SetParent(tempParent.transform);
            transform.position = this.tempParent.transform.position + new Vector3(0,2f,0);
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().detectCollisions = true;
        }
    }
}
