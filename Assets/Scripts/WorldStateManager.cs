using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance { get; private set; }

    private HashSet<string> deadBosses = new HashSet<string>();
    private HashSet<string> clearedGauntlets = new HashSet<string>();

    public Vector3 checkpointPosition;
    public bool hasCheckpoint = false;
    public bool checkpointUsedOnce = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkBossDead(string id)
    {
        deadBosses.Add(id);
    }

    public bool IsBossDead(string id)
    {
        return deadBosses.Contains(id);
    }

    public void MarkGauntletCleared(string id)
    {
        clearedGauntlets.Add(id);
    }

    public bool IsGauntletCleared(string id)
    {
        return clearedGauntlets.Contains(id);
    }

    public void SaveCheckpoint(Vector3 pos)
    {
        checkpointPosition = pos;
        hasCheckpoint = true;
    }

    public void MarkCheckpointUsedOnce()
    {
        checkpointUsedOnce = true;
    }
}
