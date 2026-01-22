

using Repair.Domain;

namespace Repair.Infrastructure.Persistence.Data
{
    public static class DataSeeder
    {
        public static void  Seed (ApplicationDbContext context)
        {
            if (context.Brands.Any())
                return;

            // ---------- Brands ----------
            var brands = new[]
            {
            Brand.Create("iPhone", "Is your iPhone screen, battery, back glass or battery broken? As an Independent Repair Provider for Apple, we can repair your iPhone with original parts, but also with cheaper alternatives of the highest quality!"),
            Brand.Create("Samsung", "Do you want to replace your Samsung screen, back glass, camera or battery? We offer Samsung repairs with original parts or cheaper options of the highest quality! We do this with a 30-minute service and you are assured of your data!"  ),
            Brand.Create("Google", "Do you want to replace your Google pixel screen, back glass, camera or battery? We offer Samsung repairs with original parts or cheaper options of the highest quality! We do this with a 30-minute service and you are assured of your data!"),
            Brand.Create("Huawei", "Do you want to replace your Huawei screen, back glass, camera or battery? We offer repairs with original parts or cheaper options of the highest quality! We do this with a 30-minute service and you are assured of your data!"),
            Brand.Create("Xiaomi", "Do you want to replace your Xiaomi screen, back glass, camera or battery? We offer repairs with original parts or cheaper options of the highest quality! We do this with a 30-minute service and you are assured of your data!"),
            Brand.Create("OnePlus", "Is your OnePlus screen, battery, back glass or battery broken? As an Independent Repair Provider, we can repair your Phone with original parts, but also with cheaper alternatives of the highest quality!"),
            Brand.Create("Oppo", "Is your Oppo screen, battery, back glass or battery broken? As an Independent Repair Provider for Apple, we can repair your Oppo with original parts, but also with cheaper alternatives of the highest quality!")
        };

            context.Brands.AddRange(brands);

            // ---------- Phone Models ----------
            var models = new List<PhoneModel>
        {
            // Apple
            PhoneModel.Create("iPhone 17", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898511/svgs/iPhone-17-series-180x300_zd0kdy.webp", brands[0].Id),
            PhoneModel.Create("iPhone 14", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898511/svgs/iPhone-17-series-180x300_zd0kdy.webp", brands[0].Id),

            // Samsung
            PhoneModel.Create("Galaxy S24", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898510/svgs/Samsung-S24-Ultra-193x300_lzwiao.webp", brands[1].Id),
            PhoneModel.Create("Galaxy A54", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898510/svgs/Samsung-S24-Ultra-193x300_lzwiao.webp", brands[1].Id),

            // Google
            PhoneModel.Create("Pixel 8", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898510/svgs/googlepixel_gn0rgn.webp", brands[2].Id),

            // Huawei
            PhoneModel.Create("P60 Pro", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898510/svgs/hauwei_pcyc9d.jpg", brands[3].Id),

            // Xiaomi
            PhoneModel.Create("Xiaomi 13 Pro", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898510/svgs/redmi_ytdeib.webp", brands[4].Id),

            // OnePlus
            PhoneModel.Create("OnePlus 12", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898511/svgs/OnePlus-193x300_re2nx3.png", brands[5].Id),

            // Oppo
            PhoneModel.Create("Oppo Find X6", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768898511/svgs/oppo_zjtmzr.webp", brands[6].Id),

          
        };

            context.PhoneModels.AddRange(models);

            // ---------- Phone Parts ----------
            var parts = new[]
            {
            PhonePart.Create("Screen", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768845444/svgs/xapprenpwxkhakdwa0uz.svg"),
            PhonePart.Create("Battery", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768845446/svgs/qo7ngsyvk94ok22iaodc.svg"),
            PhonePart.Create("Charging Port", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768845454/svgs/f6mgxnxzowbs4mynnagw.svg"),
            PhonePart.Create("Camera", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768845449/svgs/zfiagyg2rydhkhfvx35u.svg"),
            PhonePart.Create("Speaker", "https://res.cloudinary.com/dtalnpcgh/image/upload/v1768845452/svgs/gakmeeol5mrfxayujs1q.svg")
        };

            context.PhoneParts.AddRange(parts);

            // ---------- Repair Prices ----------
            var prices = new List<RepairPrice>();

            foreach (var model in models)
            {
                prices.AddRange(new[]
                {
                RepairPrice.Create(model.Id, parts[0].Id, 180m, 30), // Screen
                RepairPrice.Create(model.Id, parts[1].Id, 90m, 10),  // Battery
                RepairPrice.Create(model.Id, parts[2].Id, 70m, 30),  // Charging Port
                RepairPrice.Create(model.Id, parts[3].Id, 120m, 40), // Camera
                RepairPrice.Create(model.Id, parts[4].Id, 60m, 20)   // Speaker
            });
            }

            context.RepairPrices.AddRange(prices);

             context.SaveChanges();
        }
    }

}
