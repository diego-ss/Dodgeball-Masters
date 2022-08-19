using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [Header("Parâmetros")]
    public float maxStamina;
    public float stamina;
    public float staminaDecay;
    public float staminaRestore;
    public bool isReloading;

    [Header("Referências")]
    [SerializeField]
    private Image staminaFill;
    private Color staminaFillOriginalColor;
    private bool isEnemy;

    // Start is called before the first frame update
    void Start()
    {
        try {
            staminaFill = transform.Find("Canvas").Find("staminaFill").GetComponent<Image>();
        }
        catch {
            staminaFill = null;
        }

        if(staminaFill  != null)
            staminaFillOriginalColor = staminaFill.color;

        Resetar();
        isEnemy = transform.CompareTag("IAEnemy");
    }

    /// <summary>
    /// Reseta a stamina para os status iniciais
    /// </summary>
    public void Resetar()
    {
        if (isEnemy)
            stamina = Random.Range(0f, 100f);
        else
            stamina = maxStamina;
    }

    /// <summary>
    /// Aumenta a stamina atual
    /// </summary>
    public void AumentarStamina()
    {
        // gerando perturbações no reload da stamina dos inimigos
        if((isEnemy && Random.value > 0.5) || !isEnemy)
            stamina += Time.deltaTime * staminaRestore;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    /// <summary>
    /// Aumenta a stamina atual
    /// </summary>
    /// <param name="amount"></param>
    public void AumentarStamina(float amount)
    {
        stamina = Mathf.Clamp(stamina + amount, 0, maxStamina);
    }

    /// <summary>
    /// Diminui a stamina atual
    /// </summary>
    public void DiminuirStamina(float amount = 0)
    {
        if (amount == 0)
            stamina -= (staminaDecay * Time.deltaTime);
        else
            stamina -= amount;
    }

    /// <summary>
    /// Atualização dos elementos de tela
    /// </summary>
    private void AtualizarUI()
    {
        staminaFill.DOFillAmount(stamina / maxStamina, 0.5f).SetEase(Ease.OutBack);

        if (stamina <= 15)
            staminaFill.color = Color.red;
        else
            staminaFill.color = staminaFillOriginalColor;
    }

    // Update is called once per frame
    void Update()
    {
        AumentarStamina();

        // se a stamina chega a zero, não deixa correr até que recarregue pelo menos 15
        if (stamina == 0)
            isReloading = true;

        if (!isEnemy && stamina > 15)
            isReloading = false;
        // maior tempo de recarga minima para inimigos
        else if (isEnemy && stamina > (maxStamina * 0.8))
            isReloading = false;

        if (staminaFill != null && staminaFill.IsActive())
            AtualizarUI();
    }
}
