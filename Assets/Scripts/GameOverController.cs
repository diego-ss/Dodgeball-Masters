using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text gameOverText;
    public Button restartButton;

    private ModoJogo modoJogo;

    private void Start()
    {
        Cursor.visible = true;

        if (!GameManager.Instance.victory)
            gameOverText.text = "Ah nao! Voce perdeu.";
        else
            gameOverText.text = "Parabens, voce venceu!";

        modoJogo = GameManager.Instance.modoJogo;

        if(modoJogo == ModoJogo.ARCADE)
            restartButton.gameObject.SetActive(false);
    }

    public void Reiniciar()
    {
        GameManager.Instance.trainingScore = 0;

        if(modoJogo == Assets.Scripts.Enums.ModoJogo.TREINO)
            SceneManager.LoadScene("QuadraTreino", LoadSceneMode.Single);
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("00_Menu", LoadSceneMode.Single);
    }
}
