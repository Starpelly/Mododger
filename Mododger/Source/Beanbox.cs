using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;

namespace Mododger
{
    public class Beanbox
    {
        private static GameObject beanboxClipsHolder;
        public static readonly Dictionary<string, AudioSource> beanboxClips = new();

        public static async UniTask Init()
        {
            beanboxClipsHolder = new GameObject
            {
                name = "beanbox"
            };
            Object.DontDestroyOnLoad(beanboxClipsHolder); // I spent 20 minutes figuring out why this wasn't working lol

            var clips = Directory.GetFiles(Tools.AssetPath("beanbox"));
            foreach (var clip in clips)
            {
                var source = beanboxClipsHolder.AddComponent<AudioSource>();
                using (var req = UnityWebRequestMultimedia.GetAudioClip(clip, AudioType.OGGVORBIS))
                {
                    await req.SendWebRequest();

                    source.clip = DownloadHandlerAudioClip.GetContent(req);
                    beanboxClips.Add(Path.GetFileNameWithoutExtension(clip), source);
                }
            }
        }
    }
}
