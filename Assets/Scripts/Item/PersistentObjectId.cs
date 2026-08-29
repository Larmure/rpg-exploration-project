using UnityEngine;

/// <summary>
/// Donne un identifiant unique et stable à un objet de la scène,
/// utilisé par WorldState pour savoir s'il a déjà été ramassé/vaincu.
/// L'ID doit être généré UNE FOIS dans l'éditeur (clic droit > Générer un ID unique)
/// puis ne plus jamais changer, sinon la sauvegarde ne le reconnaîtra plus.
/// </summary>
[DisallowMultipleComponent]
public class PersistentObjectId : MonoBehaviour
{
    [SerializeField] private string uniqueId;

    public string Id => uniqueId;

#if UNITY_EDITOR
    [ContextMenu("Générer un ID unique")]
    private void GenerateId()
    {
        uniqueId = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
