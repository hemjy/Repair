

namespace Repair.Application.DTOs.PhoneParts
{
    public class GetPricesDto
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public Guid PhoneModelId { get; set; }
        public string PhoneModeName { get; set; }
        public Guid PhonePartId { get; set; }
        public string PhonePartName { get; set; }
        public decimal Cost { get; set; }
        public string DUration { get; set; }

    }
}
