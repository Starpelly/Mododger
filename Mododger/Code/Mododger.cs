using System.Collections.Generic;
using System.IO;
using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mododger
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Mododger : BaseUnityPlugin
    {
        public const string pluginGuid = "com.soundodgermodding.mododger";
        public const string pluginName = "Mododger";
        public const string pluginVersion = "1.1.0";

        private static GUIStyle style = GUIStyle.none;

        public static ModGameData GameData;

        public static MainGame MainGame => GameObject.FindObjectOfType<MainGame>();

        public void Awake()
        {
            style.fontSize = 32;
            var font = Resources.Load("lang/fonts/" + "Century Gothic SDF") as TMP_FontAsset;
            style.font = font.sourceFontFile;

            if (File.Exists(ModGameData.GetJsonFile()))
                GameData = ModGameData.ReadFromJson();
            else
                GameData = new ModGameData();

            new Harmony(pluginGuid).PatchAll();
        }


        public static void SetOptions()
        {

        }

        public void OnGUI()
        {
            // GUI.Label(new Rect(0, 0, 400, 200), "Mododger v" + pluginVersion, style);
            // GUI.Label(new Rect(0, 60, 400, 200), "no", style);
        }

        /// <summary>
        /// Updates the chosen data to the configuration file.
        /// </summary>
        public static void UpdateData()
        {
            ModGameData.SaveToJson(GameData);
        }

        public static Texture2D LoadPNG(string filePath)
        {
            byte[] fileData;
            Texture2D tex = null;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }
    }
}