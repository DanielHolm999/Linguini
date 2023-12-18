using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxXp(int xp)
    {
        slider.maxValue = xp;
    }

    public void SetXp(int xp)
    {
        slider.value = xp;
    }
}
