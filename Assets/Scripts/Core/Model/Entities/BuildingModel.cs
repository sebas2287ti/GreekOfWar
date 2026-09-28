// Core/Model/Entities/BuildingModel.cs
using Core.Model.Enums;

namespace Core.Model.Entities
{
    public class BuildingModel
    {
        public int Id { get; set; }
        public int FactionId { get; set; }
        public BuildingType Type { get; set; }
        
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        
        // Producción de recursos pasivos por segundo
        public float ResourceGenerationRate { get; set; } = 5f;

        // Constructor principal que requiere el SystemController
        public BuildingModel(int id, int factionId, BuildingType type, float positionX, float positionY, float maxHealth = 100f)
        {
            Id = id;
            FactionId = factionId;
            Type = type;
            PositionX = positionX;
            PositionY = positionY;
            MaxHealth = maxHealth;
            Health = maxHealth;
        }
    }
}   