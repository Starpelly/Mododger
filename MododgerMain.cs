using BepInEx;
using HarmonyLib;
using System.IO;
using TMPro;
using UnityEngine;
using DiscordRPC;

namespace Mododger;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
public class MododgerMain : BaseUnityPlugin
{
    public const string pluginGuid = "com.soundodgermodding.mododger";
    public const string pluginName = "Mododger";
    public const string pluginVersion = "1.2.0";

    private static GUIStyle style = GUIStyle.none;

    public static bool OpenedEditorFromWelcome = false;
    public static ModGameData GameData;

    public static MainGame MainGame => GameObject.FindObjectOfType<MainGame>();

    private static DiscordRpcClient DiscordClient;
    private static RichPresence CurrentRichPresence;

    private void Awake()
    {
        if (File.Exists(ModGameData.GetJsonFile()))
            GameData = ModGameData.ReadFromJson();
        else
            GameData = new ModGameData();

        style.fontSize = 32;
        var font = Resources.Load("lang/fonts/" + "Century Gothic SDF") as TMP_FontAsset;
        style.font = font.sourceFontFile;

        new Harmony(pluginGuid).PatchAll();

        Beanbox.Init();

        if (GameData.discord)
        {
            EnableDiscord();
        }
    }

    private void Update()
    {
        if (DiscordClient != null)
            DiscordClient.Invoke();
    }

    private void OnApplicationQuit()
    {
        DiscordClient.ClearPresence();
        DiscordClient.Dispose();
    }

    public void OnGUI()
    {
        // GUI.Label(new Rect(0, 0, 400, 200), "Mododger v" + pluginVersion, style);
        // GUI.Label(new Rect(0, 60, 400, 200), "no", style);
    }

    /// <summary>
    /// Updates the chosen data to the configuration file.
    /// </summary>
    public static void UpdateData()
    {
        ModGameData.SaveToJson(GameData);
    }

    public static void SetPresence(string state, string details, string largeImageKey, bool startCounter = false)
    {
        CurrentRichPresence = new RichPresence()
        {
            State = state,
            Details = details,
            Assets = new Assets()
            {
                LargeImageKey = largeImageKey
            }
        };
        if (startCounter)
            CurrentRichPresence.Timestamps = Timestamps.Now;

        if (!GameData.discord) return;
        DiscordClient.SetPresence(CurrentRichPresence);
    }

    public static void EnableDiscord()
    {
        if (DiscordClient == null)
            DiscordClient = new DiscordRpcClient("1125083189380137060");

        DiscordClient.Initialize();
        if (CurrentRichPresence != null)
            DiscordClient.SetPresence(CurrentRichPresence);
    }

    public static void DisableDiscord()
    {
        DiscordClient.ClearPresence();
        DiscordClient.Deinitialize();
    }
}
