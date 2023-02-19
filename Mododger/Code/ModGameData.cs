
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mododger
{
    public class ModGameData
    {
        public bool skipSplashscreen;
        public bool ignoreArena;
        public bool autoRestart;
        public bool removeDomes;
        public bool titleSplashtext;
        public bool firstPersonMode;
        public bool liberalMode;

        public List<Mod> mods = new List<Mod>();

        public class Mod
        {
            public string guid;
        }

        public static void SaveToJson(ModGameData m)
        {
            var json = JsonUtility.ToJson(m);
            Debug.Log(m.ignoreArena);
            File.WriteAllText(GetJsonFile(), json);
        }

        public static ModGameData ReadFromJson()
        {
            var json = File.ReadAllText(GetJsonFile());
            return JsonUtility.FromJson<ModGameData>(json);
        }

        public static string GetJsonFile()
        {
            return Tools.AssetPath("data.json");
        }
    }
}
