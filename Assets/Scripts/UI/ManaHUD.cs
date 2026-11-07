using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Смужка мани гравця у світі. Тільки показує ману з BattleUnit, нічого не змінює.
public sealed class ManaHUD : MonoBehaviour
{
    public Slider mpSlider;
    public TMP_Text mpText;
    // Чию ману показуємо
    public BattleUnit unit;

    // Що зараз показано (щоб не оновлювати UI кожен кадр без потреби)
    private int shownMP = -1;
    private int shownMaxMP = -1;

    private void Update()
    {
        if (unit == null)
        {
            return;
        }

        int mp = unit.CurrentMP;
        int maxMP = unit.MaxMP;

        // Нічого не змінилось
        if (mp == shownMP && maxMP == shownMaxMP)
        {
            return;
        }

        shownMP = mp;
        shownMaxMP = maxMP;

        if (mpSlider != null)
        {
            // Максимум хоча б 1, щоб слайдер не зламався при 0
            mpSlider.maxValue = Mathf.Max(1, maxMP);
            mpSlider.value = mp;
        }

        if (mpText != null)
        {
            mpText.text = mp + " / " + maxMP;
        }
    }
}
