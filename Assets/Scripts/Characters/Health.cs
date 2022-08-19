using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [Header("Parâmetros")]
    public float maxHealth;
    public float health;

    [Header("Referências")]
    [SerializeField]
    private Image healthFill;
    private Color healthFillOriginalColor;

    private void Start()
    {
        healthFill = transform.Find("Canvas").Find("healthFill").GetComponent<Image>();
        healthFillOriginalColor = healthFill.color;
        Resetar();
    }

    /// <summary>
    /// Restaura os status iniciais de vida
    /// </summary>
    public void Resetar()
    {
        health = maxHealth;

        if(healthFill.IsActive())
            AtualizarUI();
    }

    /// <summary>
    /// Aumenta a vida atual
    /// </summary>
    /// <param name="amount"></param>
    public void AumentarVida(float amount = 1)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        AtualizarUI();
    }

    /// <summary>
    /// Diminui a vida atual
    /// </summary>
    /// <param name="amount"></param>
    public void CausarDano(float amount = -1)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        AtualizarUI();
    }

    /// <summary>
    /// Atualização dos elementos de tela
    /// </summary>
    private void AtualizarUI()
    {
        // ajustando o UI da imagem conforme a saúde
        healthFill.DOFillAmount(health/maxHealth, 2).SetEase(Ease.OutBack);

        if (health <= 1)
            healthFill.color = Color.red;
        else
            healthFill.color = healthFillOriginalColor;
    }
}
