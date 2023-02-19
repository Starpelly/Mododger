using BepInEx;
using UnityEngine.SceneManagement;

namespace Mododger
{
    public abstract class MododgerMod : BaseUnityPlugin
    {
        public void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "Splash":
                    OnSplashAnimationLoad();
                    break;
            }
        }

        public virtual void OnSplashAnimationLoad()
        {

        }
    }
}
