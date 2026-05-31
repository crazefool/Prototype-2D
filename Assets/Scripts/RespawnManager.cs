using UnityEngine;

public static class RespawnManager
{
    public static void RespawnPlayer(PlayerStats stats)
    {
        var ws = WorldStateManager.Instance;

        if (!ws.hasCheckpoint)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
            return;
        }

        Transform t = stats.transform;
        t.position = ws.checkpointPosition;

        Rigidbody2D rb = stats.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        stats.RestoreFullHealth();
        stats.RestoreFullMana();

        RestoreWorldState();
    }

    private static void RestoreWorldState()
    {
        var ws = WorldStateManager.Instance;

        BossState[] bosses = Object.FindObjectsByType<BossState>(FindObjectsSortMode.None);
        foreach (var boss in bosses)
        {
            if (ws.IsBossDead(boss.uniqueID))
                boss.gameObject.SetActive(false);
        }

        GauntletState[] gauntlets = Object.FindObjectsByType<GauntletState>(FindObjectsSortMode.None);
        foreach (var g in gauntlets)
        {
            if (ws.IsGauntletCleared(g.uniqueID))
                g.gameObject.SetActive(false);
        }
    }
}
