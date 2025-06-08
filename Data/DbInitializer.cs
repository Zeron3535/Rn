using TinaKuafor.Models;
using Microsoft.EntityFrameworkCore;

namespace TinaKuafor.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // For PostgreSQL, we use migrations instead of EnsureCreated
            // context.Database.EnsureCreated() is replaced by context.Database.Migrate() in Program.cs

            // Check if we already have data
            if (context.ServiceCategories.Any())
            {
                return; // DB has been seeded
            }

            // Seed business hours
            var businessHours = new BusinessHours[]
            {
                new BusinessHours { DayOfWeek = "Monday", IsOpen = false, OpenTime = "", CloseTime = "" },
                new BusinessHours { DayOfWeek = "Tuesday", IsOpen = true, OpenTime = "10:00", CloseTime = "21:00" },
                new BusinessHours { DayOfWeek = "Wednesday", IsOpen = true, OpenTime = "10:00", CloseTime = "21:00" },
                new BusinessHours { DayOfWeek = "Thursday", IsOpen = true, OpenTime = "10:00", CloseTime = "21:00" },
                new BusinessHours { DayOfWeek = "Friday", IsOpen = true, OpenTime = "10:00", CloseTime = "21:00" },
                new BusinessHours { DayOfWeek = "Saturday", IsOpen = true, OpenTime = "10:00", CloseTime = "21:00" },
                new BusinessHours { DayOfWeek = "Sunday", IsOpen = true, OpenTime = "10:00", CloseTime = "21:00" }
            };

            context.BusinessHours.AddRange(businessHours);
            context.SaveChanges();

            // Seed service categories
            var categories = new ServiceCategory[]
            {
                new ServiceCategory { 
                    Name = "Saç Hizmetleri", 
                    UrlSlug = "izmir-balcova-sac-hizmetleri", 
                    Description = "Profesyonel saç kesimi, boyama ve bakım hizmetleri", 
                    SeoDescription = "İzmir Balçova'da profesyonel saç kesimi, boyama ve bakım hizmetleri sunan Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Saç Kesimi, İzmir Balçova Saç Boyama, İzmir Kadın Kuaför", 
                    DisplayOrder = 1 
                },
                new ServiceCategory { 
                    Name = "Manikür & Pedikür", 
                    UrlSlug = "izmir-balcova-manikur-pedikur", 
                    Description = "Profesyonel tırnak bakımı ve güzellik hizmetleri", 
                    SeoDescription = "İzmir Balçova'da profesyonel manikür, pedikür ve kalıcı oje hizmetleri sunan Tina Kuaför'de tırnaklarınızı güzelleştirin.", 
                    SeoKeywords = "İzmir Manikür, İzmir Pedikür, İzmir Balçova Kalıcı Oje", 
                    DisplayOrder = 2 
                },
                new ServiceCategory { 
                    Name = "Cilt Bakımı", 
                    UrlSlug = "izmir-balcova-cilt-bakimi", 
                    Description = "Profesyonel cilt bakımı ve güzellik hizmetleri", 
                    SeoDescription = "İzmir Balçova'da profesyonel cilt bakımı hizmetleri sunan Tina Kuaför'de cildinizi yenileyin.", 
                    SeoKeywords = "İzmir Cilt Bakımı, İzmir Balçova Cilt Bakımı, İzmir Kadın Güzellik Salonu", 
                    DisplayOrder = 3 
                },
                new ServiceCategory { 
                    Name = "Ağda Hizmetleri", 
                    UrlSlug = "izmir-balcova-agda-hizmetleri", 
                    Description = "Profesyonel sir ağda ve epilasyon hizmetleri", 
                    SeoDescription = "İzmir Balçova'da profesyonel sir ağda hizmetleri sunan Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Ağda, İzmir Balçova Sir Ağda, İzmir Kadın Güzellik Salonu", 
                    DisplayOrder = 4 
                }
            };

            context.ServiceCategories.AddRange(categories);
            context.SaveChanges();

            // Seed services
            var services = new Service[]
            {
                // Saç Hizmetleri
                new Service { 
                    Name = "İzmir Balçova Saç Kesimi", 
                    UrlSlug = "izmir-balcova-sac-kesimi", 
                    Price = 250, 
                    DurationMinutes = 45, 
                    Description = "Profesyonel saç kesimi hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel saç kesimi hizmeti. Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Saç Kesimi, İzmir Balçova Saç Kesimi, İzmir Kadın Kuaför", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Dip Boyası", 
                    UrlSlug = "izmir-balcova-dip-boyasi", 
                    Price = 500, 
                    DurationMinutes = 90, 
                    Description = "Profesyonel dip boya hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel dip boya hizmeti. Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Dip Boya, İzmir Balçova Dip Boya, İzmir Kadın Kuaför", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Saç Boyası", 
                    UrlSlug = "izmir-balcova-sac-boyasi", 
                    Price = 700, 
                    DurationMinutes = 120, 
                    Description = "Profesyonel saç boyama hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel saç boyama hizmeti. Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Saç Boyama, İzmir Balçova Saç Boyama, İzmir Kadın Kuaför", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Keratin", 
                    UrlSlug = "izmir-balcova-keratin", 
                    Price = 1000, 
                    DurationMinutes = 180, 
                    Description = "Profesyonel keratin bakımı", 
                    SeoDescription = "İzmir Balçova'da profesyonel keratin bakımı. Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Keratin, İzmir Balçova Keratin, İzmir Kadın Kuaför", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Fön", 
                    UrlSlug = "izmir-balcova-fon", 
                    Price = 200, 
                    DurationMinutes = 30, 
                    Description = "Profesyonel fön hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel fön hizmeti. Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Fön, İzmir Balçova Fön, İzmir Kadın Kuaför", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Maşa", 
                    UrlSlug = "izmir-balcova-masa", 
                    Price = 300, 
                    DurationMinutes = 45, 
                    Description = "Profesyonel maşa hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel maşa hizmeti. Tina Kuaför'de saçlarınızı güvenle emanet edebilirsiniz.", 
                    SeoKeywords = "İzmir Maşa, İzmir Balçova Maşa, İzmir Kadın Kuaför", 
                    CategoryId = categories[0].Id 
                },

                // Manikür & Pedikür
                new Service { 
                    Name = "İzmir Balçova Manikür", 
                    UrlSlug = "izmir-balcova-manikur", 
                    Price = 250, 
                    DurationMinutes = 45, 
                    Description = "Profesyonel manikür hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel manikür hizmeti. Tina Kuaför'de tırnaklarınızı güzelleştirin.", 
                    SeoKeywords = "İzmir Manikür, İzmir Balçova Manikür, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[1].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Pedikür", 
                    UrlSlug = "izmir-balcova-pedikur", 
                    Price = 300, 
                    DurationMinutes = 60, 
                    Description = "Profesyonel pedikür hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel pedikür hizmeti. Tina Kuaför'de ayaklarınızı güzelleştirin.", 
                    SeoKeywords = "İzmir Pedikür, İzmir Balçova Pedikür, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[1].Id 
                },

                // Cilt Bakımı
                new Service { 
                    Name = "İzmir Balçova Cilt Bakımı", 
                    UrlSlug = "izmir-balcova-cilt-bakimi", 
                    Price = 600, 
                    DurationMinutes = 90, 
                    Description = "Profesyonel cilt bakımı hizmeti (Her şey dahil)", 
                    SeoDescription = "İzmir Balçova'da profesyonel cilt bakımı hizmeti. Tina Kuaför'de cildinizi yenileyin.", 
                    SeoKeywords = "İzmir Cilt Bakımı, İzmir Balçova Cilt Bakımı, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[2].Id 
                },

                // Ağda Hizmetleri
                new Service { 
                    Name = "İzmir Balçova Komple Sir", 
                    UrlSlug = "izmir-balcova-komple-sir", 
                    Price = 600, 
                    DurationMinutes = 90, 
                    Description = "Profesyonel komple sir ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel komple sir ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Komple Ağda, İzmir Balçova Sir Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Dudak Üstü", 
                    UrlSlug = "izmir-balcova-dudak-ustu", 
                    Price = 50, 
                    DurationMinutes = 15, 
                    Description = "Profesyonel dudak üstü ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel dudak üstü ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Dudak Üstü Ağda, İzmir Balçova Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Kaş", 
                    UrlSlug = "izmir-balcova-kas", 
                    Price = 100, 
                    DurationMinutes = 20, 
                    Description = "Profesyonel kaş ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel kaş ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Kaş Ağda, İzmir Balçova Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Kol Sir", 
                    UrlSlug = "izmir-balcova-kol-sir", 
                    Price = 200, 
                    DurationMinutes = 30, 
                    Description = "Profesyonel kol sir ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel kol sir ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Kol Ağda, İzmir Balçova Sir Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Komple Yüz Alımı", 
                    UrlSlug = "izmir-balcova-komple-yuz-alimi", 
                    Price = 300, 
                    DurationMinutes = 45, 
                    Description = "Profesyonel komple yüz ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel komple yüz ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Yüz Ağda, İzmir Balçova Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Özel Bölge", 
                    UrlSlug = "izmir-balcova-ozel-bolge", 
                    Price = 300, 
                    DurationMinutes = 30, 
                    Description = "Profesyonel özel bölge ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel özel bölge ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Özel Bölge Ağda, İzmir Balçova Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "İzmir Balçova Bacak Sir", 
                    UrlSlug = "izmir-balcova-bacak-sir", 
                    Price = 200, 
                    DurationMinutes = 30, 
                    Description = "Profesyonel bacak sir ağda hizmeti", 
                    SeoDescription = "İzmir Balçova'da profesyonel bacak sir ağda hizmeti. Tina Kuaför'de istenmeyen tüylerden kurtulun.", 
                    SeoKeywords = "İzmir Bacak Ağda, İzmir Balçova Sir Ağda, İzmir Kadın Güzellik Salonu", 
                    CategoryId = categories[3].Id 
                }
            };

            context.Services.AddRange(services);
            context.SaveChanges();

            // Seed testimonials
            var testimonials = new Testimonial[]
            {
                new Testimonial { 
                    CustomerName = "Sezen Dayanç", 
                    Content = "Çok memnun kaldım, güler yüzlü sahibi, temiz ve özenli.", 
                    Response = "Teşekkür ederim 💞", 
                    IsApproved = true, 
                    DisplayOrder = 1 
                },
                new Testimonial { 
                    CustomerName = "Ferda Zengl", 
                    Content = "Güler yüzlü ve yaptığı işi itinayla yaptığı için çok memnunum. Ailemdeki tüm bayanları getirdim. Hepsi çok memnun çünkü baştan savma yapmıyor işini. Tavsiye ederim. Fatmacım çok teşekkür ediyoruz.", 
                    Response = "Ben teşekkür ederim 💞", 
                    IsApproved = true, 
                    DisplayOrder = 2 
                },
                new Testimonial { 
                    CustomerName = "Hypatia 415", 
                    Content = "Saçımı kestirmek için gittiğimde keşfettim. Gayet memnun kaldım, artık her zaman tercihim burası. Çok naif, işini özenle yapan bir kuaför.", 
                    Response = "Teşekkür ederim ❤️", 
                    IsApproved = true, 
                    DisplayOrder = 3 
                },
                new Testimonial { 
                    CustomerName = "Hatice Erbil", 
                    Content = "Çok ilgililerdi, yaptırdığım işlemlerden memnun kaldım. Tekrar tekrar gideceğim bir yer.", 
                    Response = "Teşekkür ederim ❤️", 
                    IsApproved = true, 
                    DisplayOrder = 4 
                },
                new Testimonial { 
                    CustomerName = "Büşra Öngül", 
                    Content = "Her zaman tırnaklarımı burada yaptırıyorum. Temiz ve işini düzgün yapan bir yer, sahibi de çok ilgili.", 
                    Response = "Teşekkür ederim ❤️", 
                    IsApproved = true, 
                    DisplayOrder = 5 
                },
                new Testimonial { 
                    CustomerName = "Rojin Beyter", 
                    Content = "Her hizmette kusursuz olmakla birlikte sıcak kanlı olmalarıyla ön plana çıkıyorlar.", 
                    Response = "Teşekkür ederim ❤️", 
                    IsApproved = true, 
                    DisplayOrder = 6 
                },
                new Testimonial { 
                    CustomerName = "Sibel 35", 
                    Content = "Çok memnun kaldım. Çok güler yüzlü ve işinin ehli.", 
                    Response = "Teşekkür ederim ❤️", 
                    IsApproved = true, 
                    DisplayOrder = 7 
                },
                new Testimonial { 
                    CustomerName = "Neslihan Özyavuz", 
                    Content = "Benim için Balçova'nın en gözde kuaför salonu. İlgisi ve işi çok iyi, fiyatları da diğer salonlara göre oldukça uygun. Balçova'ya yolunuz düşerse uğrayın derim. :)", 
                    Response = "Teşekkür ederim ❤️", 
                    IsApproved = true, 
                    DisplayOrder = 8 
                },
                new Testimonial { 
                    CustomerName = "Nur Akçay", 
                    Content = "Uzun zamandır işini düzgün yapan bir kuaför arıyordum, burası tam aradığım bir yer. Güler yüzlü, temiz ve işini düzgün yapan bir işletme, teşekkür ederim.", 
                    Response = "Güzel düşünceniz ve bizi tercih ettiğiniz için teşekkür ederim ☺️", 
                    IsApproved = true, 
                    DisplayOrder = 9 
                },
                new Testimonial { 
                    CustomerName = "Berfin Demirel", 
                    Content = "Şiddetle tavsiye ediyorum. Fatma Hanım'ın ellerine sağlık, çok tatlı biri, çok da ilgili… ❤️", 
                    Response = "Teşekkür ederim ☺️🌹", 
                    IsApproved = true, 
                    DisplayOrder = 10 
                },
                new Testimonial { 
                    CustomerName = "Dilek Altuner", 
                    Content = "Çok memnun kaldım, çok güler yüzlü, temiz ve özenli.", 
                    Response = "Teşekkür ederim Dilek Hanım 🌹☺️ Hizmetimizden memnun kalmanız beni çok mutlu etti 😍😍😍", 
                    IsApproved = true, 
                    DisplayOrder = 11 
                },
                new Testimonial { 
                    CustomerName = "Beyza Aytan", 
                    Content = "Çok ilgili, güzel, temiz ve mutlu ayrılabileceğiniz bir yer. Tavsiye ederim.", 
                    Response = "Değerli yorumunuz için çok teşekkür ederim, sizi tanımak güzeldi 🤗", 
                    IsApproved = true, 
                    DisplayOrder = 12 
                },
                new Testimonial { 
                    CustomerName = "İlayda Emir", 
                    Content = "Çok güler yüzlü karşıladılar ve ilgilendiler, teşekkür ederim. Memnun kaldım.", 
                    Response = "Teşekkür ederim, her zaman bekleriz 🥰🤗", 
                    IsApproved = true, 
                    DisplayOrder = 13 
                },
                new Testimonial { 
                    CustomerName = "GizemM", 
                    Content = "Çok ilgilisiniz, çok da başarılı. Teşekkürler 💕💕", 
                    Response = "Teşekkürler güzel düşünceniz için 🥰🤗", 
                    IsApproved = true, 
                    DisplayOrder = 14 
                }
            };

            context.Testimonials.AddRange(testimonials);
            context.SaveChanges();
        }


    }
}