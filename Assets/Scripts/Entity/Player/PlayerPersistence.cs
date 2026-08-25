using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Un Player existe déjà (venant d'une scène précédente) :
            // on détruit ce doublon pour garder celui qui a l'état à jour.
            Destroy(gameObject);
        }
    }
}