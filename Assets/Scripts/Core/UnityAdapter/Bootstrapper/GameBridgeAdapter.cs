using System.Collections.Generic;
using UnityEngine;
using Core.Controllers;
using Core.Model.Entities;
using Core.Model.Enums;
using UnityAdapter.Views;

namespace UnityAdapter.Bootstrapper
{
    public class GameBridgeAdapter : MonoBehaviour
    {
        [Header("Referencias de Vista")]
        [SerializeField] private UnityMapView mapView;

        [Header("Assets de Guerrero (Warrior)")]
        [SerializeField] private Sprite playerWarriorSprite;
        [SerializeField] private Sprite enemyWarriorSprite;

        [Header("Assets de Constructor (Builder)")]
        [SerializeField] private Sprite playerBuilderSprite;
        [SerializeField] private Sprite enemyBuilderSprite;

        public CoreGameController CoreGame { get; private set; }

        private const int PLAYABLE_SIZE = 31;
        private const int TOTAL_SIZE = PLAYABLE_SIZE + 2;

        private readonly Dictionary<int, UnityUnitView> _unitViews = new Dictionary<int, UnityUnitView>();

        private void Awake()
        {
            CoreGame = new CoreGameController(TOTAL_SIZE, TOTAL_SIZE);
        }

        private void Start()
        {
            GenerateMapWithWaterBorders();

            if (mapView != null)
            {
                mapView.RenderMap(CoreGame.Map);
            }

            SpawnTestUnits();
            CoreGame.StartSimulationLoop();
        }

        private void Update()
        {
            SynchronizeUnits();
        }

        private void SpawnTestUnits()
        {
            // 1. Guerrero Jugador (Facción 1) en (5, 5)
            UnitModel playerWarrior = new UnitModel(id: 1, factionId: 1, type: UnitType.Warrior, x: 5f, y: 5f);
            CoreGame.Units.TryAdd(playerWarrior.Id, playerWarrior);
            CreateUnitView(playerWarrior);

            // 2. Constructor Jugador (Facción 1) en (7, 5)
            UnitModel playerBuilder = new UnitModel(id: 2, factionId: 1, type: UnitType.Builder, x: 7f, y: 5f);
            CoreGame.Units.TryAdd(playerBuilder.Id, playerBuilder);
            CreateUnitView(playerBuilder);

            // 3. Guerrero Enemigo (Facción 2) en (25, 25)
            UnitModel enemyWarrior = new UnitModel(id: 3, factionId: 2, type: UnitType.Warrior, x: 25f, y: 25f);
            CoreGame.Units.TryAdd(enemyWarrior.Id, enemyWarrior);
            CreateUnitView(enemyWarrior);
        }

        private void CreateUnitView(UnitModel model)
        {
            GameObject unitObj = new GameObject($"Unit_{model.Id}_{model.Type}_Faction_{model.FactionId}");
            
            unitObj.transform.position = new Vector3(model.PositionX, model.PositionY, -1f);
            unitObj.transform.localScale = Vector3.one;

            SpriteRenderer renderer = unitObj.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 10;

            CircleCollider2D collider = unitObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;

            Rigidbody2D rb = unitObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            UnityUnitView unitView = unitObj.AddComponent<UnityUnitView>();
            unitView.Initialize(
                model, 
                playerWarriorSprite, 
                enemyWarriorSprite, 
                playerBuilderSprite, 
                enemyBuilderSprite
            );

            _unitViews.Add(model.Id, unitView);
        }

        private void SynchronizeUnits()
        {
            List<int> deadUnitIds = new List<int>();

            // 1. Actualizar posición y salud de cada vista activa
            foreach (var kvp in _unitViews)
            {
                int unitId = kvp.Key;
                UnityUnitView view = kvp.Value;

                if (CoreGame.Units.TryGetValue(unitId, out UnitModel model))
                {
                    if (view != null)
                    {
                        view.UpdatePosition(model.PositionX, model.PositionY);
                        view.OnHealthChanged(model.Health, model.MaxHealth);
                    }
                }
                else
                {
                    // La unidad fue removida de la simulación del Core (murió)
                    deadUnitIds.Add(unitId);
                }
            }

            // 2. Destruir los GameObjects y limpiar el diccionario de las unidades muertas
            foreach (int id in deadUnitIds)
            {
                if (_unitViews.TryGetValue(id, out UnityUnitView view))
                {
                    if (view != null)
                    {
                        Destroy(view.gameObject);
                    }
                    _unitViews.Remove(id);
                }
            }
        }

        private void GenerateMapWithWaterBorders()
        {
            for (int x = 0; x < TOTAL_SIZE; x++)
            {
                for (int y = 0; y < TOTAL_SIZE; y++)
                {
                    if (x == 0 || x == TOTAL_SIZE - 1 || y == 0 || y == TOTAL_SIZE - 1)
                    {
                        CoreGame.ChangeCellType(x, y, CellType.Water);
                    }
                    else
                    {
                        CoreGame.ChangeCellType(x, y, CellType.Land);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            CoreGame?.StopSimulationLoop();
        }
    }
}