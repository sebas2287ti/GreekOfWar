using UnityEngine;
using Core.Model.Entities;
using Core.Model.Enums;
using UnityAdapter.Bootstrapper;
using UnityAdapter.Views;

namespace UnityAdapter.Controllers
{
    public class PlayerInputController : MonoBehaviour
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

            // Convertir la posición del mouse en pantalla a coordenadas 2D del mundo
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // Buscar cualquier Collider 2D que esté exactamente bajo el cursor
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos2D);

            if (hitCollider != null)
            {
                UnityUnitView unitView = hitCollider.GetComponent<UnityUnitView>();

                // Solo seleccionar si es de la Facción 1 (Jugador)
                if (unitView != null && unitView.FactionId == 1)
                {
                    SelectUnit(unitView);
                    return;
                }
            }

            // Si hicimos clic en el suelo o en un enemigo, deseleccionar
            DeselectUnit();
        }

        private void HandleRightClick()
        {
            if (SelectedUnitView == null || bridgeAdapter == null) return;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // Redondear posición al centro o decimal más cercano en la grilla
            float targetX = Mathf.Round(mouseWorldPos.x * 10f) / 10f;
            float targetY = Mathf.Round(mouseWorldPos.y * 10f) / 10f;

            // Enviar orden al Core de la simulación
            if (bridgeAdapter.CoreGame.Units.TryGetValue(SelectedUnitView.UnitId, out UnitModel unitModel))
            {
                unitModel.TargetPositionX = targetX;
                unitModel.TargetPositionY = targetY;
                unitModel.State = UnitState.Moving;
                unitModel.TargetUnitId = -1; // Cancelar objetivo de ataque previo
                
                Debug.Log($"[RTS] Orden de movimiento enviada a Unidad {unitModel.Id} -> ({targetX}, {targetY})");
            }
        }

        private void SelectUnit(UnityUnitView unitView)
        {
            DeselectUnit();
            SelectedUnitView = unitView;

            if (SelectedUnitView.TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = Color.yellow; // Resaltado visual de selección
            }

            Debug.Log($"[RTS] Unidad seleccionada: ID {unitView.UnitId}");
        }

        private void DeselectUnit()
        {
            if (SelectedUnitView != null)
            {
                if (SelectedUnitView.TryGetComponent(out SpriteRenderer sr))
                {
                    sr.color = Color.white; // Restaurar color
                }
                SelectedUnitView = null;
            }
        }
    }
}