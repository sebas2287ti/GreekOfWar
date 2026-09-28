using UnityEngine;

namespace UnityAdapter.Views
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Configuración de Movimiento")]
        [SerializeField] private float moveSpeed = 15f;
        [SerializeField] private float sprintMultiplier = 2f;

        [Header("Configuración de Zoom")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoomSize = 5f;
        [SerializeField] private float maxZoomSize = 20f;

        [Header("Límites del Mapa (33x33 celdas)")]
        [SerializeField] private bool useMapBounds = true;
        [SerializeField] private Vector2 minBounds = new Vector2(0f, 0f);
        [SerializeField] private Vector2 maxBounds = new Vector2(33f, 33f);

        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
        }

        private void Update()
        {
            HandleMovement();
            HandleZoom();
            ClampPosition();
        }

     
        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal"); 
            float vertical = Input.GetAxisRaw("Vertical");    

            Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;

           
            float currentSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= sprintMultiplier;
            }

            transform.position += direction * (currentSpeed * Time.deltaTime);
        }
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.01f)
            {
                float targetZoom = _cam.orthographicSize - (scroll * zoomSpeed * 10f);
                _cam.orthographicSize = Mathf.Clamp(targetZoom, minZoomSize, maxZoomSize);
            }
        }


        private void ClampPosition()
        {
            if (!useMapBounds) return;

            Vector3 pos = transform.position;

            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);

            transform.position = pos;
        }


        public void SetMapBounds(float width, float height)
        {
            minBounds = Vector2.zero;
            maxBounds = new Vector2(width, height);
        }
    }
}