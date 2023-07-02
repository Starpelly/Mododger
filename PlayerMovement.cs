﻿using HarmonyLib;
using UnityEngine;

namespace Mododger
{
    [HarmonyPatch(typeof(playerMovement))]
    public class PlayerMovementPatch
    {
        private static float radius = 5f;

        private static FirstPersonPlayerMovement fps;
        private static float mouseSpeed;
        public static float originalMouseSpeed = 25;
        private static bool startedWithFirstperson = false;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(playerMovement __instance)
        {
            if (MainGamePatch.notEditor)
            {
                startedWithFirstperson = MododgerMain.GameData.firstPersonMode;
                if (startedWithFirstperson)
                {
                    fps = __instance.gameObject.AddComponent<FirstPersonPlayerMovement>();
                    GameObject.Find("ball_sns").SetActive(false);
                }
                else
                {
                    if (fps != null) GameObject.Destroy(fps);
                }
                originalMouseSpeed = GameData.mouseSpeed;
            }
        }

        /// <summary>
        /// Stores the local radius to the radius of the arena so we can set it back in "UpdateAfter".
        /// Not setting it back has the consequence of bullets not destroying when exiting the arena.
        /// </summary>
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update(playerMovement __instance, ref PlayerInput ___input)
        {
            if (MododgerMain.GameData.ignoreArena)
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
                ___input.enabled = true;
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
