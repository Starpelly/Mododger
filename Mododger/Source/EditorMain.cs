using HarmonyLib;
using Shapes;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mododger
{
    [HarmonyPatch(typeof(EditorMain))]
    public class EditorMainPatch
    {
        public static Camera LinesCamera;
        public static RenderTexture Texture;
        public static GameObject textureHolder;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            if (LevelData.editorWelcome)
            {
                MododgerMain.SetPresence("", "Editor", "thumb");
            }

            LinesCamera = GameObject.Instantiate(GameObject.Find("editorCam")).GetComponent<Camera>();
            LinesCamera.name = "linesCamera";
            LinesCamera.clearFlags = CameraClearFlags.Color;
            LinesCamera.backgroundColor = new Color(1, 1, 1, 0);
            LinesCamera.allowHDR = false;

            Texture = new RenderTexture(1920, 1080, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            //Texture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat;
            // Texture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNORM;
            Texture.filterMode = FilterMode.Bilinear;
           
            LinesCamera.targetTexture = Texture;

            var editorCanvas = GameObject.Find("Editor/Canvas");

            textureHolder = GameObject.Instantiate(editorCanvas.transform.Find("tools").gameObject, editorCanvas.transform);

            // This was ANNOYING.
            // I had to do this, or else it WOULDN'T WORK!!!
            for (var i = 0; i < textureHolder.transform.childCount; i++)
            {
                var child = textureHolder.transform.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }

            textureHolder.name = "textureHolder";
            textureHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(1600, 900);
            textureHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            var rawImg = textureHolder.AddComponent<RawImage>();
            rawImg.texture = Texture;
            rawImg.raycastTarget = false;

            textureHolder.transform.SetParent(editorCanvas.transform.Find("wave"));
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            // textureHolder.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
            textureHolder.transform.SetAsLastSibling(); // Makes sure it always appears on top of everything else.
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

        [HarmonyPatch("pauseToggle")]
        [HarmonyPostfix]
        public static void PauseToggle()
        {
            // GameObject.FindObjectOfType<TAS>().SetVelocities();
        }

        [HarmonyPatch("drawEventLines")]
        [HarmonyPrefix]
        public static bool DrawEventLines(EditorMain __instance, EventData.eventType type = EventData.eventType.none, bool ghost = false)
        {
            if (!MododgerMain.GameData.fixEditorLines) return true;

            if (type == EventData.eventType.none)
            {
                type = __instance.eventType;
            }

            var waveYBot = (float)typeof(EditorMain).GetField("waveYBot", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var waveYTop = (float)typeof(EditorMain).GetField("waveYTop", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            int pointCount = Mathf.Clamp(Mathf.RoundToInt(Mathf.Pow(__instance.waveform.transform.localScale.x / 50f * 24f, 1.3f)), 4, 24);
            float y = SoundUtils.remap(__instance.valToY(LevelData.defEventVal(type), type), waveYBot, waveYTop, -53.5f, -17f, false);
            using (Draw.Command(LinesCamera, CameraEvent.BeforeImageEffects))
            {
                using (PolylinePath polylinePath = new PolylinePath())
                {
                    Vector3 vector = new Vector3(__instance.waveform.transform.position.x, y, 89.9f);
                    polylinePath.AddPoint(new PolylinePoint(vector));
                    int num = 0;
                    Vector2 rhs = new Vector2(-100f, -100f);
                    AnchorPoint anchorPoint = __instance.dummyAP;
                    anchorPoint.transform.position = vector;
                    foreach (GameObject gameObject in __instance.currEventList(type))
                    {
                        AnchorPoint component = gameObject.GetComponent<AnchorPoint>();
                        var anchorYFix = typeof(EditorMain).GetMethod("anchorYFix", BindingFlags.NonPublic | BindingFlags.Instance);
                        var vector2 = (Vector2)anchorYFix.Invoke(__instance, new object[] { component.GetComponent<RectTransform>().anchoredPosition });

                        // Vector2 vector2 = __instance.anchorYFix(component.GetComponent<RectTransform>().anchoredPosition);

                        if (vector2 != rhs)
                        {
                            if (anchorPoint.smoothOut || component.smoothIn)
                            {
                                Vector3 point = (!anchorPoint.smoothOut) ? anchorPoint.transform.position : new Vector3((component.transform.position.x + anchorPoint.transform.position.x) * 0.5f, anchorPoint.transform.position.y, anchorPoint.transform.position.z);
                                Vector3 point2 = (!component.smoothIn) ? component.transform.position : new Vector3((component.transform.position.x + anchorPoint.transform.position.x) * 0.5f, component.transform.position.y, anchorPoint.transform.position.z);
                                polylinePath.BezierTo(new PolylinePoint(point), new PolylinePoint(point2), new PolylinePoint(component.transform.position), pointCount);
                            }
                            else
                            {
                                polylinePath.AddPoint(new PolylinePoint(component.transform.position));
                            }
                            rhs = new Vector2(vector2.x, vector2.y);
                            anchorPoint = component;
                            num++;
                        }
                    }
                    vector.x = __instance.waveform.transform.position.x + __instance.waveform.transform.localScale.x * 179.62f;
                    if (anchorPoint.smoothOut)
                    {
                        Vector3 point3 = (!anchorPoint.smoothOut) ? anchorPoint.transform.position : new Vector3((vector.x + anchorPoint.transform.position.x) * 0.5f, anchorPoint.transform.position.y, anchorPoint.transform.position.z);
                        polylinePath.BezierTo(new PolylinePoint(point3), new PolylinePoint(vector), new PolylinePoint(vector), pointCount);
                    }
                    else
                    {
                        polylinePath.AddPoint(new PolylinePoint(vector));
                    }

                    var getGhostColor = typeof(EditorMain).GetMethod("getGhostColor", BindingFlags.NonPublic | BindingFlags.Instance);
                    var eventLineColor = (Color)typeof(EditorMain).GetField("eventLineColor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                    Draw.Polyline(polylinePath, false, ghost ? 0.3f : 0.5f, ghost ? (Color)getGhostColor.Invoke(__instance, new object[] { type }) : eventLineColor);
                }
            }


            return false;
        }
    }
}
