using Core.Model.Enums;

namespace Core.Model.Entities
{
    public class UnitModel
    {
        public int Id { get; set; }
        public int FactionId { get; set; }
        public UnitType Type { get; set; }
        public UnitState State { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public float AttackRange { get; set; }
        public float MovementSpeed { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        
        // Propiedades de navegación y objetivo
        public float TargetPositionX { get; set; }
        public float TargetPositionY { get; set; }
        public int? TargetUnitId { get; set; }

        public UnitModel(int id, int factionId, UnitType type, float x, float y)
        {
            Id = id;
            FactionId = factionId;
            Type = type;
            State = UnitState.Idle;
            PositionX = x;
            PositionY = y;
            TargetPositionX = x;
            TargetPositionY = y;

            if (type == UnitType.Warrior)
            {
                MaxHealth = 100f;
                Health = 100f;
                Damage = 15f;
                AttackRange = 1.5f;
                MovementSpeed = 4f;
            }
            else // Builder
            {
                MaxHealth = 50f;
                Health = 50f;
                Damage = 2f;
                AttackRange = 1f;
                MovementSpeed = 3f;
            }
        }
    }
}