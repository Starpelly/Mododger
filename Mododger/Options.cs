using HarmonyLib;
using System.Reflection;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

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
            modVer.GetComponent<RectTransform>().localPosition = new Vector2(-440, modVer.GetComponent<RectTransform>().localPosition.y);
            modVer.GetComponent<TMP_Text>().text = "mod ver " + MododgerMain.pluginVersion;

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
            scrollRect.localPosition -= new Vector3(0, 129);
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

            AddToggle("skip splashscreen", MododgerMain.GameData.skipSplashscreen);
            AddToggle("first person mode", MododgerMain.GameData.firstPersonMode);
            AddToggle("ignore arena", MododgerMain.GameData.ignoreArena);
            AddToggle("remove domes", MododgerMain.GameData.removeDomes);
            AddToggle("discord rich presence", MododgerMain.GameData.discord);
            AddToggle("show bullet hitboxes", MododgerMain.GameData.showHitboxes);
            AddToggle("hide bullets", MododgerMain.GameData.hideBullets);
            AddToggle("lock player in editor", MododgerMain.GameData.lockPlayerInEditor);
            // AddToggle("animated hearts", MododgerMain.GameData.animatedHearts);
            AddToggle("fix editor lines", MododgerMain.GameData.fixEditorLines);
            // AddToggle("beanbox", MododgerMain.GameData.beanMode);
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
                MododgerMain.GameData.skipSplashscreen = !MododgerMain.GameData.skipSplashscreen;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.skipSplashscreen, true });
                MododgerMain.UpdateData();
            }
            if (obj.parent.name == "first person mode")
            {
                MododgerMain.GameData.firstPersonMode = !MododgerMain.GameData.firstPersonMode;
                if (SceneManager.GetActiveScene().name == "MainGame" && MainGamePatch.notEditor)
                {
                    MododgerMain.MainGame.restart();
                    SFX.Play("pause_submit", 1f, false, 1f);
                }
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.firstPersonMode, true });
                MododgerMain.UpdateData();

                if (MododgerMain.GameData.firstPersonMode)
                    GameState.notify("half life 4", 1.5f, Notification.types.normal, false, true);
            }
            if (obj.parent.name == "ignore arena")
            {
                MododgerMain.GameData.ignoreArena = !MododgerMain.GameData.ignoreArena;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.ignoreArena, true });
                MododgerMain.UpdateData();

                if (MododgerMain.GameData.ignoreArena)
                    GameState.notify("become ungovernable", 1.5f, Notification.types.normal, false, true);
                else
                    GameState.notify("appeal to authority much?", 1.5f, Notification.types.normal, false, true);
            }
            if (obj.parent.name == "remove domes")
            {
                MododgerMain.GameData.removeDomes = !MododgerMain.GameData.removeDomes;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.removeDomes, true });
                MododgerMain.UpdateData();

                if (MododgerMain.GameData.removeDomes)
                    GameState.notify("domes are dumb, i agree", 1.5f, Notification.types.normal, false, true);
                else
                    GameState.notify("actually, domes aren't all that bad", 1.5f, Notification.types.normal, false, true);
            }
            if (obj.parent.name == "discord rich presence")
            {
                MododgerMain.GameData.discord = !MododgerMain.GameData.discord;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.discord, true });
                MododgerMain.UpdateData();

                if (MododgerMain.GameData.discord)
                    MododgerMain.EnableDiscord();
                else
                    MododgerMain.DisableDiscord();
            }
            if (obj.parent.name == "show bullet hitboxes")
            {
                MododgerMain.GameData.showHitboxes = !MododgerMain.GameData.showHitboxes;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.showHitboxes, true });
                MododgerMain.UpdateData();
            }
            if (obj.parent.name == "hide bullets")
            {
                MododgerMain.GameData.hideBullets = !MododgerMain.GameData.hideBullets;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.hideBullets, true });
                MododgerMain.UpdateData();
            }
            if (obj.parent.name == "lock player in editor")
            {
                MododgerMain.GameData.lockPlayerInEditor = !MododgerMain.GameData.lockPlayerInEditor;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.lockPlayerInEditor, true });
                MododgerMain.UpdateData();
            }
            if (obj.parent.name == "animated hearts")
            {
                MododgerMain.GameData.animatedHearts = !MododgerMain.GameData.animatedHearts;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.animatedHearts, true });
                MododgerMain.UpdateData();
            }
            if (obj.parent.name == "fix editor lines")
            {
                MododgerMain.GameData.fixEditorLines = !MododgerMain.GameData.fixEditorLines;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.fixEditorLines, true });
                MododgerMain.UpdateData();

                if (MododgerMain.GameData.fixEditorLines)
                    GameState.notify("warning: line colors may be off", 2.75f, Notification.types.problem, false, true);
                // GameState.modal(ModalWindow.types.notes, Color.yellow, "Warning: Colors may be off.", "OK", "", null, null, "l", "");
            }
            /*
            if (obj.parent.name == "beanbox")
            {
                MododgerMain.GameData.beanMode = !MododgerMain.GameData.beanMode;
                ToggleInvoke(new object[] { obj, MododgerMain.GameData.beanMode, true });
                MododgerMain.UpdateData();

                if (MododgerMain.GameData.beanMode && Beanbox.beanboxClips.Count < 3)
                    GameState.notify("some clips for beanbox are missing, might not work correctly.", 2.5f, Notification.types.problem, false, true);
            }
            */
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
