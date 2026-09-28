// Core/Controllers/CoreGameController.cs
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Entities;
using Core.Model.Enums;
using Core.Model.Map;

namespace Core.Controllers
{
    public class CoreGameController
    {
        // --- ESTADO DEL JUEGO ---
        public GridMap Map { get; private set; }
        public ConcurrentDictionary<int, UnitModel> Units { get; } = new ConcurrentDictionary<int, UnitModel>();
        public ConcurrentDictionary<int, BuildingModel> Buildings => _systemController.Buildings; // Expone los edificios del system controller
        public ConcurrentDictionary<int, FactionResources> Factions { get; } = new ConcurrentDictionary<int, FactionResources>();

        // --- CONTROLADORES EXTERNOS ---
        private readonly CombatController _combatController;
        private readonly SystemController _systemController; // <--- Nuevo controlador integrado

        // --- BUCLE MULTIHILO ---
        private CancellationTokenSource _cts;
        private bool _isRunning;

        public CoreGameController(int mapWidth = 32, int mapHeight = 32)
        {
            Map = new GridMap(mapWidth, mapHeight);
            _combatController = new CombatController();
            _systemController = new SystemController(); // <--- Inicializamos el system controller

            // Inicializar facciones por defecto (1: Jugador, 2: IA)
            Factions.TryAdd(1, new FactionResources(1, 100f)); // Jugador con 100 de oro inicial
            Factions.TryAdd(2, new FactionResources(2, 100f)); // IA
        }

        #region Control del Bucle de Simulación

        public void StartSimulationLoop(int targetFps = 30)
        {
            if (_isRunning) return;
            _isRunning = true;
            _cts = new CancellationTokenSource();

            int frameDelay = 1000 / targetFps;
            float deltaTime = frameDelay / 1000f;

            Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    UpdateSimulation(deltaTime);
                    await Task.Delay(frameDelay, _cts.Token);
                }
            }, _cts.Token);
        }

        public void StopSimulationLoop()
        {
            _isRunning = false;
            _cts?.Cancel();
        }

        public void UpdateSimulation(float deltaTime)
        {
            UpdateMovement(deltaTime);
            
            // Delegamos el combate al CombatController
            _combatController.UpdateCombat(Units, deltaTime);
            
            UpdateIA();

            // <--- NUEVO: El SystemController actualiza la economía y la producción pasiva de los edificios
            _systemController.UpdateEconomy(Factions, deltaTime);
        }

        #endregion

        #region Lógica del Mapa y Edificios

        public bool ChangeCellType(int x, int y, CellType newType)
        {
            if (!Map.IsValidPosition(x, y)) return false;
            return Map.SetCellType(x, y, newType);
        }

        public void SetRegionType(int startX, int startY, int width, int height, CellType type)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    ChangeCellType(x, y, type);
                }
            }
        }

        public void ExpandMap(int additionalWidth, int additionalHeight)
        {
            int newWidth = Map.Width + additionalWidth;
            int newHeight = Map.Height + additionalHeight;
            Map.ExpandMap(newWidth, newHeight);
        }

        // <--- NUEVO: Método para ordenar la construcción desde un constructor
       public bool OrderBuildStructure(int builderId, float targetX, float targetY, BuildingType buildingType)
        {
            if (Units.TryGetValue(builderId, out var builder))
            {
                // Definir costos según el tipo de edificio
                float cost = (buildingType == BuildingType.Base) ? 100f : 50f;

                // Llamada limpia pasando el buildingType y el cost en el orden correcto
                bool success = _systemController.TryBuildStructure(Factions, builder.FactionId, buildingType, targetX, targetY, cost);

                if (success)
                {
                    builder.TargetPositionX = targetX;
                    builder.TargetPositionY = targetY;
                    builder.State = UnitState.Moving;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Sistemas Internos (Movimiento e IA)

        private void UpdateMovement(float deltaTime)
        {
            foreach (var unit in Units.Values)
            {
                if (unit.State != UnitState.Moving && unit.State != UnitState.Attack)
                    continue;

                float dx = unit.TargetPositionX - unit.PositionX;
                float dy = unit.TargetPositionY - unit.PositionY;
                float distanceSq = dx * dx + dy * dy;

                if (distanceSq > 0.01f)
                {
                    float distance = (float)Math.Sqrt(distanceSq);
                    float moveDist = unit.MovementSpeed * deltaTime;

                    if (moveDist >= distance)
                    {
                        unit.PositionX = unit.TargetPositionX;
                        unit.PositionY = unit.TargetPositionY;
                        if (unit.State == UnitState.Moving)
                            unit.State = UnitState.Idle;
                    }
                    else
                    {
                        unit.PositionX += (dx / distance) * moveDist;
                        unit.PositionY += (dy / distance) * moveDist;
                    }
                }
            }
        }

        public void OrderAttackUnit(int attackerId, int targetId)
        {
            _combatController.OrderAttack(Units, attackerId, targetId);
        }

        private void UpdateIA()
        {
            const float detectionRange = 6.0f;
            float detectionRangeSq = detectionRange * detectionRange;

            foreach (var unit in Units.Values)
            {
                if (unit.FactionId != 2) continue; // Facción 2 = IA

                if (unit.State == UnitState.Idle)
                {
                    foreach (var enemy in Units.Values)
                    {
                        if (enemy.FactionId != 2 && enemy.Health > 0)
                        {
                            float dx = enemy.PositionX - unit.PositionX;
                            float dy = enemy.PositionY - unit.PositionY;
                            float distSq = dx * dx + dy * dy;

                            // Solo ataca si el enemigo está a una distancia de 6 casillas o menos
                            if (distSq <= detectionRangeSq)
                            {
                                unit.TargetUnitId = enemy.Id;
                                unit.State = UnitState.Attack;
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}