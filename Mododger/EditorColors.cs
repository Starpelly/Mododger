/*
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Mododger
{
    [HarmonyPatch(typeof(EditorColors))]
    public class EditorColorsPatch
    {
        [HarmonyPatch("boxClick")]
        [HarmonyPrefix]
        public static bool BoxClick(EditorColors __instance, Transform btn, int mouseBtn)
        {
            var copiedColor = (Color)typeof(EditorColors).GetField("copiedColor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var editor = (EditorMain)typeof(EditorColors).GetField("editor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            if (mouseBtn == 0)
            {
                __instance.pickToggle(btn);
                return false;
            }
            if (mouseBtn == 1)
            {
                if (copiedColor != default(Color))
                {
                    ColorInputItem componentInParent = btn.GetComponentInParent<ColorInputItem>();
                    if (componentInParent.myType == ColorInputItem.types.c && (componentInParent.ct == LevelSettings.cType.floor || componentInParent.ct == LevelSettings.cType.score || componentInParent.ct == LevelSettings.cType.outline))
                    {
                        btn.GetComponent<Image>().color = copiedColor;
                        if (componentInParent.ct == LevelSettings.cType.floor)
                        {
                            editor.main.arenaFloor.Color = SoundUtils.colorAlpha(__instance.localGColors[componentInParent.ct][componentInParent.currSet - 1], copiedColor.a);
                        }
                        else if (componentInParent.ct == LevelSettings.cType.score)
                        {
                            editor.main.scoreCircle.Color = SoundUtils.colorAlpha(__instance.localGColors[componentInParent.ct][componentInParent.currSet - 1], copiedColor.a);
                        }
                        else if (componentInParent.ct == LevelSettings.cType.outline)
                        {
                            Color outlineCol = SoundUtils.colorAlpha(__instance.localGColors[componentInParent.ct][componentInParent.currSet - 1], copiedColor.a);
                            var apOC = typeof(EditorColors).GetMethod("applyOutlineColor", BindingFlags.NonPublic | BindingFlags.Instance);
                            apOC.Invoke(__instance, new object[] { outlineCol });
                        }
                    }
                    else
                    {
                        btn.GetComponent<Image>().color = SoundUtils.colorAlpha(copiedColor, 1f);
                    }
                    componentInParent.input.text = ColorUtility.ToHtmlStringRGB(copiedColor);
                    return false;
                }
            }
            else if (mouseBtn == 2)
            {
                typeof(EditorColors).GetField("copiedColor", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, btn.GetComponent<UnityEngine.UI.Image>().color);
            }

            return false;
        }

        [HarmonyPatch("pickToggle")]
        [HarmonyPostfix]
        public static void PickToggle(EditorColors __instance, Transform btn = null)
        {
            var slider = (Slider)typeof(EditorColors).GetField("slider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var activePreview = (Image)typeof(EditorColors).GetField("activePreview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            var componentInParent = btn.GetComponentInParent<ColorInputItem>();
            if (componentInParent.myType == ColorInputItem.types.b)
            {
                slider.transform.parent.gameObject.SetActive(true);
                slider.value = (float)Mathf.RoundToInt(activePreview.color.a * 100f);
            }
        }

        [HarmonyPatch("OnSliderValueChanged")]
        [HarmonyPostfix]
        public static void OnSliderValueChanged(EditorColors __instance, float value)
        {
            var activePreview = (Image)typeof(EditorColors).GetField("activePreview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var editor = (EditorMain)typeof(EditorColors).GetField("editor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            if (Enum.GetNames(typeof(LevelData.bType)).Contains(activePreview.transform.parent.name))
            {
                var bulletType = (LevelData.bType)Enum.Parse(typeof(LevelData.bType), activePreview.transform.parent.name);
                LevelData.settings.bColors[bulletType][0] = activePreview.color;
            }
        }
    }
}
*/