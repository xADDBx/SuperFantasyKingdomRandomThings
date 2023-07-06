using HarmonyLib;
using ModKit;
using ModKit.Utility;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using static ModKit.UI;
using static UnityModManagerNet.UnityModManager;

namespace RandomThings {
#if DEBUG
    [EnableReloading]
#endif
    public class Main {
        internal static Harmony HarmonyInstance;
        public static bool Enabled;
        public static ModEntry.ModLogger Mod;
        public static Settings settings;
        private static bool Load(ModEntry modEntry) {
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnUnload = OnUnload;
            Mod = modEntry.Logger;
            settings = Settings.Load<Settings>(modEntry);
            onStart();
            HarmonyInstance = new Harmony(modEntry.Info.Id);
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        private static bool OnUnload(ModEntry modEntry) {
            DataViewer.ResetTree();
            HarmonyInstance.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        private static bool OnToggle(ModEntry modEntry, bool value) {
            Enabled = value;
            return true;
        }

        private static void OnSaveGUI(ModEntry modEntry) {
            settings.Save(modEntry);
        }
        public static void OnGUI(ModEntry modEntry) {
            TabBar(ref settings.selectedTab,
                () => Space(25),
                new NamedAction("Other", () => OtherUI.OnGUI())
#if DEBUG
                , new NamedAction("DataViewer", () => DataViewer.OnGUI()));
#else
                );
#endif
        }
        private static void onStart() {
        }
    }
}
