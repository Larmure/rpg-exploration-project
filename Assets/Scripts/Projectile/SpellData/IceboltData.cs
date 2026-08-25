using UnityEngine;

[CreateAssetMenu(fileName = "NewIceboltData", menuName = "Spells/Icebolt Data")]
public class IceboltData : SpellData
{
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.4f;
    public float slowMultiplier = 0.5f; 
    public float slowDuration = 2f; 
}