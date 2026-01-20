namespace Repair.Domain
{
    public sealed class PhoneModel : EntityBase
    {
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public required string  Name { get; set; }
        public required string ImageUrl { get; set; }

        public static PhoneModel Create(string name, string imageUrl, Guid brandId) => new() { ImageUrl = imageUrl, Name = name, BrandId = brandId };
    }
}
