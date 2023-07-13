using HarmonyLib;
using UnityEngine;

namespace Mododger
{
    [HarmonyPatch(typeof(Enemy))]
    public class EnemyPatch
    {
        [HarmonyPatch("shoot")]
        [HarmonyPrefix]
        public static void Shoot(BulletPatternInfo bp = null, bool isStream = false)
        {
            /*
            if (MododgerMain.GameData.beanbox)
            {
                if (!isStream)
                {
                    MonoBehaviour.print("shot bullet " + Time.time);
                    if (bp != null)
                    {
                        if (bp.amount.x >= 4)
                            Beanbox.beanboxClips["4ormore"].Play();
                        else
                            Beanbox.beanboxClips["single"].Play();
                    }
                    Beanbox.beanboxClips["single"].Play();
                }
                else
                {
                    Beanbox.beanboxClips["laser"].Play();
                }
            }
            */
        }
    }
}
