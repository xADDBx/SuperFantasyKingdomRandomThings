using Assets.Utils;
using ModKit.Utility;
using System;
using UnityEngine;
using static ModKit.UI;
using static ResourceSource_Base;

namespace RandomThings {
    public static class OtherUI {
        public static Settings settings => Main.settings;
        static bool showGameStats = false;
        static bool showDangerous = false;
        static bool showLootMultiplier = false;
        static bool showImportMultiplier = false;
        public static void OnGUI() {
#if DEBUG
            // Opens Console; Console does nothing
            ActionButton("Console Test", () => UIManager.Instance.ShowMenu(FullscreenUIWindowManaged.FullscreenMenuId.DevConsole));
#endif
            LogSlider("Time Multiplier", ref settings.TimeMultiplier, 0.00001f, 10, 1, 5, "", AutoWidth());
            LogSlider("Loot Multiplier", ref settings.LootMultiplier, 0.00001f, 100, 1, 5, "", AutoWidth());
            Toggle("Use Item Specific Loot Multipliers", ref settings.useFineLootMultiplier);
            DisclosureToggle("Item Loot Multiplier", ref showLootMultiplier);
            if (showLootMultiplier) {
                using (HorizontalScope()) {
                    Space(10);
                    using (VerticalScope()) {
                        foreach (ResourceSourceVariants res in Enum.GetValues(typeof(ResourceSourceVariants))) {
                            float tmp = settings.fineLootMultipliers[res];
                            if (LogSlider($"{res} Loot Multiplier", ref tmp, 0.00001f, 100, 1, 5, "", AutoWidth())) {
                                settings.fineLootMultipliers[res] = tmp;
                            }
                        }
                    }
                }
            }
#if false
            LogSlider("Import Multiplier", ref settings.ImportMultiplier, 0.00001f, 10, 1, 5, "", AutoWidth());
            Toggle("Use Item Specific Import Multiplier", ref settings.useFineImportMultiplier);
            DisclosureToggle("Import Loot Multiplier", ref showImportMultiplier);
            if (showImportMultiplier) {
                using (HorizontalScope()) {
                    Space(10);
                    using (VerticalScope()) {
                        foreach (var UID in SingletonBehaviour<InvestmentManager>.Instance.GetUnlockedImports().Keys) {
                            var resName = SingletonBehaviour<ResourceFactory>.Instance.GetResourcePrefab(UID).Name;
                            if (!settings.fineImportMultiplier.ContainsKey(resName)) {
                                settings.fineImportMultiplier[resName] = 1.0f;
                            }
                            float tmp2 = settings.fineImportMultiplier[resName];
                            if (LogSlider($"{resName} Import Multiplier", ref tmp2, 0.00001f, 100, 1, 5, "", AutoWidth())) {
                                settings.fineImportMultiplier[resName] = tmp2;
                            }
                        }
                    }
                }
            }
#endif
            Toggle("Make everything free (Resources + Money)", ref settings.enableEverythingCostsNothing);
            Toggle("Make player invulnerable", ref settings.enableInvulnerability);
            if (MainGameScript.Instance.PlayerAvatar.IsInvulnerable != settings.enableInvulnerability) {
                SettingsManager.Instance.CheatSettings.avatarInvulnerable = settings.enableInvulnerability;
            }
            DisclosureToggle("Show Game Stats", ref showGameStats);
            if (showGameStats) {
                using (HorizontalScope()) {
                    Space(20);
                    Label(GameStatsManager.Instance.GetStatsInAString(), AutoWidth());
                }
            }
            using (HorizontalScope()) {
                if (Toggle("Activate Player Map Symbol", ref settings.showCharacterOnMap, Width(300))) {
                    GameObject found = null;
                    foreach (var obj in Main.objects.Values) {
                        if (obj.name.Equals("Landmark Location Player")) {
                            found = obj;
                        }
                    }
                    if (found != null) {
                        found.SetActive(settings.showCharacterOnMap);
                        if (settings.showCharacterOnMap) {
                            Tweaks.UIManager_ShowMenu_Patch.updatePosition(found);
                        }
                    } else {
                        if (settings.showCharacterOnMap) {
                            Tweaks.UIManager_ShowMenu_Patch.createPlayerMarker();
                        }
                    }
                }
                Label("This only works for Outskirts".Green());
            }
            DisclosureToggle("Dangerous", ref showDangerous);
            if (showDangerous) {
                if (SaveLoadManager.Instance != null) {
                    using (HorizontalScope()) {
                        Label("Changes the Amount of saves (probably per Slot) to keep. Default 10. Games are deleted when loading a save.".Cyan());
                        ValueAdjustorEditable("", () => settings.saveGameChainFileCap, (v) => {
                            settings.saveGameChainFileCap = v;
                            Main.applySaveChange();
                        }, 1, 10, 50);
                    }
                }
                ActionButton("Save", () => {
                    if (GameStatsManager.Instance != null && SaveLoadManager.Instance != null) {
                        GameStatsManager.Instance.TrySaveGameStatsToFile();
                        SaveLoadManager.Instance.TryWriteSaveGameDataToFile();
                    }
                });
            }
        }
    }
}
