using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TreinoController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float trainingTotalTime;

    [Header("Referências")]
    public TMP_Text gameTimeText;
    public Text objetivoAlvosText;
    public Text objetivoTempoText;

    private float trainingRemainingGameTime;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.isPlaying = true;
        trainingTotalTime = GameManager.Instance.trainingRoundTime;
        trainingRemainingGameTime = trainingTotalTime;

        objetivoTempoText.text = "SOBREVIVER POR " + trainingTotalTime + " SEGUNDOS";
    }


    // Update is called once per frame
    void Update()
    {
        //contabilizando quantos alvos foram derrubados
        var finalScore = GameManager.Instance.trainingScore - GameManager.Instance.trainingTotalTargets;

        if (GameManager.Instance.isPlaying)
        {
            trainingRemainingGameTime = Mathf.Clamp(trainingRemainingGameTime - Time.deltaTime, 0, Mathf.Infinity);
            gameTimeText.text = trainingRemainingGameTime.ToString("00");

            if (Mathf.Clamp(trainingRemainingGameTime, 0, Mathf.Infinity) == 0)
                GameManager.Instance.isPlaying = false;
        }
        else
        {

            //caso todos os alvos tenham sido derrubados e o tempo esgotou, o jogador vence
            if (trainingRemainingGameTime == 0 && finalScore == 0)
            {
                GameManager.Instance.victory = true;
                objetivoTempoText.color = Color.green;
            }
            else
                GameManager.Instance.victory = false;

            StartCoroutine(CarregarGameOver(2));
        }

        objetivoAlvosText.text = "ALVOS DESTRUÍDOS: " + GameManager.Instance.trainingScore + "/" + GameManager.Instance.trainingTotalTargets;

        if (finalScore == 0)
            objetivoAlvosText.color = Color.green;

    }

    IEnumerator CarregarGameOver(int seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene("03_GameOver");
    }
}
