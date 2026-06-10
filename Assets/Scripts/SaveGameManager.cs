using System.Collections.Generic;
using UnityEngine;

public static class SaveGameManager
{
    // PLAYER
    public static Vector3 savedPlayerPosition;
    public static int savedMaxHealth;
    public static int savedMaxMana;
    public static bool hasSave = false;

    // SPELL UNLOCKS
    public static bool bladeBeamUnlocked;
    public static bool megaSlashUnlocked;
    public static bool slashDashUnlocked;

    // BOSSES
    private static HashSet<string> defeatedBosses = new HashSet<string>();
    public static void MarkBossDefeated(string bossName) => defeatedBosses.Add(bossName);
    public static bool IsBossDefeated(string bossName) => defeatedBosses.Contains(bossName);

    // CHESTS
    private static HashSet<string> openedChests = new HashSet<string>();
    public static void MarkChestOpened(string chestID) => openedChests.Add(chestID);
    public static bool IsChestOpened(string chestID) => openedChests.Contains(chestID);

    // LEVERS
    private static HashSet<string> usedLevers = new HashSet<string>();
    public static void MarkLeverUsed(string leverID) => usedLevers.Add(leverID);
    public static bool IsLeverUsed(string leverID) => usedLevers.Contains(leverID);

    // GAUNTLETS
    private static HashSet<string> clearedGauntlets = new HashSet<string>();
    public static void MarkGauntletCleared(string gauntletID) => clearedGauntlets.Add(gauntletID);
    public static bool IsGauntletCleared(string gauntletID) => clearedGauntlets.Contains(gauntletID);

    // FULL SAVE (statue/checkpoint): position + stats + spells
    public static void SaveProgress(PlayerStats player)
    {
        hasSave = true;

        savedPlayerPosition = player.transform.position;
        savedMaxHealth = player.MaxHealth;
        savedMaxMana = player.MaxMana;

        PlayerAttack attack = Object.FindFirstObjectByType<PlayerAttack>();
        if (attack != null)
        {
            bladeBeamUnlocked = attack.bladeBeamUnlocked;
            megaSlashUnlocked = attack.megaSlashUnlocked;
            slashDashUnlocked = attack.slashDashUnlocked;
        }
    }

    // PARTIAL SAVE (chests): stats + spells, keep last checkpoint position
    public static void SaveProgressWithoutPosition(PlayerStats player)
    {
        hasSave = true;

        // Do NOT change savedPlayerPosition here
        savedMaxHealth = player.MaxHealth;
        savedMaxMana = player.MaxMana;

        PlayerAttack attack = Object.FindFirstObjectByType<PlayerAttack>();
        if (attack != null)
        {
            bladeBeamUnlocked = attack.bladeBeamUnlocked;
            megaSlashUnlocked = attack.megaSlashUnlocked;
            slashDashUnlocked = attack.slashDashUnlocked;
        }
    }

    // LOAD
    public static void ApplyProgressAfterSceneLoad(PlayerStats player)
    {
        if (!hasSave)
            return;

        player.SetMaxHealth(savedMaxHealth);
        player.SetMaxMana(savedMaxMana);

        // Respawn at last statue/checkpoint
        player.transform.position = savedPlayerPosition;

        PlayerAttack attack = Object.FindFirstObjectByType<PlayerAttack>();
        if (attack != null)
        {
            attack.bladeBeamUnlocked = bladeBeamUnlocked;
            attack.megaSlashUnlocked = megaSlashUnlocked;
            attack.slashDashUnlocked = slashDashUnlocked;
        }
    }
}
