// Core/Controllers/CombatController.cs
using System;
using System.Collections.Concurrent;
using Core.Model.Entities;
using Core.Model.Enums;

namespace Core.Controllers
{
    public class CombatController
    {
        public void OrderAttack(ConcurrentDictionary<int, UnitModel> units, int attackerId, int targetId)
        {
            if (units.TryGetValue(attackerId, out var attacker) && units.ContainsKey(targetId))
            {
                attacker.TargetUnitId = targetId;
                attacker.State = UnitState.Attack;
            }
        }

        public void UpdateCombat(ConcurrentDictionary<int, UnitModel> units, float deltaTime)
        {
            foreach (var unit in units.Values)
            {
                if (unit.State != UnitState.Attack || !unit.TargetUnitId.HasValue)
                    continue;

                if (units.TryGetValue(unit.TargetUnitId.Value, out var target))
                {
                    if (target.Health <= 0)
                    {
                        ResetUnitAttackState(unit);
                        continue;
                    }

                    float dx = target.PositionX - unit.PositionX;
                    float dy = target.PositionY - unit.PositionY;
                    float distSq = dx * dx + dy * dy;

                    float attackRange = unit.AttackRange > 0 ? unit.AttackRange : 1.5f;

                    if (distSq <= attackRange * attackRange)
                    {
                        unit.TargetPositionX = unit.PositionX;
                        unit.TargetPositionY = unit.PositionY;

                        // Aplicar daño asegurando que si el stat Damage es bajo, al menos haga un daño mínimo por golpe o escala real
                        float damageDealt = unit.Damage * deltaTime;
                        
                        // Si el daño resultante es muy bajo por culpa del deltaTime, aplicamos un daño base por frame o el cálculo directo
                        target.Health -= damageDealt > 0 ? damageDealt : unit.Damage;

                        if (target.Health <= 0)
                        {
                            units.TryRemove(target.Id, out _);
                            ResetUnitAttackState(unit);
                        }
                    }
                    else
                    {
                        unit.TargetPositionX = target.PositionX;
                        unit.TargetPositionY = target.PositionY;
                    }
                }
                else
                {
                    ResetUnitAttackState(unit);
                }
            }
        }

        private void ResetUnitAttackState(UnitModel unit)
        {
            unit.TargetUnitId = null;
            unit.State = UnitState.Idle;
            unit.TargetPositionX = unit.PositionX;
            unit.TargetPositionY = unit.PositionY;
        }
    }
}