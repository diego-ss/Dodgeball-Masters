using Assets.Scripts.Enums;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Gameplay Geral")]
    public ModoJogo modoJogo;
    public bool isPlaying = true;
    public bool victory;

    [Header("Gameplay Treino")]
    public int trainingRoundTime = 60;

    static private GameManager instance;

    static public GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
