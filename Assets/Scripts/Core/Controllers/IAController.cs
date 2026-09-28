using System;
using System.Collections.Generic;
using Core.Model.Entities;
using Core.Model.Enums;

namespace Core.Controllers
{
    public class IAController
    {
        private const float ATTACK_TRIGGER_RANGE = 6.0f; // Rango de detección: 6 casillas
        private const float MELEE_ATTACK_RANGE = 1.2f;   // Rango para infligir daño físico

        private readonly int _aiFactionId;
        private readonly int _playerFactionId;

        public IAController(int aiFactionId = 2, int playerFactionId = 1)
        {
            _aiFactionId = aiFactionId;
            _playerFactionId = playerFactionId;
        }

        /// <summary>
        /// Evalúa y ejecuta las acciones de todas las unidades de la IA.
        /// Este método debe ser llamado desde el bucle principal de simulación del Core.
        /// </summary>
        public void UpdateAITurn(Dictionary<int, UnitModel> units, float deltaTime)
        {
            if (units == null || units.Count == 0) return;

            // Separar unidades enemigas (IA) y unidades del jugador
            List<UnitModel> aiUnits = new List<UnitModel>();
            List<UnitModel> playerUnits = new List<UnitModel>();

            foreach (var kvp in units)
            {
                UnitModel unit = kvp.Value;
                if (unit.Health <= 0) continue;

                if (unit.FactionId == _aiFactionId)
                {
                    aiUnits.Add(unit);
                }
                else if (unit.FactionId == _playerFactionId)
                {
                    playerUnits.Add(unit);
                }
            }

            // Si no hay objetivos del jugador, las unidades de la IA no actúan
            if (playerUnits.Count == 0) return;

            // Procesar cada unidad controlada por la IA
            foreach (UnitModel aiUnit in aiUnits)
            {
                ProcessUnitBehavior(aiUnit, playerUnits, deltaTime);
            }
        }

        private void ProcessUnitBehavior(UnitModel aiUnit, List<UnitModel> playerUnits, float deltaTime)
        {
            // 1. Buscar la unidad del jugador más cercana
            UnitModel closestTarget = FindClosestTarget(aiUnit, playerUnits, out float distanceToTarget);

            if (closestTarget == null) return;

            // 2. Verificar si está dentro del rango de percepción de 6 casillas
            if (distanceToTarget <= ATTACK_TRIGGER_RANGE)
            {
                // Si ya está en distancia de cuerpo a cuerpo (ataque)
                if (distanceToTarget <= MELEE_ATTACK_RANGE)
                {
                    ExecuteAttack(aiUnit, closestTarget, deltaTime);
                }
                else
                {
                    // Si está entre 1.2 y 6 casillas, avanza hacia la unidad del jugador
                    MoveTowardsTarget(aiUnit, closestTarget, deltaTime);
                }
            }
            else
            {
                // Fuera del rango de 6 casillas: La IA ignora al objetivo y permanece quieta
                // (Se puede añadir patrulla o comportamiento idle si se requiere)
            }
        }

        private UnitModel FindClosestTarget(UnitModel aiUnit, List<UnitModel> playerUnits, out float minDistance)
        {
            UnitModel closest = null;
            minDistance = float.MaxValue;

            foreach (UnitModel target in playerUnits)
            {
                float dx = target.PositionX - aiUnit.PositionX;
                float dy = target.PositionY - aiUnit.PositionY;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = target;
                }
            }

            return closest;
        }

        private void MoveTowardsTarget(UnitModel aiUnit, UnitModel target, float deltaTime)
        {
            float speed = 2.0f; // Velocidad de movimiento en casillas por segundo
            float dx = target.PositionX - aiUnit.PositionX;
            float dy = target.PositionY - aiUnit.PositionY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0.01f)
            {
                // Normalizar dirección
                float dirX = dx / distance;
                float dirY = dy / distance;

                // Actualizar posición en el modelo del Core
                aiUnit.PositionX += dirX * speed * deltaTime;
                aiUnit.PositionY += dirY * speed * deltaTime;
            }
        }

        private void ExecuteAttack(UnitModel attacker, UnitModel target, float deltaTime)
        {
            float attackPower = 15.0f; // Daño infligido
            
            // Infligir daño reduciendo la salud del modelo en el Core
            target.Health -= attackPower * deltaTime;

            if (target.Health < 0)
            {
                target.Health = 0;
            }
        }
    }
}