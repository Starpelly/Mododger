using System.Net;
using HarmonyLib;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Mododger
{
    [HarmonyPatch(typeof(MainGame))]
    public class MainGamePatch
    {
        public static bool notEditor => LevelData.type != LevelData.levelType.editor;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(MainGame __instance)
        {
            if (notEditor)
            {
                if (MododgerMain.GameData.firstPersonMode)
                    __instance.camH = Object.Instantiate(new GameObject());

                if (MododgerMain.GameData.discord)
                {
                    string difficulty;

                    if (LevelData.settings.customDiff != string.Empty)
                        difficulty = LevelData.settings.customDiff;
                    else
                        difficulty = LevelData.difficulties[LevelData.settings.difficulty - 1]; // Why is it offsetted by 1???

                    var designer = difficulty + " by: " + LevelData.settings.designer;
                    var name = LevelData.settings.artist + " - " + LevelData.settings.title;

                    designer = NoRichText(designer);
                    name = NoRichText(name);

                    var thumb = "thumb";
                    // I was gonna make a thumbnail for every main level in the game, but that shit took too long.
                    if (LevelData.type == LevelData.levelType.main)
                    {
                    }
                    MododgerMain.SetPresence(designer, name, thumb, true);
                }
            }

            var hitboxes = __instance.camH.transform.GetChild(3).gameObject.AddComponent<Hitboxes>();
            hitboxes.mainGame = __instance;
        }

        // Some assholes decided to use TMP rich text, so that makes it awkward for discord to parse it.
        // This function removes all that.
        private static string NoRichText(string str)
        {
            var rich = new Regex(@"<[^>]*>");

            var retur = str;
            if (rich.IsMatch(str))
            {
                retur = rich.Replace(str, string.Empty);
            }

            return retur;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update(MainGame __instance)
        {
            if (notEditor)
            {
                if (MododgerMain.GameData.firstPersonMode)
                {
                    var vig = (Vignette)(typeof(MainGame).GetField("vig", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance));
                    if (vig != null)
                    {
                        vig.center.value = new Vector2(0.5f, 0.5f);
                        typeof(MainGame).GetField("vig", BindingFlags.NonPublic | BindingFlags.Instance)
                            ?.SetValue(__instance, vig);
                    }
                }
            }
            
            __instance.camH.transform.GetChild(3).GetComponent<Camera>().enabled = !MododgerMain.GameData.hideBullets;
            __instance.camH.transform.GetChild(4).GetComponent<Camera>().enabled = !MododgerMain.GameData.hideBullets;
        }
        
        [HarmonyPatch("getHit")]
        [HarmonyPostfix]
        public static void GetHit(PlayerInfo pl, GameObject obj = null, bool hugOut = false)
        {
        }

        [HarmonyPatch("restart")]
        [HarmonyPostfix]
        public static void Restart(bool notify = true)
        {
            // GameObject.FindObjectOfType<TAS>().SetVelocities();
        }
    }
}
