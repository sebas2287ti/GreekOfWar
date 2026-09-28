using System.Collections.Generic;
using Core.Model.Enums;

namespace Core.View.Dtos
{
    public readonly struct UnitRenderState
    {
        public int Id { get; }
        public int FactionId { get; }
        public UnitType Type { get; }
        public UnitState State { get; }
        public float X { get; }
        public float Y { get; }
        public int Health { get; }
        public int MaxHealth { get; }

        public UnitRenderState(int id, int factionId, UnitType type, UnitState state, float x, float y, int health, int maxHealth)
        {
            Id = id;
            FactionId = factionId;
            Type = type;
            State = state;
            X = x;
            Y = y;
            Health = health;
            MaxHealth = maxHealth;
        }
    }

    public readonly struct BuildingRenderState
    {
        public int Id { get; }
        public int FactionId { get; }
        public BuildingType Type { get; }
        public int X { get; }
        public int Y { get; }
        public int Health { get; }
        public int MaxHealth { get; }
        public float BuildProgress { get; }

        public BuildingRenderState(int id, int factionId, BuildingType type, int x, int y, int health, int maxHealth, float buildProgress = 1.0f)
        {
            Id = id;
            FactionId = factionId;
            Type = type;
            X = x;
            Y = y;
            Health = health;
            MaxHealth = maxHealth;
            BuildProgress = buildProgress;
        }
    }

    public readonly struct CellRenderState
    {
        public int X { get; }
        public int Y { get; }
        public CellType Type { get; }

        public CellRenderState(int x, int y, CellType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }

    public readonly struct ControlPointRenderState
    {
        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        public int ControllingFactionId { get; }

        public ControlPointRenderState(int id, int x, int y, int controllingFactionId)
        {
            Id = id;
            X = x;
            Y = y;
            ControllingFactionId = controllingFactionId;
        }
    }

    public readonly struct MapRenderState
    {
        public int Width { get; }
        public int Height { get; }
        public CellType[,] Grid { get; }
        public IReadOnlyList<ControlPointRenderState> ControlPoints { get; }

        public MapRenderState(int width, int height, CellType[,] grid, IReadOnlyList<ControlPointRenderState> controlPoints)
        {
            Width = width;
            Height = height;
            Grid = grid;
            ControlPoints = controlPoints;
        }
    }

    public readonly struct FactionResourcesRenderState
    {
        public int Wood { get; }
        public int Gold { get; }
        public int Stone { get; }
        public int Metal { get; }
        public int Food { get; }

        public FactionResourcesRenderState(int wood, int gold, int stone, int metal, int food)
        {
            Wood = wood;
            Gold = gold;
            Stone = stone;
            Metal = metal;
            Food = food;
        }
    }
}