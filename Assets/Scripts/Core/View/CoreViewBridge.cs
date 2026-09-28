using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.View.Dtos;

namespace Core.View
{
    public class CoreViewBridge
    {
        private MapRenderState _mapState;
        private readonly object _mapLock = new object();

        private readonly ConcurrentDictionary<int, UnitRenderState> _unitStates = new ConcurrentDictionary<int, UnitRenderState>();
        private readonly ConcurrentDictionary<int, BuildingRenderState> _buildingStates = new ConcurrentDictionary<int, BuildingRenderState>();
        private readonly ConcurrentDictionary<int, FactionResourcesRenderState> _resourceStates = new ConcurrentDictionary<int, FactionResourcesRenderState>();

        // Actualización de Mapa (Sincronizado sin restricción de tipo de referencia)
        public void UpdateMap(MapRenderState newMapState)
        {
            lock (_mapLock)
            {
                _mapState = newMapState;
            }
        }

        public MapRenderState GetMapState()
        {
            lock (_mapLock)
            {
                return _mapState;
            }
        }

        // Unidades
        public void UpdateUnit(UnitRenderState unit) => _unitStates[unit.Id] = unit;
        public void RemoveUnit(int id) => _unitStates.TryRemove(id, out _);

        public void GetUnitsNonAlloc(List<UnitRenderState> results)
        {
            results.Clear();
            foreach (var kvp in _unitStates)
            {
                results.Add(kvp.Value);
            }
        }

        // Edificios
        public void UpdateBuilding(BuildingRenderState building) => _buildingStates[building.Id] = building;
        public void RemoveBuilding(int id) => _buildingStates.TryRemove(id, out _);

        public void GetBuildingsNonAlloc(List<BuildingRenderState> results)
        {
            results.Clear();
            foreach (var kvp in _buildingStates)
            {
                results.Add(kvp.Value);
            }
        }

        // Recursos por Facción
        public void UpdateResources(int factionId, FactionResourcesRenderState res) => _resourceStates[factionId] = res;
        
        public FactionResourcesRenderState GetResources(int factionId)
        {
            _resourceStates.TryGetValue(factionId, out var res);
            return res;
        }
    }
}