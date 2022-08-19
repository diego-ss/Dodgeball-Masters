using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Health : MonoBehaviour
{
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
    }

    public void Resetar()
    {
        health = maxHealth;
        AtualizarUI();
    }

    public void AumentarVida(float amount = 1)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        AtualizarUI();
    }

    public void CausarDano(float amount = -1)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        AtualizarUI();
    }

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
