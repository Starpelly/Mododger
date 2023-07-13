using HarmonyLib;

namespace Mododger
{
    [HarmonyPatch(typeof(Title))]
    public class TitlePatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            MododgerMain.SetPresence("", "Title", "thumb");
        }
    }
}
