using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider hpSlider;
    public Gradient gradient;
    public Image fill;

    public void SetHud(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Level " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
        fill.color = gradient.Evaluate(hpSlider.normalizedValue);
    }
}
