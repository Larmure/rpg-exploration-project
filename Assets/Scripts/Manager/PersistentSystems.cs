using System.Collections.Generic;
using UnityEngine;

public class SystemsPersistence : MonoBehaviour
{
    private static readonly HashSet<string> persistedRoots = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        persistedRoots.Clear();
    }

    private void Awake()
    {
        if (persistedRoots.Contains(gameObject.name))
        {
            gameObject.SetActive(false); // <- ajouté : invisible aux Find dès cette frame
            Destroy(gameObject);
            return;
        }

        persistedRoots.Add(gameObject.name);
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}