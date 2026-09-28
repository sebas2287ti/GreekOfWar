// UnityAdapter/Controllers/RTSPlayerInputController.cs
using UnityEngine;
using Core.Model.Entities;
using Core.Model.Enums;
using UnityAdapter.Bootstrapper;
using UnityAdapter.Views;

namespace UnityAdapter.Controllers
{
    public class RTSPlayerInputController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameBridgeAdapter bridgeAdapter;
        [SerializeField] private Camera mainCamera;

        public UnityUnitView SelectedUnitView { get; private set; }

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (bridgeAdapter == null)
            {
                bridgeAdapter = GetComponent<GameBridgeAdapter>();
            }
        }

        private void Update()
        {
            // Clic Izquierdo: Seleccionar Unidad
            if (Input.GetMouseButtonDown(0))
            {
                HandleLeftClick();
            }

            // Clic Derecho: Ordenar Movimiento
            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
        }

        private void HandleLeftClick()
        {
            if (mainCamera == null) return;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // Detección directa del Collider 2D bajo el puntero
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos2D);

            if (hitCollider != null)
            {
                UnityUnitView unitView = hitCollider.GetComponent<UnityUnitView>();

                // Seleccionar solo si pertenece a la Facción 1 (Jugador)
                if (unitView != null && unitView.FactionId == 1)
                {
                    SelectUnit(unitView);
                    return;
                }
            }

            // Clic en área vacía deselecciona
            DeselectUnit();
        }

        private void HandleRightClick()
        {
            if (SelectedUnitView == null || bridgeAdapter == null) return;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            float targetX = Mathf.Round(mouseWorldPos.x * 10f) / 10f;
            float targetY = Mathf.Round(mouseWorldPos.y * 10f) / 10f;

            if (bridgeAdapter.CoreGame.Units.TryGetValue(SelectedUnitView.UnitId, out UnitModel unitModel))
            {
                unitModel.TargetPositionX = targetX;
                unitModel.TargetPositionY = targetY;
                unitModel.State = UnitState.Moving;
                unitModel.TargetUnitId = -1; // Cancelar objetivo de ataque previo

                Debug.Log($"[RTS] Orden enviada a Unidad {unitModel.Id} -> ({targetX}, {targetY})");
            }
        }

        private void SelectUnit(UnityUnitView unitView)
        {
            DeselectUnit();
            SelectedUnitView = unitView;

            if (SelectedUnitView.TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = Color.yellow; // Resaltado amarillo
            }

            Debug.Log($"[RTS] Unidad seleccionada ID: {unitView.UnitId}");
        }

        private void DeselectUnit()
        {
            if (SelectedUnitView != null)
            {
                if (SelectedUnitView.TryGetComponent(out SpriteRenderer sr))
                {
                    sr.color = Color.white; // Restaurar color normal
                }
                SelectedUnitView = null;
            }
        }
    }
}