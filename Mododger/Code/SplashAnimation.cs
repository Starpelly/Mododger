using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mododger
{
    [HarmonyPatch(typeof(SplashAnimation))]
    public class MDSplashAnimation
    {
        private static SplashAnimation splashAnimObj;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void PreStart()
        {
            splashAnimObj = Object.FindObjectOfType<SplashAnimation>();
        }

        public static Camera BackgroundCamera()
        {
            return splashAnimObj.GetComponent<Camera>();
        }
    }
}
