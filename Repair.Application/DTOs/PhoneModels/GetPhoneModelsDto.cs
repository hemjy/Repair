

namespace Repair.Application.DTOs.PhoneParts
{
    public class GetPhoneModelsDto
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string? Name { get; set; }
        public string? BrandName { get; set; }
        public string? ImageUrl { get; set; }

    }
}
