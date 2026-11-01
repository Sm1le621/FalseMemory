using TMPro;
using UnityEngine;
using UnityEngine.UI;

// World HUD bar for the player's MP. It only reads BattleUnit resources.
public sealed class ManaHUD : MonoBehaviour
{
    public Slider mpSlider;
    public TMP_Text mpText;
    public BattleUnit unit;

    private int shownMP = -1, shownMaxMP = -1;

    private void Update()
    {
        if (unit == null) return;
        int mp = unit.CurrentMP, maxMP = unit.MaxMP;
        if (mp == shownMP && maxMP == shownMaxMP) return;
        shownMP = mp; shownMaxMP = maxMP;
        if (mpSlider != null)
        {
            mpSlider.maxValue = Mathf.Max(1, maxMP);
            mpSlider.value = mp;
        }
        if (mpText != null) mpText.text = mp + " / " + maxMP;
    }
}
