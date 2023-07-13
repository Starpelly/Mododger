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
            if (!MododgerMain.GameData.skipSplashscreen) return true;
            
            splashAnimObj.GetComponent<Camera>().backgroundColor = splashAnimObj.grey;
            splashAnimObj.gameObject.SetActive(false);
            SceneManager.LoadScene("Title");
            
            return false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool Update()
        {
            return !MododgerMain.GameData.skipSplashscreen;
        }
    }
}
