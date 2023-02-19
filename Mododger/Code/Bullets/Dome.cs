using System.Reflection;
using HarmonyLib;

namespace Mododger
{
    [HarmonyPatch(typeof(Dome))]
    public class DomePatch
    {
        /// <summary>
        /// Removes domes if "removeDomes" is true.
        /// </summary>
        [HarmonyPatch("activate")]
        [HarmonyPrefix]
        public static void RemoveDomesOnActivate(Dome __instance)
        {
            if (Mododger.GameData.removeDomes)
            {
                GameState.notify("domes are dumb", 0.25f, Notification.types.normal, false, true);
                var dyMeth = typeof(Dome).GetMethod("removeSelf", BindingFlags.NonPublic | BindingFlags.Instance);
                dyMeth.Invoke(__instance, new object[] { });
            }    
        }
    }
}
