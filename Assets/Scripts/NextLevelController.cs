using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelController : MonoBehaviour
{
    public Transform playerPosition;
    public Button btnNextLevel;
    public Button btnMenu;

    private GameObject player;
    private bool hasNextLevel;
    private string nextLevelName;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.level++;
        nextLevelName = "QuadraArcade_" + GameManager.Instance.level;
        hasNextLevel = SceneUtility.GetBuildIndexByScenePath(nextLevelName) != -1;

        Cursor.visible = true;

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

        if (hasNextLevel)
            SceneManager.LoadScene("QuadraArcade_" + GameManager.Instance.level, LoadSceneMode.Single);
        else
            SceneManager.LoadScene("04_FimDeJogo");
    }

    public void MenuPrincipal()
    {
        Destroy(player);
        GameManager.Instance.level = 0;
        SceneManager.LoadScene("00_Menu", LoadSceneMode.Single);
    }
}
