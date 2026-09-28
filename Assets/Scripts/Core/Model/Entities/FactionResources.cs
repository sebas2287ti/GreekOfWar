// Core/Model/Entities/FactionResources.cs
namespace Core.Model.Entities
{
    public class FactionResources
    {
        public int FactionId { get; set; }
        public float Gold { get; set; }

        public FactionResources(int factionId, float initialGold = 100f)
        {
            FactionId = factionId;
            Gold = initialGold;
        }

        public bool Spend(float amount)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
                return true;
            }
            return false;
        }

        public void Add(float amount)
        {
            Gold += amount;
        }
    }
}