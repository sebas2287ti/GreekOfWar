using System;
using System.Collections.Generic;
using Core.Model.Entities;
using Core.Model.Enums;

namespace Core.Controllers
{
    public class IAController
    {
        private const float ATTACK_TRIGGER_RANGE = 6.0f;
        private const float MELEE_ATTACK_RANGE = 1.2f;

        private readonly int _aiFactionId;
        private readonly int _playerFactionId;

        public IAController(int aiFactionId = 2, int playerFactionId = 1)
        {
            _aiFactionId = aiFactionId;
            _playerFactionId = playerFactionId;
        }

        public void UpdateAITurn(Dictionary<int, UnitModel> units, float deltaTime)
        {
            if (units == null || units.Count == 0) return;

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

            if (playerUnits.Count == 0) return;

            foreach (UnitModel aiUnit in aiUnits)
            {
                ProcessUnitBehavior(aiUnit, playerUnits, deltaTime);
            }
        }

        private void ProcessUnitBehavior(UnitModel aiUnit, List<UnitModel> playerUnits, float deltaTime)
        {
            UnitModel closestTarget = FindClosestTarget(aiUnit, playerUnits, out float distanceToTarget);

            if (closestTarget == null) return;

            if (distanceToTarget <= ATTACK_TRIGGER_RANGE)
            {
                if (distanceToTarget <= MELEE_ATTACK_RANGE)
                {
                    ExecuteAttack(aiUnit, closestTarget, deltaTime);
                }
                else
                {
                    MoveTowardsTarget(aiUnit, closestTarget, deltaTime);
                }
            }
            else
            {
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
            float speed = 2.0f;
            float dx = target.PositionX - aiUnit.PositionX;
            float dy = target.PositionY - aiUnit.PositionY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0.01f)
            {
                float dirX = dx / distance;
                float dirY = dy / distance;

                aiUnit.PositionX += dirX * speed * deltaTime;
                aiUnit.PositionY += dirY * speed * deltaTime;
            }
        }

        private void ExecuteAttack(UnitModel attacker, UnitModel target, float deltaTime)
        {
            float attackPower = 15.0f;
            
            target.Health -= attackPower * deltaTime;

            if (target.Health < 0)
            {
                target.Health = 0;
            }
        }
    }
}