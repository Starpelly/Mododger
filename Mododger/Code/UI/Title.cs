using System.IO;
using HarmonyLib;
using Mododger.Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mododger
{
    [HarmonyPatch(typeof(Title))]
    public class TitlePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(Title __instance)
        {
            if (Mododger.GameData.titleSplashtext)
            {
                var texts = GameObject.Find("logoAll/logo text");

                var splashJson = File.ReadAllText(Tools.AssetPath("titlesplash.json"));
                var txt = JsonUtility.FromJson<TitleSplash>(splashJson);
                var caption = texts.transform.GetChild(11).GetComponent<TMP_Text>();

                if (txt.allSplashes.Count == 0)
                {
                    caption.text = "wheres the text";
                }
                else
                {
                    var randomSplash = txt.allSplashes[Random.Range(0, txt.allSplashes.Count)];
                    caption.text = randomSplash;

                    caption.text = randomSplash;
                    var maxSize = caption.fontSize / 3;
                    caption.fontSize = maxSize;
                    caption.rectTransform.anchoredPosition = new Vector2(-40, -140);
                    caption.rectTransform.sizeDelta = new Vector2(1200, caption.rectTransform.sizeDelta.y + 200);
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}
