using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset;
    private GameObject player;
    public float YOffset;
    public float Sensibility;
    public float RotationLimit;

    float rotX;
    float rotY;
    // Start is called before the first frame update

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse X");
        float mouseX = Input.GetAxis("Mouse Y");

        rotX -= mouseX * Sensibility * Time.deltaTime;
        rotY += mouseY * Sensibility * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -RotationLimit, RotationLimit);
        //transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }

    private void LateUpdate()
    {
        transform.position = offset + player.transform.position + player.transform.up * YOffset;
    }
}
