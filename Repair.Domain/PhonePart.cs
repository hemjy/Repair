namespace Repair.Domain
{
    public class PhonePart : EntityBase
    {
        public required string Name { get; set; }
        public required string Image { get; set; }

        public static PhonePart Create(string name, string image) => new() { Image = image, Name = name };
    }
}
