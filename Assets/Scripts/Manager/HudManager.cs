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
        healthText.text = $"{PlayerScript.instance.currentHealth}/{PlayerScript.instance.maxHealth}";
        armorText.text = $"Armor {PlayerScript.instance.armorPoints}"; 
        goldText.text = $"{PlayerScript.instance.gold}";
        manaText.text = $"{PlayerScript.instance.currentMana}/{PlayerScript.instance.maxMana}";
    }
}
