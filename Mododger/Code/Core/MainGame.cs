using HarmonyLib;
using UnityEngine;
using System.Reflection;
using UnityEngine.Rendering.PostProcessing;

namespace Mododger
{
    [HarmonyPatch(typeof(MainGame))]
    public class MainGamePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(MainGame __instance)
        {
            if (Mododger.GameData.firstPersonMode)
                __instance.camH = GameObject.Instantiate(new GameObject());
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update(MainGame __instance)
        {
            if (Mododger.GameData.firstPersonMode)
            {
                var vig = (Vignette)(typeof(MainGame).GetField("vig", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
                vig.center.value = new Vector2(0.5f, 0.5f);
                typeof(MainGame).GetField("vig", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, vig);
            }
        }

        /// <summary>
        /// Calls auto-restart
        /// </summary>
        [HarmonyPatch("getHit")]
        [HarmonyPrefix]
        public static void GetHit(MainGame __instance)
        {
            if (Mododger.GameData.autoRestart && !__instance.inEditor)
            {
                __instance.restart();
                SFX.Play("pause_submit", 1f, false, 1f);
                SFX.Play("game_loseHeart", 1f, false, 1f);
            }
        }
    }
}
