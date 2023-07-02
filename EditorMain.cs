using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mododger
{
    [HarmonyPatch(typeof(EditorMain))]
    public class EditorMainPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            if (LevelData.editorWelcome)
            {
                MododgerMain.SetPresence("", "Editor", "thumb");
            }
        }

        [HarmonyPatch("loaded")]
        [HarmonyPostfix]
        public static void Loaded()
        {
            if (LevelData.type == LevelData.levelType.editor)
                MododgerMain.SetPresence(LevelData.settings.artist + " - " + LevelData.settings.title, "Editor", "thumb", true);
        }

        [HarmonyPatch("toTitle")]
        [HarmonyPrefix]
        public static bool ToTitle()
        {
            if (MododgerMain.OpenedEditorFromWelcome)
            {
                LevelData.type = LevelData.levelType.editor;
                LevelData.resetEditor();
                SceneManager.LoadScene("MainGame");
            }
            else
            {
                MonoBehaviour.print(LevelData.type);
                LevelData.type = LevelData.levelType.user;
                SceneManager.LoadScene("LevelSelect");
            }
            return false;
        }

        [HarmonyPatch("loadLevel")]
        [HarmonyPrefix]
        public static void LoadLevel(string fullPath)
        {
            MododgerMain.OpenedEditorFromWelcome = LevelData.editorWelcome;
        }
    }
}
