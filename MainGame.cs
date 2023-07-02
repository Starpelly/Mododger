using HarmonyLib;
using System.Reflection;
using System.Text.RegularExpressions;
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
                    __instance.camH = GameObject.Instantiate(new GameObject());

                if (MododgerMain.GameData.discord)
                {
                    var difficulty = string.Empty;

                    if (LevelData.settings.customDiff != string.Empty)
                        difficulty = LevelData.settings.customDiff;
                    else
                        difficulty = LevelData.difficulties[LevelData.settings.difficulty - 1]; // Why is it offseted by 1???

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
        }

        // Some assholes decided to use TMP rich text, so that makes it awkward for discord to parse it.
        // This function removes all that.
        private static string NoRichText(string str)
        {
            var rich = new Regex(@"<[^>]*>");

            if (rich.IsMatch(str))
            {
                str = rich.Replace(str, string.Empty);
            }

            return str;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update(MainGame __instance)
        {
            if (notEditor)
            {
                if (MododgerMain.GameData.firstPersonMode)
                {
                    var vig = (Vignette)(typeof(MainGame).GetField("vig", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
                    vig.center.value = new Vector2(0.5f, 0.5f);
                    typeof(MainGame).GetField("vig", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, vig);
                }
            }
        }
    }
}
