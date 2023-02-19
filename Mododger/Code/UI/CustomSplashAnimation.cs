using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using System.Reflection;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;

namespace Mododger
{
    [HarmonyPatch(typeof(SplashAnimation))]
    public class SplashAnimationPatch
    {
        public static Color color;
        public static float sat = 0f;

        private static SplashAnimation splashAnimObj => Object.FindObjectOfType<SplashAnimation>();

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void PreStart()
        {
            if (Mododger.GameData.skipSplashscreen)
            {
                SceneManager.LoadScene("Title");
                return;
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            if (Mododger.GameData.skipSplashscreen) return;

            GameObject.Find("logo").SetActive(false);
            GameObject gameObject = new GameObject("ass");
            var texture = Mododger.LoadPNG(Tools.AssetPath("splash.png"));
            gameObject.AddComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 100f);

            splashAnimObj.logo = gameObject.transform;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            var fi = typeof(SplashAnimation).GetField("timer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(splashAnimObj);
            var timer = (float)fi;

            float num = 4f;
            float num2 = 1f - Mathf.Pow(Mathf.Cos(timer * Mathf.PI * (2f / num)) * 0.5f + 0.5f, 1f);

            sat = Mathf.Lerp((timer < 2f) ? 0 : 0.5f, 0.5f, num2);

            color = ShiftHueBy(color, 0.95f * Time.deltaTime);
            splashAnimObj.GetComponent<Camera>().backgroundColor = Color.Lerp((timer < 2f) ? Color.black : splashAnimObj.grey, color, num2);
            splashAnimObj.logo.transform.localScale = Vector3.Slerp((timer < 2f) ? new Vector2(0.14f, 0.185f) : new Vector2(0.14f, 0.185f), new Vector2(0.215f, 0.235f), num2);
        }

        private static Color ShiftHueBy(Color color, float amount)
        {
            // convert from RGB to HSV
            Color.RGBToHSV(color, out float hue, out float sat, out float val);

            // shift hue by amount
            hue += amount;
            sat = SplashAnimationPatch.sat;
            val = 1f;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, sat, val);
        }
    }
}
