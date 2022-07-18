using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text gameOverText;

    private void Start()
    {
        if (!GameManager.Instance.isPlaying && !GameManager.Instance.victory)
            gameOverText.text = "Ah nao! Voce perdeu.";
        else if (!GameManager.Instance.isPlaying)
            gameOverText.text = "Vitoria!";
    }

    public void ReiniciarTreino()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
