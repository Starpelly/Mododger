using HarmonyLib;
using Mododger;
using UnityEngine;

namespace ModTest
{
    [HarmonyPatch(typeof(Options))]
    public class OptionsPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            return;
            ModsOptions.AddSeperator("SuperMod");
            ModsOptions.AddToggle("test", false);
            ModsOptions.AddToggle("test", false);
            ModsOptions.AddToggle("test", false);
            ModsOptions.AddToggle("test", false);
            ModsOptions.AddToggle("test", false);
        }

        [HarmonyPatch("toggleClicked")]
        [HarmonyPrefix]
        public static void ToggleClicked(Transform obj)
        {
            if (obj.parent.name == "test")
            {
                SuperMod.test = !SuperMod.test;
                ModsOptions.ToggleInvoke(new object[] { obj, SuperMod.test, true });
                // Mododger.ModsOptions.UpdateData();
            }
        }
    }
}
