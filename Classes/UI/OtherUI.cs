using System;
using UnityEngine;
using static ModKit.UI;

namespace RandomThings {
    public static class OtherUI {
        public static Settings settings => Main.settings;
        public static void OnGUI() {
            Label("Hello");
        }
    }
}
