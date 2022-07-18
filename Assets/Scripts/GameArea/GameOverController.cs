﻿using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text gameOverText;

    private void Start()
    {
        if (!GameManager.Instance.victory)
            gameOverText.text = "Ah nao! Voce perdeu.";
        else
            gameOverText.text = "Parabens, voce venceu!";
    }

    public void Reiniciar()
    {
        if(GameManager.Instance.modoJogo == Assets.Scripts.Enums.ModoJogo.TREINO)
            SceneManager.LoadScene("01_GameScene", LoadSceneMode.Single);
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("00_Menu", LoadSceneMode.Single);
    }
}
