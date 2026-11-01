using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider hpSlider;
    public TMP_Text hpText;

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

    // Shared by healing consumables. Returns actual restored HP.
    public int Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return 0;
        int before = currentHealth;
        currentHealth = (int)System.Math.Min(System.Math.Max(0, maxHealth), (long)currentHealth + amount);
        UpdateUI();
        return System.Math.Max(0, currentHealth - before);
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
