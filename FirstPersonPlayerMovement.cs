using UnityEngine;

namespace Mododger
{
    public class FirstPersonPlayerMovement : MonoBehaviour
    {
        private static float xRotation = 0f;
        private static float yRotation = 0f;
        private static Transform camera;

        private float originalY;
        private float yVelocity = 0f;

        public void Start()
        {
            camera = GameObject.Find("camH").transform;
            camera.transform.SetParent(transform, false);

            if (!MododgerMain.MainGame.inEditor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            var canvas = GameObject.Find("GameCanvas");

            originalY = transform.localPosition.y;
        }

        public void UpdateFPS(playerMovement playerMovement)
        {
            camera.transform.localPosition = new Vector3(0, 1f, 0);

            var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 400;
            var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 400;
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");

            var moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
            var speed = (Input.GetKey(KeyCode.LeftShift)) ? 12f : 5f;
            transform.position += moveDirection.normalized * speed * Time.deltaTime;

            for (int i = 0; i < camera.childCount; i++)
            {
                var cam = camera.transform.GetChild(i).GetComponent<Camera>();
                if (cam != null) cam.fieldOfView = 90f;
            }

            var enemyHolder = GameObject.Find("enemyH").transform;
            if (enemyHolder != null)
            {
                for (int i = 2; i < enemyHolder.childCount; i++)
                {
                    var ball = enemyHolder.GetChild(i).GetChild(0);
                    var flare = enemyHolder.GetChild(i).GetChild(2);
                    ball.localEulerAngles = new Vector3(-90f, 0, 0);
                    flare.localPosition = new Vector3(ball.localPosition.x, -5f, ball.localPosition.z);
                    flare.localEulerAngles = new Vector3(0, 0, 0);
                }
            }

            var bullets = FindObjectsOfType<BulletMovement>();
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].transform.localPosition = new Vector3(bullets[i].transform.localPosition.x, 0, bullets[i].transform.localPosition.z);
            }

            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                yVelocity = 24.09f * Time.deltaTime;
            }
            yVelocity -= Time.deltaTime * 0.5f;
            transform.localPosition += new Vector3(0f, 1f * yVelocity);*/

            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, originalY, Mathf.Infinity), transform.localPosition.z);
        }
    }
}
