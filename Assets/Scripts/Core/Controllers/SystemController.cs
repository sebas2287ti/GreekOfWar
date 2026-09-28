using System.Collections.Concurrent;
using Core.Model.Entities;
using Core.Model.Enums;

namespace Core.Controllers
{
    public class SystemController
    {
        public ConcurrentDictionary<int, BuildingModel> Buildings { get; } = new ConcurrentDictionary<int, BuildingModel>();

        public bool TryBuildStructure(ConcurrentDictionary<int, FactionResources> factions, int factionId, BuildingType type, float x, float y, float cost)
        {
            if (factions.TryGetValue(factionId, out var factionResources))
            {
                if (factionResources.Spend(cost))
                {
                    int newBuildingId = Buildings.Count + 1;
                    
                    // Asignar generación pasiva según el tipo (Base genera más, Barracks menos o nada)
                    float genRate = (type == BuildingType.Base) ? 10f : 2f;

                    var newBuilding = new BuildingModel(
                        newBuildingId, 
                        factionId, 
                        type, 
                        x, 
                        y, 
                        maxHealth: 200f
                    )
                    {
                        ResourceGenerationRate = genRate
                    };

                    Buildings.TryAdd(newBuildingId, newBuilding);
                    return true;
                }
            }
            return false;
        }

        public void UpdateEconomy(ConcurrentDictionary<int, FactionResources> factions, float deltaTime)
        {
            foreach (var building in Buildings.Values)
            {
                if (building.Health > 0 && factions.TryGetValue(building.FactionId, out var faction))
                {
                    faction.Add(building.ResourceGenerationRate * deltaTime);
                }
            }
        }
    }
}