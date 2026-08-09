using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Manager : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    public static HUD_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        healthText.text = $"Health : {PlayerScript.instance.currentHealth}/{PlayerScript.instance.maxHealth}";
    }
}
