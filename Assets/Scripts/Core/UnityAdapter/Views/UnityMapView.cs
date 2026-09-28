using UnityEngine;
using Core.Model.Map;
using Core.Model.Enums;

namespace UnityAdapter.Views
{
    public class UnityMapView : MonoBehaviour
    {
        [Header("Sprites de Terreno (PNGs)")]
        [SerializeField] private Sprite landSprite;
        [SerializeField] private Sprite waterSprite;

        [Header("Configuración de Celdas")]
        [SerializeField] private float cellSize = 1f;

        private GameObject[,] _cellViews;

        public void RenderMap(GridMap map)
        {
            ClearCurrentViews();

            _cellViews = new GameObject[map.Width, map.Height];

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MapCell cell = map.GetCell(x, y);
                    if (cell != null)
                    {
                        CreateCellGameObject(cell);
                    }
                }
            }
        }

        public void UpdateCellView(MapCell cell)
        {
            if (cell.X >= _cellViews.GetLength(0) || cell.Y >= _cellViews.GetLength(1))
                return;

            GameObject cellObj = _cellViews[cell.X, cell.Y];
            if (cellObj != null)
            {
                SpriteRenderer renderer = cellObj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = GetSpriteForType(cell.Type);
                }
            }
        }

        private void CreateCellGameObject(MapCell cell)
        {
            GameObject cellObj = new GameObject($"Cell_{cell.X}_{cell.Y}");
            cellObj.transform.SetParent(transform);
            
            // Posicionamiento en plano 2D Top-Down (X e Y)
            cellObj.transform.position = new Vector3(cell.X * cellSize, cell.Y * cellSize, 0f);
            
            // Sin rotaciones 3D en 2D puro
            cellObj.transform.rotation = Quaternion.identity;

            SpriteRenderer renderer = cellObj.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSpriteForType(cell.Type);
            renderer.sortingOrder = 0; // Capa base del suelo

            _cellViews[cell.X, cell.Y] = cellObj;
        }

        private Sprite GetSpriteForType(CellType type)
        {
            return type switch
            {
                CellType.Land => landSprite,
                CellType.Water => waterSprite,
                _ => landSprite
            };
        }

        private void ClearCurrentViews()
        {
            if (_cellViews == null) return;

            foreach (var view in _cellViews)
            {
                if (view != null) Destroy(view);
            }
        }
    }
}