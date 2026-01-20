

namespace Repair.Domain
{
    public sealed class RepairPrice : EntityBase
    {
        private RepairPrice()
        {
            
        }
        public Guid PhoneModelId { get; set; }
        public PhoneModel PhoneModel { get; set; }
        public Guid PhonePartId { get; set; }
        public PhonePart PhonePart { get; set; }
        public decimal Cost { get; set; }
        public int Duration { get; set; }

        public static RepairPrice Create(Guid phoneModelId, Guid phonePartId, decimal cost, int duration) => new() { PhonePartId = phonePartId, PhoneModelId = phoneModelId, Cost = cost, Duration = duration};
    }
}
