using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    public static HudManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        healthText.text = $"HP {PlayerScript.instance.currentHealth}/{PlayerScript.instance.maxHealth}";
    }
}
