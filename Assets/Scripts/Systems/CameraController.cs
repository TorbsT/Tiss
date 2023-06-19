using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    internal class CameraController : MonoBehaviour
    {
        private Camera cachedCamera;
        [SerializeField] private float panSpeed = 0.02f;
        [SerializeField] private float zoomSpeed = 0.08f;

        private void Awake()
        {
            cachedCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            // Zoom
            float zoom = -Input.mouseScrollDelta.y;
            float newSize = cachedCamera.orthographicSize +
                zoom * cachedCamera.orthographicSize*zoomSpeed;
            newSize = Mathf.Clamp(newSize, 0.1f, 100f);
            cachedCamera.orthographicSize = newSize;

            // Pan
            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");
            Vector2 pos = cachedCamera.transform.position;
            Vector2 newPos = pos + 
                new Vector2(hor, ver)*panSpeed*cachedCamera.orthographicSize;
            cachedCamera.transform.position = new(newPos.x, newPos.y, -10);
        }
    }
}
