using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Здоров'я персонажа. Це ЄДИНЕ місце, де зберігається HP
// (і у світі, і в бою використовується саме цей компонент).
public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    // Смужка HP (може бути не призначена)
    public Slider hpSlider;
    // Текст "50 / 100" (може бути не призначений)
    public TMP_Text hpText;

    void Start()
    {
        // На старті персонаж повністю здоровий
        currentHealth = maxHealth;
        UpdateUI();
    }

    // Отримати шкоду
    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        // HP не може бути менше 0 і більше максимуму
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    // Лікування (використовують зілля).
    // Повертає, скільки HP реально відновилось.
    public int Heal(int amount)
    {
        // Не можна лікувати на 0 або мінус, і не можна воскресити мертвого
        if (amount <= 0 || currentHealth <= 0)
        {
            return 0;
        }

        int before = currentHealth;

        // Рахуємо в long, щоб не було переповнення, якщо amount дуже великий
        long newHealth = (long)currentHealth + amount;
        long limit = System.Math.Max(0, maxHealth);
        if (newHealth > limit)
        {
            newHealth = limit;
        }
        currentHealth = (int)newHealth;

        UpdateUI();

        int restored = currentHealth - before;
        if (restored < 0)
        {
            restored = 0;
        }
        return restored;
    }

    // Оновлюємо смужку і текст HP
    void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }

        if (hpText != null)
        {
            hpText.text = currentHealth + " / " + maxHealth;
        }
    }
}
