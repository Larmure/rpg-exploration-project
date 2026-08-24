using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI manaText;

    public static HudManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        healthText.text = $"{Player.instance.GetCurrentHealth()}/{Player.instance.GetMaxHealth()}";
        armorText.text = $"Armor {Player.instance.GetArmorPoints()}"; 
        goldText.text = $"{Player.instance.gold}";
        manaText.text = $"{Player.instance.GetCurrentMana()}/{Player.instance.GetMaxMana()}";
    }
}
