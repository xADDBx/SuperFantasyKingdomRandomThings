using Assets.Utils;
using HarmonyLib;
using ModKit.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RandomThings {
    public static class Tweaks {
        public static Settings settings => Main.settings;
        public static HashSet<SolidResourceHolder> chests = new();
        public static HashSet<SolidResourceHolder> crates = new();

        [HarmonyPatch(typeof(MasterTimer), nameof(MasterTimer.Update))]
        private static class MasterTimer_Update_Patch {
            private static void Prefix(MasterTimer __instance) {
                if (!__instance.IsFastForwardingTowardsTarget && (settings.TimeMultiplier != 1f || __instance.TimeMultiplier != 1f)) {
                    __instance.TimeMultiplier = settings.TimeMultiplier;
                }
            }
        }

        [HarmonyPatch(typeof(InventoryGrid), nameof(InventoryGrid.InitializeGridWithElements))]
        private static class InventoryGrid_InitializeGridWithElements_Patch {
            private static void Postfix(InventoryGrid __instance) {
                Transform inv = null;
                if (__instance.gameObject.name.Equals("Inventory Grid Avatar")) {
                    inv = __instance.gameObject.transform.parent.parent;
                } else if (__instance.gameObject.name.Equals("Inventory Grid SmallChest Variant")) {
                    inv = __instance.gameObject.transform.parent;
                }
                if (inv != null) {
                    if (settings.showSortButtons) {
                        var inventoryMenu = inv.Find("HeaderPlank");
                        if (inventoryMenu != null) {
                            if (inventoryMenu.Find("CustomSortButton") == null) {
                                GameObject SkinButton = inventoryMenu.transform.Find("ButtonPlaqueRoundImg (TMP) HUD Navigation back").gameObject;
                                GameObject myButton = GameObject.Instantiate(SkinButton, inventoryMenu.transform);
                                myButton.name = "CustomSortButton";
                                Main.objects[myButton.GetHashCode()] = myButton;
                                myButton.transform.localPosition = new Vector3(-150, 5.5f, 0);
                                myButton.transform.Find("Icon").gameObject.SetActive(false);
                                GameObject.DestroyImmediate(myButton.GetComponent<Button>());
                                Button b = myButton.AddComponent<Button>();
                                b.onClick.AddListener(() => __instance.getInventory().sort(settings.sortMode));
                            }
                        }
                    }
                    if (settings.sortContainerOnOpening) {
                        __instance.getInventory().sort(settings.sortMode);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SolidResourceHolder), nameof(SolidResourceHolder.LoadContentsSaveGameData))]
        private static class SolidResourceHolder_LoadContentsSaveGameData_Patch {
            private static void Postfix(SolidResourceHolder __instance) {
                if (settings.sortOnGameLoad) {
                    if (__instance.name.Equals("Chest(Clone)") || __instance.name.Equals("Avatar")) {
                        __instance.sort(settings.sortMode);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UIManager), nameof(UIManager.ShowMenu), new Type[] { typeof(FullscreenUIWindowManaged.FullscreenMenuId), typeof(object) })]
        public static class UIManager_ShowMenu_Patch {
            public static void updatePosition(GameObject playerMarker) {
                Vector3 pos = MainGameScript.Instance.PlayerAvatar.transform.localPosition;
                Vector3 posStraight = Matrix4x4.Rotate(Quaternion.Euler(0, -45, 0)).MultiplyPoint(pos);
                // Magic numbers found by testing
                posStraight.x *= 2.4644f;
                posStraight.y = posStraight.z * 2.4333f;
                posStraight.z = 0;
                playerMarker.transform.localPosition = posStraight + new Vector3(-149.25f, 280.5f, 0);
                foreach (Transform child in playerMarker.transform) {
                    child.gameObject.SetActive(true);
                }
            }
            public static void createPlayerMarker() {
                GameObject ShopPin = MainGameScript.Instance.MainCamera.transform.Find("--- REGULAR MENUS Sort Order 15/Menu - Journal/Map/LandmarkLocations/Landmark Location Shop").gameObject;
                GameObject playerMarker = GameObject.Instantiate(ShopPin, ShopPin.transform.parent);
                playerMarker.name = "Landmark Location Player";
                playerMarker.transform.localScale = new Vector3(0.7f, 0.7f);
                Main.objects.Add(playerMarker.GetHashCode(), playerMarker);
                foreach (Transform child in playerMarker.transform) {
                    if (child.name.Equals("ButtonText (TMP)")) {
                        GameObject.DestroyImmediate(child.GetComponent<I2.Loc.Localize>());
                        child.GetComponent<TMPro.TextMeshProUGUI>().text = "Player";
                    }
                }
                GameObject zoomButton = MainGameScript.Instance.MainCamera.transform.Find("--- REGULAR MENUS Sort Order 15/Menu - Journal/Map/ButtonPlaqueRoundImg (TMP) Label/ButtonPlaqueRoundImg (TMP)").gameObject;
                Button button = zoomButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                // Needs better implementation (pops in immediately instead of waiting for animation to finish)
                button.onClick.AddListener(() => {
                    bool turnOn = settings.showCharacterOnMap && !playerMarker.activeSelf;
                    playerMarker.SetActive(turnOn);
                    if (turnOn) updatePosition(playerMarker);
                });
                updatePosition(playerMarker);
            }
            private static void Prefix(FullscreenUIWindowManaged.FullscreenMenuId menuId) {
                if (menuId == FullscreenUIWindowManaged.FullscreenMenuId.Journal) {
                    if (settings.showCharacterOnMap) {
                        Avatar player = MainGameScript.Instance.PlayerAvatar;
                        if (player != null) {
                            foreach (GameObject obj in Main.objects.Values) {
                                if (obj.name.Equals("Landmark Location Player")) {
                                    updatePosition(obj);
                                    return;
                                }
                            }
                            createPlayerMarker();
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SolidResourceHolder), nameof(SolidResourceHolder.Awake))]
        private static class SolidResourceHolder_Awake_Patch {
            private static void Prefix(SolidResourceHolder __instance) {
                if (__instance.gameObject.name.StartsWith("Chest")) {
                    chests.Add(__instance);
                    if (settings.maxChestStackSize > 50) {
                        __instance.MaxStackSize = settings.maxChestStackSize;
                    }
                } else if (__instance.gameObject.name.StartsWith("Crate")) {
                    crates.Add(__instance);
                    if (settings.maxCrateStackSize > 50) {
                        __instance.MaxStackSize = settings.maxCrateStackSize;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPurchaseCost), nameof(PlayerPurchaseCost.TryDoPayment))]
        private static class PlayerPurchaseCost_TryDoPayment_Patch {
            private static bool Prefix() {
                if (settings.enableEverythingCostsNothing) {
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RadialSwipe), nameof(RadialSwipe.DoDamage))]
        private static class RadialSwipe_DoDamage_Patch {
            private static bool Prefix() {
                if (settings.enableInvulnerability) {
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ResourceLootSpawner), nameof(ResourceLootSpawner.SpawnLootFromSpawnPoint))]
        private static class ResourceLootSpawner_SpawnLootFromSpawnPoint_Patch {
            private static bool Prefix(ResourceLootSpawner __instance, ResourceSpawnPoint point, LootSpawnSettings lootSpawnSettings) {
                if (__instance.AreaObject.ToString().ToLower().Contains("resourcesource") && (settings.LootMultiplier != 1.0f || (settings.useFineLootMultiplier && settings.fineLootMultipliers.Any(kv => kv.Value != 1.0f)))) {
                    if (point.HasSpawned) {
                        return false;
                    }
                    __instance.lastChainDelay = 0f;
                    __instance.spawnedCount = 0;
                    __instance.circularRotationStart = UnityEngine.Random.value * 360f;
                    __instance.circularSlices = UnityEngine.Random.Range(__instance.LootCircularSlicesPerCircle.x, __instance.LootCircularSlicesPerCircle.y);
                    __instance.circularMultiplier = UnityEngine.Random.Range(__instance.LootCircularSliceMultiplier.x, __instance.LootCircularSliceMultiplier.y);
                    Vector3 position = point.transform.position;
                    foreach (KeyValuePair<string, LootEntry> keyValuePair in point.Entries) {
                        if (!settings.useFineLootMultiplier) {
                            ResourceLootSpawner._spawnLootFromSpawnPoint = (int)(settings.LootMultiplier * keyValuePair.Value.SpawnCount);
                        } else {
                            ResourceLootSpawner._spawnLootFromSpawnPoint = (int)(settings.fineLootMultipliers[__instance.RSBase.ResourceSourceVariant] * keyValuePair.Value.SpawnCount);
                        }
                        while (ResourceLootSpawner._spawnLootFromSpawnPoint > 0) {
                            __instance.GenerateLootAtPoint(keyValuePair.Key, keyValuePair.Value, position, lootSpawnSettings);
                            ResourceLootSpawner._spawnLootFromSpawnPoint--;
                        }
                    }
                    point.HasSpawned = true;
                    return false;
                }
                return true;
            }
        }
#if false
        [HarmonyPatch(typeof(InvestmentManager), nameof(InvestmentManager.GetResourceImportMaxAmount))]
        private static class InvestmentManager_GetResourceImportMaxAmount_Patch {
            private static void Postfix(ref int __result) {
                if (settings.ImportMultiplier != 1.0f || (settings.useFineImportMultiplier && settings.fineImportMultiplier.Any(kv => kv.Value != 1.0f))) {
                    if (!settings.useFineImportMultiplier) {
                        __result = (int)(__result * settings.ImportMultiplier);
                    } else {
                        var resName = SingletonBehaviour<ResourceFactory>.Instance.GetResourcePrefab(resourceUID).Name;
                        if (settings.fineImportMultiplier.ContainsKey(resName)) {
                            __result = (int)(__result * settings.fineImportMultiplier[resName]);
                        }
                        StackTrace stackTrace = new();
                        foreach (var stuff in stackTrace.GetFrame(1).GetMethod().GetMethodBody().LocalVariables) {
                            Main.Mod.Log($"{stuff}");
                            Menu_Import Menu_Import_Window = UIManager._instance._menus[FullscreenUIWindowManaged.FullscreenMenuId.ImportMenu] as Menu_Import;
                            Menu_Import_Window.UpdateElements();
                        }
                    }
                }
            }
        }
#endif
    }
}
