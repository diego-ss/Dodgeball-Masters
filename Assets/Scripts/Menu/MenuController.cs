using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text titulo;
    public GameObject panelInstrucoes;
    public Image dodgeballLogo;
    public GameObject Bryce;
    public GameObject Sophie;
    public GameObject James;

    private float refTime;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        refTime = 0;

        Destroy(Bryce.GetComponent<PlayerController>());
        Destroy(Bryce.GetComponent<Health>());
        Destroy(Bryce.GetComponent<Stamina>());
        Destroy(James.GetComponent<PlayerController>());
        Destroy(James.GetComponent<Health>());
        Destroy(James.GetComponent<Stamina>());
        Destroy(Sophie.GetComponent<PlayerController>());
        Destroy(Sophie.GetComponent<Health>());
        Destroy(Sophie.GetComponent<Stamina>());

        Bryce.GetComponent<Animator>().Play("Bouncing Fight Idle");
        Sophie.GetComponent<Animator>().Play("Ninja Idle");
        James.GetComponent<Animator>().Play("Sitting Idle");
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

    public void Instrucoes()
    {
        panelInstrucoes.SetActive(true);
    }

    public void FecharInstrucoes()
    {
        panelInstrucoes.SetActive(false);

    }

    public void Sair()
    {
        Application.Quit();
    }
}
