using HarmonyLib;
using System.Reflection;

namespace Mododger
{
    [HarmonyPatch(typeof(Dome))]
    public class DomePatch
    {
        [HarmonyPatch("activate")]
        [HarmonyPostfix]
        public static void RemoveDomesOnActivate(Dome __instance)
        {
            if (MododgerMain.GameData.removeDomes)
            {
                __instance.gameObject.SetActive(false);
                // var dyMeth = typeof(Dome).GetMethod("removeSelf", BindingFlags.NonPublic | BindingFlags.Instance);
                // dyMeth.Invoke(__instance, new object[] { });
            }
        }
    }
}
