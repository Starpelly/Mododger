using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Mododger
{
    [HarmonyPatch(typeof(Options))]
    public class ModsOptions
    {
        private static Options optionsObj => UnityEngine.Object.FindObjectOfType<Options>();

        private static GameObject togglePre = null;

        private static Transform modsContent;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void Start()
        {
            var ver = optionsObj.transform.Find("canvas/stuff").Find("ver");
            var modVer = GameObject.Instantiate(ver, optionsObj.transform.Find("canvas/stuff"));
            modVer.GetComponent<RectTransform>().localPosition = new Vector2(-435, modVer.GetComponent<RectTransform>().localPosition.y);
            modVer.GetComponent<TMP_Text>().text = "mod ver: " + Mododger.pluginVersion;

            GetAllComponents();

            var modsTabs = MakeTab("mods").transform;
            modsTabs.Find("label").GetComponent<TMP_Text>().text = "mods";
            Transform transform2 = optionsObj.transform.Find("canvas/stuff/video");
            modsContent = UnityEngine.Object.Instantiate<Transform>(transform2, transform2.transform.parent);
            modsContent.name = "mods";
            modsContent.gameObject.SetActive(true);
            for (int j = 0; j < modsContent.childCount; j++)
            {
                UnityEngine.Object.Destroy(modsContent.GetChild(j).gameObject);
            }
            modsContent.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
            modsContent.GetComponent<VerticalLayoutGroup>().spacing = 8;

            var scroll = GameObject.Instantiate(new GameObject(), transform2.transform.parent).gameObject.AddComponent<ScrollRect>();
            var scrollRect = scroll.GetComponent<RectTransform>();
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.sizeDelta = new Vector2(800, 650);
            scrollRect.localPosition -= new Vector3(0, 75);
            scroll.gameObject.AddComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0);
            scroll.name = "mods";
            scroll.gameObject.SetActive(true);

            var viewport = GameObject.Instantiate(new GameObject(), scroll.transform);
            viewport.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;

            scroll.viewport = viewport.GetComponent<RectTransform>();
            scroll.content = modsContent.GetComponent<RectTransform>();
            modsContent.SetParent(viewport.transform);
            modsContent.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            for (int i = 0; i < modsContent.childCount; i++)
            {
                modsContent.GetChild(i).SetParent(viewport.transform);
            }

            AddSeperator("Mododger Defaults");

            AddToggle("skip splashscreen", Mododger.GameData.skipSplashscreen);
            AddToggle("first person mode", Mododger.GameData.firstPersonMode);
            AddToggle("ignore arena", Mododger.GameData.ignoreArena);
            AddToggle("auto restart on hit", Mododger.GameData.autoRestart);
            AddToggle("remove domes", Mododger.GameData.removeDomes);
            AddToggle("title splashtext", Mododger.GameData.titleSplashtext);
            AddToggle("LIBERAL MODE", Mododger.GameData.liberalMode);
        }

        private static void ScaleMyRect(ref RectTransform myRect)
        {
            //If you want the middle of the rect be somewhere else then the middle of the screen change it here (0 ... 1, 0 ... 1)
            Vector2 rectMiddle = new Vector2(0.5f, 0.5f);

            float horizontalSize = 0.5f; //50% of horizontal screen used
            float verticalSize = 0.75f; //75%  of vertical screen used

            myRect.sizeDelta = Vector2.zero; //Dont want any delta sizes, because that would defeat the point of anchors
            myRect.anchoredPosition = Vector2.zero; //And the position is set by the anchors aswell so we set the offset to 0

            myRect.anchorMin = new Vector2(rectMiddle.x - horizontalSize / 2,
                                        rectMiddle.y - verticalSize / 2);
            myRect.anchorMax = new Vector2(rectMiddle.x + horizontalSize / 2,
                                        rectMiddle.y + verticalSize / 2);
        }

        [HarmonyPatch("changeTab")]
        [HarmonyPrefix]
        public static void ChangeTab(string name, ref string ___currTab)
        {
            Debug.Log(___currTab);
            if (___currTab != name)
            {
                ___currTab = name;
                optionsObj.transform.Find("canvas/stuff/tabs/audio").GetComponent<OptionsInput>().select(___currTab == "audio");
                optionsObj.transform.Find("canvas/stuff/tabs/video").GetComponent<OptionsInput>().select(___currTab == "video");
                optionsObj.transform.Find("canvas/stuff/tabs/gameplay").GetComponent<OptionsInput>().select(___currTab == "gameplay");
                optionsObj.transform.Find("canvas/stuff/tabs/mods").GetComponent<OptionsInput>().select(___currTab == "mods");

                optionsObj.transform.Find("canvas/stuff/audio").gameObject.SetActive(___currTab == "audio");
                optionsObj.transform.Find("canvas/stuff/video").gameObject.SetActive(___currTab == "video");
                optionsObj.transform.Find("canvas/stuff/gameplay").gameObject.SetActive(___currTab == "gameplay");
                optionsObj.transform.Find("canvas/stuff/mods").gameObject.SetActive(___currTab == "mods");
                SFX.Play("menu_changeBtn", 1f, false, 1f);
            }
        }

        [HarmonyPatch("toggleClicked")]
        [HarmonyPrefix]
        public static void ToggleClicked(Transform obj)
        {
            if (obj.parent.name == "skip splashscreen")
            {
                Mododger.GameData.skipSplashscreen = !Mododger.GameData.skipSplashscreen;
                ToggleInvoke(new object[] { obj, Mododger.GameData.skipSplashscreen, true });
                Mododger.UpdateData();
            }
            if (obj.parent.name == "first person mode")
            {
                Mododger.GameData.firstPersonMode = !Mododger.GameData.firstPersonMode;
                if (SceneManager.GetActiveScene().name == "MainGame")
                {
                    Mododger.MainGame.restart();
                    SFX.Play("pause_submit", 1f, false, 1f);
                }
                ToggleInvoke(new object[] { obj, Mododger.GameData.firstPersonMode, true });
                Mododger.UpdateData();
            }
            if (obj.parent.name == "ignore arena")
            {
                Mododger.GameData.ignoreArena = !Mododger.GameData.ignoreArena;
                ToggleInvoke(new object[] { obj, Mododger.GameData.ignoreArena, true });
                Mododger.UpdateData();
            }
            if (obj.parent.name == "auto restart on hit")
            {
                Mododger.GameData.autoRestart = !Mododger.GameData.autoRestart;
                ToggleInvoke(new object[] { obj, Mododger.GameData.autoRestart, true });
                Mododger.UpdateData();
            }
            if (obj.parent.name == "remove domes")
            {
                Mododger.GameData.removeDomes = !Mododger.GameData.removeDomes;
                ToggleInvoke(new object[] { obj, Mododger.GameData.removeDomes, true });
                Mododger.UpdateData();
            }
            if (obj.parent.name == "title splashtext")
            {
                Mododger.GameData.titleSplashtext = !Mododger.GameData.titleSplashtext;
                ToggleInvoke(new object[] { obj, Mododger.GameData.titleSplashtext, true });
                Mododger.UpdateData();
            }
            if (obj.parent.name == "LIBERAL MODE")
            {
                Mododger.GameData.liberalMode = !Mododger.GameData.liberalMode;
                ToggleInvoke(new object[] { obj, Mododger.GameData.liberalMode, true });
                Mododger.UpdateData();
            }
        }

        public static void AddSeperator(string label)
        {
            GameObject tmp = GameObject.Instantiate(optionsObj.transform.Find("canvas/stuff/title").gameObject, modsContent);
            tmp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(540, 100);
            var text = tmp.GetComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 45;
            text.alignment = TextAlignmentOptions.CenterGeoAligned;
        }

        #region Toggles

        public static void AddToggle(string label, bool show)
        {
            GameObject toggle = GameObject.Instantiate(togglePre, modsContent);
            toggle.name = label;
            toggle.SetActive(true);
            toggle.GetComponent<TMP_Text>().text = label;

            CanvasGroup component = toggle.transform.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
            component.alpha = (float)(show ? 1 : 0);
        }

        public static void ToggleInvoke(object[] parameters)
        {
            var f = typeof(Options).GetMethod("toggleToggle", BindingFlags.NonPublic | BindingFlags.Instance);
            f.Invoke(optionsObj, parameters);
        }

        #endregion

        #region Private Helpers

        private static GameObject MakeTab(string name)
        {
            Transform ass = optionsObj.transform.Find("canvas/stuff/tabs/video");
            ass.transform.parent.GetComponent<RectTransform>().anchoredPosition += new Vector2(100f, 0f);
            ass.transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(250f, 0f);

            Transform modsTabs = UnityEngine.Object.Instantiate<Transform>(ass.transform, ass.transform.parent);
            modsTabs.gameObject.name = name;
            return modsTabs.gameObject;
        }

        private static void GetAllComponents()
        {
            var content = optionsObj.transform.Find("canvas/stuff/video");
            for (int k = 0; k < content.childCount; k++)
            {
                GameObject child = content.GetChild(k).gameObject;
                if (content.GetChild(k).gameObject.name == "vsync")
                {
                    togglePre = UnityEngine.Object.Instantiate<GameObject>(child, content);
                }
            }
        }

        #endregion
    }
}
