using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Mododger.Code
{
    [HarmonyPatch(typeof(LevelSelectMain))]
    public class LevelSelectMainPatch
    {
        public static string[] LIBERALS = new string[] 
        {
            "Seventeen & Blunting",
            "Outta Vote",
            "Crypto Bubble Yuck",
            "Lost in Transition",
            "Bones and Portland, Oregon",
            "maga",
            "Sixteen & Socialist",
            "The Phantom Politics",
            "Image of November",
            "Illumination Pictures Entertainment",
            "ridin4biden",
            "Hepatitis",
            "clintonWALK.temp",
            "safety with no gun",
            "Sophomore Loan Forgiveness",
            "Evening",
            "Nagasaki",
            "Socially Distanced Crowds",
            "Can't Dodge My Pronouns!",
            "Modular Sexual Orientation",
            "Talk About Inflation",
            "Stroke, I'm dying",
            "ShinShe/Herjuku",
            "Shared Biden Memory",
            "Sigilvitotalitarian"
        };

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(LevelSelectMain __instance, ref List<GameObject> ___tickets)
        {
            if (Mododger.GameData.liberalMode && GameObject.Find("menuBtns/menuLabel").GetComponent<TMP_Text>().text == "main levels")
            {
                for (int i = 0; i < ___tickets.Count; i++)
                {
                    var ticket = ___tickets[i].GetComponent<LevelTicket>();
                    ticket.info.title = LIBERALS[i];
                    var dyMeth = typeof(LevelTicket).GetField("titleTxt", BindingFlags.NonPublic | BindingFlags.Instance);
                    var tmp = ((TMP_Text)dyMeth.GetValue(ticket)).text = LIBERALS[i];
                }
            }
        }
    }
}
