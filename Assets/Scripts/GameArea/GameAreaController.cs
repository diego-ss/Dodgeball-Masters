using TMPro;
using UnityEngine;

public class GameAreaController : MonoBehaviour
{
    [Header("Parâmetros")]
    public float trainingTotalTime;

    [Header("Referências")]
    public TMP_Text gameTimeText;

    private float trainingRemainingGameTime;
    private GameObject gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.isPlaying = true;
        trainingTotalTime = GameManager.Instance.trainingRoundTime;
        trainingRemainingGameTime = trainingTotalTime;
        gameOverPanel = GameObject.Find("Canvas").transform.Find("gameOverPanel").gameObject;
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
        } else
        {
            GameObject.Find("Canvas").transform.Find("gameOverPanel").gameObject.SetActive(true);
        }
    }
}
