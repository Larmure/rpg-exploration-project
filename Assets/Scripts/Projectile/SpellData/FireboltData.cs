using UnityEngine;

[CreateAssetMenu(fileName = "NewFireboltData", menuName = "Spells/Firebolt Data")]
public class FireboltData : SpellData
{
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.15f;
}