namespace Core.Model.Map
{
    public class ControlPointModel
    {
        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        
        // Propiedades de estado
        public int ControllingFactionId { get; private set; } = -1; // -1 = Neutral
        public float ResourceMultiplier { get; set; } = 1.5f;
        
        // Control de captura gradual
        public float CaptureProgress { get; private set; } = 0f; // 0.0 a 1.0 (o 0 a 100%)
        public int CapturingFactionId { get; private set; } = -1;

        public ControlPointModel(int id, int x, int y, float resourceMultiplier = 1.5f)
        {
            Id = id;
            X = x;
            Y = y;
            ResourceMultiplier = resourceMultiplier;
        }

        /// <summary>
        /// Cambia la facción dominante de forma explícita.
        /// </summary>
        public void SetOwner(int factionId)
        {
            ControllingFactionId = factionId;
        }

        /// <summary>
        /// Actualiza el progreso de captura de la celda.
        /// </summary>
        public void UpdateCapture(int factionId, float amount)
        {
            if (ControllingFactionId == factionId) return;

            CapturingFactionId = factionId;
            CaptureProgress += amount;

            if (CaptureProgress >= 1.0f)
            {
                ControllingFactionId = factionId;
                CaptureProgress = 1.0f;
            }
        }
    }
}