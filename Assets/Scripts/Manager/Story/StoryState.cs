using System.Collections.Generic;
using UnityEngine;

public class StoryState : MonoBehaviour
{
    public static StoryState Instance { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetFlag(string key, bool value)
    {
        flags[key] = value;
    }

    public bool GetFlag(string key)
    {
        return flags.TryGetValue(key, out bool value) && value;
    }

    public bool HasTalkedToNPC(string npcId)
    {
        return GetFlag($"talked_to_{npcId}");
    }

    public void MarkTalkedToNPC(string npcId)
    {
        SetFlag($"talked_to_{npcId}", true);
    }
}