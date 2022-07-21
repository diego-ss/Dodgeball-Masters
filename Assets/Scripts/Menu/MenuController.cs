using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ModoDeTreino()
    {
        GameManager.Instance.modoJogo = Assets.Scripts.Enums.ModoJogo.TREINO;
        GameManager.Instance.trainingScore = 0;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ModoArcade()
    {
        GameManager.Instance.modoJogo = Assets.Scripts.Enums.ModoJogo.ARCADE;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void Sair()
    {
        Application.Quit();
    }
}
