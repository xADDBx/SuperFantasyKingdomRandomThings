using ModKit.Utility;
using System;
using System.Collections.Generic;
using UnityModManagerNet;
using static ResourceSource_Base;

namespace RandomThings {
    public class Settings : UnityModManager.ModSettings {
        public int selectedRawDataType = 0;
        public float TimeMultiplier = 1f;
        public float LootMultiplier = 1f;
        public bool useFineLootMultiplier = false;
        public SerializableDictionary<ResourceSourceVariants, float> fineLootMultipliers = newMultDict();
#if false
        public float ImportMultiplier = 1f;
        public bool useFineImportMultiplier = false;
        public SerializableDictionary<string, float> fineImportMultiplier = new();
#endif
        public int selectedTab = 0;
        public int saveGameChainFileCap = 10;
        public int maxChestStackSize = 50;
        public int maxCrateStackSize = 50;
        public Extensions.SortMode sortMode = Extensions.SortMode.byNameAsc;
        public bool sortOnGameLoad = false;
        public bool sortContainerOnOpening = false;
        public bool showSortButtons = false;
        public bool enableInvulnerability = false;
        public bool enableEverythingCostsNothing = false;
        public bool showCharacterOnMap = false;

        public override void Save(UnityModManager.ModEntry modEntry) {
            Save(this, modEntry);
        }

        public static SerializableDictionary<ResourceSourceVariants, float> newMultDict() {
            SerializableDictionary<ResourceSourceVariants, float> ret = new();
            foreach (ResourceSourceVariants res in Enum.GetValues(typeof(ResourceSourceVariants))) {
                ret[res] = 1.0f;
            }
            return ret;
        }
    }
}
