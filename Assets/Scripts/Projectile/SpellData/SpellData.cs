using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellData", menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public float speed = 20f;
    public float lifetime = 3f;
    public int damage = 15;
    public int manaCost = 10;
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 1f;
    public bool destroyOnHit = true;
}