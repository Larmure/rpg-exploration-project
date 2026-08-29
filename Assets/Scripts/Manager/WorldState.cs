using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Garde en mémoire l'état "global" du monde qui doit survivre
/// aux changements de scène (mais pas forcément à la fermeture du jeu) :
/// objets ramassés, et plus tard ennemis vaincus si besoin.
/// Même logique que PersistentSystems.cs / TransitionManager.cs.
/// </summary>
public class WorldState : MonoBehaviour
{
    public static WorldState Instance;

    private readonly HashSet<string> collectedIds = new HashSet<string>();
    private readonly HashSet<string> defeatedIds = new HashSet<string>(); // pas utilisé pour l'instant

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // devient racine
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Objets ramassables ---
    public bool IsCollected(string id) => !string.IsNullOrEmpty(id) && collectedIds.Contains(id);
    public void MarkCollected(string id)
    {
        if (!string.IsNullOrEmpty(id))
            collectedIds.Add(id);
    }

    // --- Ennemis (prêt pour plus tard) ---
    public bool IsDefeated(string id) => !string.IsNullOrEmpty(id) && defeatedIds.Contains(id);
    public void MarkDefeated(string id)
    {
        if (!string.IsNullOrEmpty(id))
            defeatedIds.Add(id);
    }
}
