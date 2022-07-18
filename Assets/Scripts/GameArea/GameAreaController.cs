using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAreaController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float trainingTotalTime;

    [Header("Referências")]
    public TMP_Text gameTimeText;

    private float trainingRemainingGameTime;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.isPlaying = true;
        trainingTotalTime = GameManager.Instance.trainingRoundTime;
        trainingRemainingGameTime = trainingTotalTime;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            trainingRemainingGameTime = Mathf.Clamp(trainingRemainingGameTime - Time.deltaTime, 0, Mathf.Infinity);
            gameTimeText.text = trainingRemainingGameTime.ToString("00");

            if (Mathf.Clamp(trainingRemainingGameTime, 0, Mathf.Infinity) == 0)
                GameManager.Instance.isPlaying = false;
        }
        else
        {
            //TODO - REGRAS DE VITÓRIA
            //ALVOS DERRUBADOS...
            if (trainingRemainingGameTime > 0)
                GameManager.Instance.victory = true;
            else
                GameManager.Instance.victory = false;

            StartCoroutine(CarregarGameOver(3));
        }
    }

    IEnumerator CarregarGameOver(int seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene("03_GameOver");
    }
}
