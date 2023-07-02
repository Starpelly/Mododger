using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using DiscordRPC;

namespace Mododger
{
    [HarmonyPatch(typeof(LevelSelectMain))]
    public class LevelSelectMainPatch
    {
        private static LevelSelectMain levelSelectMainObj => Object.FindObjectOfType<LevelSelectMain>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            MododgerMain.SetPresence((LevelData.type == LevelData.levelType.user) ? "User Levels" : "Main Levels", "Level Select", "thumb");
        }

        [HarmonyPatch(typeof(LevelSelectMain), "makeTicket", new System.Type[] { typeof(List<LevelSettings>), typeof(string), typeof(LevelData.musicType), typeof(LevelSettings.unlockTypes), typeof(int) })]
        [HarmonyPrefix]
        public static bool MakeTicket(List<LevelSettings> infos, string thumbPath, LevelData.musicType musicType, LevelSettings.unlockTypes unlockType, int unlockAmt)
        {
            var updateThumbnailLater = false;
            Texture2D tex = new Texture2D(2, 2);
            if (LevelData.type == LevelData.levelType.main)
                tex = Resources.Load<Texture2D>(thumbPath + "_sm");
            else if (thumbPath != "")
            {
                updateThumbnailLater = true;
            }
            else
            {
                tex = Resources.Load<Texture2D>("prefabs/UI/levelSelect/noThumb");
            }

            var tickets = (List<GameObject>)typeof(LevelSelectMain).GetField("tickets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(levelSelectMainObj);
            var ticketH = (Transform)typeof(LevelSelectMain).GetField("ticketH", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(levelSelectMainObj);

            GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("prefabs/UI/ticket"), ticketH) as GameObject;

            SoundUtils.setAxisPos(gameObject.transform, VectorAxis.x, 0f, true);
            gameObject.GetComponent<LevelTicket>().setInfos(infos, tex, levelSelectMainObj, tickets.Count, musicType, unlockType, unlockAmt);
            gameObject.GetComponent<LevelTicket>().levelSelect = levelSelectMainObj;
            gameObject.GetComponent<LevelTicket>().thumbPath = thumbPath;
            tickets.Add(gameObject);

            if (updateThumbnailLater)
            {
                // ticketsToUpdateThumbnail.Add(gameObject.GetComponent<LevelTicket>());
                LoadThumbnail(gameObject.GetComponent<LevelTicket>(), thumbPath);
            }

            return false; // Return false to skip the original method.
        }

        public static async UniTask LoadThumbnail(LevelTicket ticket, string thumbPath)
        {
            using (var req = UnityWebRequestTexture.GetTexture(thumbPath, true))
            {
                await req.SendWebRequest();
                MonoBehaviour.print("loaded texture");
                ticket.thumbImg.texture = DownloadHandlerTexture.GetContent(req);
            }
        }

        [HarmonyPatch("leave")]
        [HarmonyPrefix]
        public static void Leave(string where)
        {
            if (LevelData.type == LevelData.levelType.editor)
                MododgerMain.OpenedEditorFromWelcome = false;
        }
    }
}
