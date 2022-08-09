using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text titulo;

    private float refTime;

    // Start is called before the first frame update
    void Start()
    {
        refTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // alterando cores do título
        if(Time.timeSinceLevelLoad - refTime > 0.7f)
        {
            titulo.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            refTime = Time.timeSinceLevelLoad;
        }
    }

    public void ModoDeTreino()
    {
        //carregando modo de treino
        GameManager.Instance.modoJogo = Assets.Scripts.Enums.ModoJogo.TREINO;
        GameManager.Instance.trainingScore = 0;
        SceneManager.LoadScene("QuadraTreino", LoadSceneMode.Single);
    }

    public void ModoArcade()
    {
        //carregando tela de seleção de personagem
        GameManager.Instance.modoJogo = Assets.Scripts.Enums.ModoJogo.ARCADE;
        GameManager.Instance.level = 0;
        SceneManager.LoadScene("01_SelecaoPersonagem", LoadSceneMode.Single);
    }

    public void Sair()
    {
        Application.Quit();
    }
}
