using UnityEngine;
using UnityEngine.UI;
using TMPro; // Подключаем пространство имен TextMeshPro

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider hpSlider;
    public TMP_Text hpText; // Ссылка на текст с цифрами

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}