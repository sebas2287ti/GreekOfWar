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

        /// <summary>
        /// Procesa la entrada WASD o Flechas de dirección.
        /// </summary>
        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D o Flechas Izq/Der
            float vertical = Input.GetAxisRaw("Vertical");     // W/S o Flechas Arriba/Abajo

            Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;

            // Multiplicador de velocidad al presionar Shift Izquierdo
            float currentSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= sprintMultiplier;
            }

            transform.position += direction * (currentSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Procesa el zoom cambiando el valor de Orthographic Size con la rueda del mouse.
        /// </summary>
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.01f)
            {
                float targetZoom = _cam.orthographicSize - (scroll * zoomSpeed * 10f);
                _cam.orthographicSize = Mathf.Clamp(targetZoom, minZoomSize, maxZoomSize);
            }
        }

        /// <summary>
        /// Mantiene la cámara dentro de las coordenadas permitidas del mapa.
        /// </summary>
        private void ClampPosition()
        {
            if (!useMapBounds) return;

            Vector3 pos = transform.position;

            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);

            transform.position = pos;
        }

        /// <summary>
        /// Asigna dinámicamente los límites del mapa según el tamaño del Grid.
        /// </summary>
        public void SetMapBounds(float width, float height)
        {
            minBounds = Vector2.zero;
            maxBounds = new Vector2(width, height);
        }
    }
}