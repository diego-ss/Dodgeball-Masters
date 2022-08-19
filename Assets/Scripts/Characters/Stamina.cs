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

    // Start is called before the first frame update
    void Start()
    {
        staminaFill = transform.Find("Canvas").Find("staminaFill").GetComponent<Image>();
        staminaFillOriginalColor = staminaFill.color;
        Resetar();
    }

    /// <summary>
    /// Reseta a stamina para os status iniciais
    /// </summary>
    public void Resetar()
    {
        stamina = maxStamina;
    }

    /// <summary>
    /// Aumenta a stamina atual
    /// </summary>
    public void AumentarStamina()
    {
        // ajustando o UI da imagem conforme a stamina
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
        // se a stamina chega a zero, não deixa correr até que recarregue pelo menos 15
        if (stamina == 0)
            isReloading = true;

        if (stamina > 15)
            isReloading = false;

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

        if (staminaFill.IsActive())
            AtualizarUI();
    }
}
