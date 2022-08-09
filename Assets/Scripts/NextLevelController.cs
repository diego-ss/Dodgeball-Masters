using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelController : MonoBehaviour
{
    public Transform playerPosition;
    public Button btnNextLevel;
    public Button btnMenu;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = playerPosition.position;
        player.transform.rotation = playerPosition.rotation;
        player.GetComponent<Animator>().Play("Dancing");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continuar()
    {
        GameManager.Instance.level++;
        SceneManager.LoadScene("QuadraArcade_" + GameManager.Instance.level, LoadSceneMode.Single);
    }

    public void MenuPrincipal()
    {
        Destroy(player);
        SceneManager.LoadScene("00_Menu", LoadSceneMode.Single);
    }
}
