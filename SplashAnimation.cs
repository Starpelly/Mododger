using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mododger
{
    [HarmonyPatch(typeof(SplashAnimation))]
    public class SplashAnimationPatch
    {
        private static SplashAnimation splashAnimObj => Object.FindObjectOfType<SplashAnimation>();

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static bool PreStart()
        {
            if (MododgerMain.GameData.skipSplashscreen)
            {
                splashAnimObj.GetComponent<Camera>().backgroundColor = splashAnimObj.grey;
                splashAnimObj.gameObject.SetActive(false);
                SceneManager.LoadScene("Title");
                return false;
            }
            return true;
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            if (MododgerMain.GameData.skipSplashscreen) return;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool Update()
        {
            if (MododgerMain.GameData.skipSplashscreen) return false;
            return true;
        }
    }
}
