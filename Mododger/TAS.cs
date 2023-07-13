/*
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mododger
{
    public class TAS : MonoBehaviour
    {
        public List<Velocity> velocities = new();

        [Serializable]
        public class Velocity
        {
            public float time;
            public float length;
            public float x;
            public float y;
        }

        private string file;

        private void Start()
        {
            SetVelocities();
        }

        public void SetVelocities()
        {
            velocities.Clear();

            file = File.ReadAllText(@"C:\Users\Braedon\Downloads\SDODGER.json");
            velocities = JsonConvert.DeserializeObject<List<Velocity>>(file);

            var totalLength = 0.0f;
            foreach (var t in velocities)
            {
                totalLength += t.length;
                t.time = totalLength;
            }
        }

        public void UpdateTAS()
        {
            var player = GameObject.FindObjectOfType<playerMovement>();
            var game = GameObject.FindObjectOfType<MainGame>();
            var time = game.audioTime; // Note, can be inconsistent, and cause bugs, pls fix

            if (game.needToChoosePractice) return;

            transform.position = Vector3.zero;

            var velocityTime = 0.0f;
            var position = new Vector2();

            var nextVelocityTime = 0.0f;
            var nextPosition = new Vector2();

            for (var i = 0; i < velocities.Count; i++)
            {
                var vel = velocities[i];
                var velTime = velocities[i].time;
                var velVelocity = new Vector2(velocities[i].x, velocities[i].y);

                if (time < velocities[0].time)
                {
                    velocityTime = 0.0f;
                    position = new Vector2();

                    nextVelocityTime = velocities[0].time;
                    nextPosition = new Vector2(velocities[0].x, velocities[0].y);
                }
                else
                {
                    if (time >= velTime)
                    {
                        velocityTime = velTime;
                        position = velVelocity;

                        if (i + 1 < velocities.Count)
                        {
                            nextVelocityTime = velocities[i + 1].time;
                            nextPosition = new Vector2(velocities[i + 1].x, velocities[i + 1].y);
                        }
                        else
                        {
                            nextVelocityTime = 10.0f; // end
                            nextPosition = Vector2.zero;
                        }
                    }
                }
            }
            // position = new Vector3(position.x, 0, position.y);
            // nextPosition = new Vector3(nextPosition.x, 0, nextPosition.y);
            player.transform.position = Vector3.Lerp(new Vector3(position.x, 0, position.y), new Vector3(nextPosition.x, 0, nextPosition.y), Normalize(time, velocityTime, nextVelocityTime));
        }

        public static float Normalize(float val, float min, float max)
        {
            return (val - min) / (max - min);
        }
    }
}
*/