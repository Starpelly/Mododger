using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;
using Mododger.Code.Firstperson;

namespace Mododger
{
    [HarmonyPatch(typeof(playerMovement))]
    public class PlayerMovementPatch : MonoBehaviour
    {
        private static float radius = 5f;
        private static FirstPersonPlayerMovement fps;
        private static float mouseSpeed;
        public static float originalMouseSpeed = 25;
        private static bool startedWithFirstperson = false;

        private static bool goingBackwards = false;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(playerMovement __instance)
        {
            startedWithFirstperson = Mododger.GameData.firstPersonMode;
            if (startedWithFirstperson)
            {
                fps = __instance.gameObject.AddComponent<FirstPersonPlayerMovement>();
            }
            originalMouseSpeed = GameData.mouseSpeed;
        }

        /// <summary>
        /// Stores the local radius to the radius of the arena so we can set it back in "UpdateAfter".
        /// Not setting it back has the consequence of bullets not destroying when exiting the arena.
        /// </summary>
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update(playerMovement __instance, ref PlayerInput ___input)
        {
            if (Mododger.GameData.ignoreArena)
            {
                radius = __instance.GetComponent<MainGame>().radius;
                __instance.GetComponent<MainGame>().radius = Mathf.Infinity;
            }

            mouseSpeed = GameData.mouseSpeed;

            if (startedWithFirstperson)
            {
                ___input.enabled = false;
                fps.UpdateFPS(__instance);
                GameData.mouseSpeed = 0f;
            }
            else
            {
                if (fps != null) GameObject.Destroy(fps);
            }
        }

        /// <summary>
        /// Sets arena size back after the PlayerMovement Update code has concluded.
        /// </summary>
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdateAfter(playerMovement __instance)
        {
            __instance.GetComponent<MainGame>().radius = radius;

            GameData.mouseSpeed = mouseSpeed;
        }
    }
}
