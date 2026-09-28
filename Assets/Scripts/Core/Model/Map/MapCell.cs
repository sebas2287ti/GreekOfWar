namespace Core.Model.Map
{
    using Core.Model.Enums;

    public class MapCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellType Type { get; set; }

        public bool IsWalkable => Type == CellType.Land;

        public MapCell(int x, int y, CellType type = CellType.Land)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }
}