using DiscordRPC;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mododger
{
    [HarmonyPatch(typeof(Title))]
    public class TitlePatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            MododgerMain.SetPresence("", "Title", "thumb");
        }
    }
}
