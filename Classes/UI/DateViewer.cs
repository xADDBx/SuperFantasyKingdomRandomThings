using ModKit;
using ModKit.DataViewer;
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
            { "CityManager", () => CityManager.Instance },
            { "DaytimeManager", () => DaytimeManager.Instance },
            { "AchievementManager", () => AchievementManager.Instance },
            { "GameManager", () => GameManager.Instance },
            { "AlertManager", () => AlertManager.Instance },
            { "CombatManager", () => CombatManager.Instance },
            { "DamageNumberManager", () => DamageNumberManager.Instance },
            { "DialogueManager", () => DialogueManager.Instance },
            { "FogManager", () => FogManager.Instance },
            { "GameEventManager", () => GameEventManager.Instance },
            { "GridManager", () => GridManager.Instance },
            { "HeroSelectionManager", () => HeroSelectionManager.Instance },
            { "InvasionManager", () => InvasionManager.Instance },
            { "ItemManager", () => ItemManager.Instance },
            { "JobManager", () => JobManager.Instance },
            { "MainManager", () => MainManager.Instance },
            { "RaceManager", () => RaceManager.Instance },
            { "RaceEventManager", () => RaceEventManager.Instance },
            { "RaceDataManager", () => RaceDataManager.Instance },
            { "ResourceManager", () => ResourceManager.Instance },
            { "SaveManager", () => SaveManager.Instance },
            { "StatisticsManager", () => StatisticsManager.Instance },
            { "TavernFoodManager", () => TavernFoodManager.Instance },
            { "TavernSaveManager", () => TavernSaveManager.Instance },
            { "UnitManager", () => UnitManager.Instance },
            { "WeatherManager", () => WeatherManager.Instance },
            { "WorldManager", () => WorldManager.Instance },
            { "BossSpawner", () => BossSpawner.Instance },
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