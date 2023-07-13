using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;

namespace Mododger
{
    public class Hitboxes : ImmediateModeShapeDrawer
    {
        public MainGame mainGame;
        
        // For some reason, God knows why, Spheres do not work. So I had to use a disc and use it as a billboard.
        // How sad...
        public override void DrawShapes(Camera cam)
        {
            if (!MododgerMain.GameData.showHitboxes || MododgerMain.GameData.firstPersonMode) return;
            
            using (Draw.Command(cam))
            {
                Draw.Color = new Color(1, 0, 0, 0.45f);
                
                var bullets = mainGame.bullets;
                foreach (var bullet in bullets)
                {
                    if (bullet.type == LevelData.bType.bubble)
                    {
                        var col = bullet.transform.Find("bubble").GetComponent<SphereCollider>();
                        Draw.Disc(col.bounds.center, Quaternion.Euler(90, 0, 0), col.bounds.extents.x);
                    }
                    else if (bullet.type == LevelData.bType.arrow)
                    {
                        var col = bullet.transform.Find("arrow").GetComponent<SphereCollider>();
                        Draw.Disc(col.bounds.center, Quaternion.Euler(90, 0, 0), col.bounds.extents.x);
                    }
                    else if (bullet.type == LevelData.bType.homing)
                    {
                        var col = bullet.transform.Find("homing").GetComponent<SphereCollider>();
                        Draw.Disc(col.bounds.center, Quaternion.Euler(90, 0, 0), col.bounds.extents.x);
                    }
                    else if (bullet.type == LevelData.bType.plus)
                    {
                        var plusObj = bullet.transform.Find("plus").GetChild(0);
                        var cols = plusObj.GetComponents<BoxCollider>();
                        foreach (var col in cols)
                        {
                            Draw.Rectangle(col.bounds.center, Quaternion.Euler(90f, col.transform.eulerAngles.y, 0), Vector3.Scale(new Vector3(col.size.x, col.size.z, col.size.y), new Vector3(col.transform.lossyScale.x, col.transform.lossyScale.z, col.transform.lossyScale.y)));
                        }
                    }
                    else if (bullet.type == LevelData.bType.ripple)
                    {
                        var col = bullet.transform.Find("ripple").GetComponent<BoxCollider>();
                        Draw.Rectangle(col.bounds.center, Quaternion.Euler(90f, col.transform.eulerAngles.y, 0), Vector3.Scale(new Vector3(col.size.x, col.size.z, col.size.y), new Vector3(col.transform.lossyScale.x, col.transform.lossyScale.z, col.transform.lossyScale.y)));
                    }
                    else if (bullet.type == LevelData.bType.laser)
                    {
                        var col = bullet.transform.Find("laser").GetChild(1).GetComponent<BoxCollider>();
                        Draw.Rectangle(col.bounds.center, Quaternion.Euler(90f, col.transform.eulerAngles.y, 0), Vector3.Scale(new Vector3(col.size.x, col.size.y * 0.5f, col.size.y), new Vector3(col.transform.lossyScale.x, col.transform.lossyScale.z, col.transform.lossyScale.y)));
                    }
                    else if (bullet.type == LevelData.bType.rubber)
                    {
                        var col = bullet.transform.Find("rubber").GetComponent<SphereCollider>();
                        Draw.Disc(col.bounds.center, Quaternion.Euler(90, 0, 0), col.bounds.extents.x);
                    }
                }
            }
        }
    }
}
