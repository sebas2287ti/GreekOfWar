namespace Core.Model.Map
{
    using Core.Model.Enums;

    public class GridMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private MapCell[,] _cells;

        public GridMap(int initialWidth = 32, int initialHeight = 32)
        {
            Width = initialWidth;
            Height = initialHeight;
            _cells = new MapCell[Width, Height];

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _cells[x, y] = new MapCell(x, y, CellType.Land);
                }
            }
        }

        public MapCell GetCell(int x, int y)
        {
            if (!IsValidPosition(x, y)) return null;
            return _cells[x, y];
        }

        public bool SetCellType(int x, int y, CellType type)
        {
            if (!IsValidPosition(x, y)) return false;
            _cells[x, y].Type = type;
            return true;
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Expande el tamaño del mapa preservando las celdas existentes.
        /// </summary>
        public void ExpandMap(int newWidth, int newHeight)
        {
            if (newWidth <= Width && newHeight <= Height) return;

            MapCell[,] newCells = new MapCell[newWidth, newHeight];

            // Copiar celdas existentes
            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    if (x < Width && y < Height)
                    {
                        newCells[x, y] = _cells[x, y];
                    }
                    else
                    {
                        // Nuevas celdas creadas por defecto como Land
                        newCells[x, y] = new MapCell(x, y, CellType.Land);
                    }
                }
            }

            _cells = newCells;
            Width = newWidth;
            Height = newHeight;
        }
    }
}