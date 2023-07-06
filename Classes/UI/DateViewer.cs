using ModKit;
using ModKit.DataViewer;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomThings {
    public static class DataViewer {
        public static Settings settings => Main.settings;
        public static IEnumerable<Scene> GetAllScenes() {
            for (var i = 0; i < SceneManager.sceneCount; i++) {
                yield return SceneManager.GetSceneAt(i);
            }
        }
        private static readonly Dictionary<string, Func<object>> TARGET_LIST = new Dictionary<string, Func<object>>()
        {
            { "None", null },
            { "Game", () => MainGameScript.Instance },
            { "Player", () => MainGameScript.Instance.PlayerAvatar },
            { "MainCamera", () => MainGameScript.Instance.MainCamera },
            { "Shop", () => MainGameScript.Instance.Shop },
            { "UnlockedAreas", () => MainGameScript.Instance.UnlockedAreas },
            { "UnlockedBuildables", () => MainGameScript.Instance.UnlockedBuildables },
            { "Scene", () => SceneManager.GetActiveScene() },
            { "WorkshopCustomizationManager", () => MainGameScript.Instance.WorkshopCustomizationManager },
            { "CustomerManager", () => CustomerManager.Instance },
            { "DialogueManager", () => DialogueManager.Instance },
            { "MasterTimer", () => MasterTimer.Instance },
            { "UI Manager", () => UIManager.Instance },
            { "Potion Prices", () => PotionPricing.Instance },
            { "DBCustomerDatabase", () => typeof(DBCustomerDatabase) },
            { "QuestLog", () => typeof(QuestLog) },
            { "DialogueLua", () => typeof(DialogueLua) },
            { "DialogueDatabase", () => typeof(DialogueDatabase) },
            { "ResourceFactory", () => ResourceNodeManager.Instance },
            { "QuestManager", () => QuestManager.Instance },
            { "QuestStateManager", () => QuestStateManager.Instance },
            { "Root Game Objects", () => DataViewer.GetAllScenes().SelectMany(s => s.GetRootGameObjects()) },
            { "Game Objects", () => UnityEngine.Object.FindObjectsOfType<GameObject>() },
            { "Unity Resources", () =>  Resources.FindObjectsOfTypeAll(typeof(GameObject)) }
       };

        private static readonly string[] _targetNames = TARGET_LIST.Keys.ToArray();

        private static ReflectionTreeView _treeView = null;

        public static int Priority => 0;
        public static void ResetTree() {
            if (_treeView == null)
                _treeView = new ReflectionTreeView();

            Func<object> getTarget = TARGET_LIST[_targetNames[Main.settings.selectedRawDataType]];
            if (getTarget == null)
                _treeView.Clear();
            else
                _treeView.SetRoot(getTarget());
        }
        public static void OnGUI() {
            try {
                if (_treeView == null)
                    ResetTree();

                // target selection
                UI.ActionSelectionGrid(ref Main.settings.selectedRawDataType, _targetNames, 5, (s) => {
                    ResetTree();
                });

                // tree view
                if (Main.settings.selectedRawDataType != 0) {
                    GUILayout.Space(10f);

                    _treeView.OnGUI();
                }
            } catch (Exception e) {
                Main.settings.selectedRawDataType = 0;
                _treeView.Clear();
                Mod.Error(e.StackTrace);
                throw e;
            }
        }

    }
}