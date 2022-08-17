using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text titulo;
    public Image dodgeballLogo;
    public GameObject Bryce;
    public GameObject Sophie;
    public GameObject James;

    private float refTime;
    Tweener tweener;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        refTime = 0;

        tweener = dodgeballLogo.gameObject.transform.DOLocalMove(new Vector3(120f, -62f, 0), .8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InBounce);

        Destroy(Bryce.GetComponent<PlayerController>());
        Destroy(James.GetComponent<PlayerController>());
        Destroy(Sophie.GetComponent<PlayerController>());

        Bryce.GetComponent<Animator>().Play("Bouncing Fight Idle");
        James.GetComponent<Animator>().Play("Ninja Idle");
        Sophie.GetComponent<Animator>().Play("Sitting Idle");
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
        tweener.Pause();
        //carregando modo de treino
        GameManager.Instance.modoJogo = Assets.Scripts.Enums.ModoJogo.TREINO;
        GameManager.Instance.trainingScore = 0;
        SceneManager.LoadScene("QuadraTreino", LoadSceneMode.Single);
    }

    public void ModoArcade()
    {
        tweener.Pause();
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
