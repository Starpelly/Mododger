using BepInEx;
using HarmonyLib;
using Mododger;
using UnityEngine;

namespace ModTest
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class SuperMod : MododgerMod
    {
        public const string pluginGuid = "com.soundodgermodding.supermod";
        public const string pluginName = "SuperMod";
        public const string pluginVersion = "1.0.0";

        public static bool test = false;

        public new void Awake()
        {
            new Harmony(pluginGuid).PatchAll();
        }

        public override void OnSplashAnimationLoad()
        {
            Debug.Log("splashscreenloaded");
            // MDSplashAnimation.BackgroundCamera().backgroundColor = Color.gray;
        }
    }
}
