using ModKit.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Xml.Serialization;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace RandomThings {
    public class Settings : UnityModManager.ModSettings {
        public int selectedTab = 0;
        public int selectedRawDataType;
        public override void Save(ModEntry modEntry) {
            string path = GetPath(modEntry);
            try {
                using StreamWriter textWriter = new StreamWriter(path);
                new XmlSerializer(typeof(Settings), (XmlAttributeOverrides)null).Serialize(textWriter, this);
            } catch (Exception e) {
                modEntry.Logger.Error("Can't save " + path + ".");
                modEntry.Logger.LogException(e);
            }
        }
    }
}
