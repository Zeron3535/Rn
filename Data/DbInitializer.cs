using TinaKuafor.Models;
using Microsoft.EntityFrameworkCore;

namespace TinaKuafor.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Veritabanı zaten oluşturulmuş ve doluysa işlem yapma
            if (context.ServiceCategories.Any())
            {
                return; // DB has been seeded
            }

            // Çalışma Saatleri
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

            // === HİZMET KATEGORİLERİ (Modernleştirildi) ===
            var categories = new ServiceCategory[]
            {
                new ServiceCategory { 
                    Name = "Saç Tasarım ve Bakım", // Değiştirildi
                    UrlSlug = "sac-tasarim-ve-bakim", // Değiştirildi
                    Description = "Profesyonel saç kesimi, renklendirme ve bakım uygulamaları.", // Değiştirildi
                    SeoDescription = "Salonumuzda modern saç kesimi, profesyonel renklendirme ve yenileyici bakım hizmetleri ile saçlarınıza yeni bir hayat verin.", // Değiştirildi
                    SeoKeywords = "Saç Kesimi, Saç Boyama, Keratin Bakım, Fön, Saç Tasarım", // Değiştirildi
                    DisplayOrder = 1 
                },
                new ServiceCategory { 
                    Name = "El ve Ayak Bakımı", // Değiştirildi
                    UrlSlug = "el-ve-ayak-bakimi", // Değiştirildi
                    Description = "Profesyonel manikür, pedikür ve kalıcı oje hizmetleri.", // Değiştirildi
                    SeoDescription = "Manikür, pedikür ve kalıcı oje uygulamaları ile el ve ayaklarınıza hak ettiği özeni gösterin.", // Değiştirildi
                    SeoKeywords = "Manikür, Pedikür, Kalıcı Oje, Tırnak Bakımı", // Değiştirildi
                    DisplayOrder = 2 
                },
                new ServiceCategory { 
                    Name = "Cilt Bakımı", 
                    UrlSlug = "cilt-bakimi", // Değiştirildi
                    Description = "Cildinizi canlandıran ve yenileyen profesyonel bakım uygulamaları.", // Değiştirildi
                    SeoDescription = "Cildinizin ihtiyacına özel olarak hazırlanan profesyonel bakım seansları ile daha taze ve parlak bir görünüme kavuşun.", // Değiştirildi
                    SeoKeywords = "Cilt Bakımı, Yüz Bakımı, Güzellik Salonu", 
                    DisplayOrder = 3 
                },
                new ServiceCategory { 
                    Name = "Ağda ve Epilasyon", // Değiştirildi
                    UrlSlug = "agda-ve-epilasyon", // Değiştirildi
                    Description = "Pürüzsüz bir cilt için profesyonel sir ağda ve epilasyon hizmetleri.", // Değiştirildi
                    SeoDescription = "İstenmeyen tüylerden kurtulmak için hijyenik ve profesyonel ağda ve epilasyon hizmetlerimizden yararlanın.", // Değiştirildi
                    SeoKeywords = "Ağda, Sir Ağda, Epilasyon, Yüz Ağdası, Bacak Ağdası", // Değiştirildi
                    DisplayOrder = 4 
                }
            };

            context.ServiceCategories.AddRange(categories);
            context.SaveChanges();

            // === HİZMETLER (Modernleştirildi ve yer bildiren ifadeler kaldırıldı) ===
            var services = new Service[]
            {
                // Saç Tasarım ve Bakım
                new Service { 
                    Name = "Saç Kesimi ve Stil Danışmanlığı", // Değiştirildi
                    UrlSlug = "sac-kesimi-ve-stil-danismanligi", // Değiştirildi
                    Price = 250, 
                    DurationMinutes = 45, 
                    Description = "Yüz şeklinize ve tarzınıza en uygun modern saç kesimi.", // Değiştirildi
                    SeoDescription = "Profesyonel ekibimizle yüz hatlarınıza uygun, modern ve stil sahibi saç kesimi hizmeti.", // Değiştirildi
                    SeoKeywords = "Saç Kesimi, Modern Kesim, Stil Danışmanlığı", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "Dip Boya Uygulaması", // Değiştirildi
                    UrlSlug = "dip-boya-uygulamasi", // Değiştirildi
                    Price = 500, 
                    DurationMinutes = 90, 
                    Description = "Saç diplerinizdeki renk farkını kapatan profesyonel uygulama.", // Değiştirildi
                    SeoDescription = "Saçlarınızın doğal görünümünü koruyarak dip boya uygulaması ile renginizi yenileyin.", // Değiştirildi
                    SeoKeywords = "Dip Boya, Saç Boyama, Renk Yenileme", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "Profesyonel Saç Boyası", // Değiştirildi
                    UrlSlug = "profesyonel-sac-boyasi", // Değiştirildi
                    Price = 700, 
                    DurationMinutes = 120, 
                    Description = "Saçınıza parlaklık ve canlılık katan komple boya hizmeti.", // Değiştirildi
                    SeoDescription = "Kaliteli ürünlerle saçınıza zarar vermeden profesyonel saç boyama hizmeti.", // Değiştirildi
                    SeoKeywords = "Saç Boyası, Komple Boya, Renk Değişimi", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "Keratin Bakımı ve Brezilya Fönü", // Değiştirildi
                    UrlSlug = "keratin-bakimi-brezilya-fonu", // Değiştirildi
                    Price = 1000, 
                    DurationMinutes = 180, 
                    Description = "Yıpranmış saçları onaran ve pürüzsüzleştiren yoğun bakım.", // Değiştirildi
                    SeoDescription = "Keratin bakımı ile saçlarınızı onarın, Brezilya fönü ile uzun süreli düzlüğün keyfini çıkarın.", // Değiştirildi
                    SeoKeywords = "Keratin, Keratin Bakım, Brezilya Fönü", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "Profesyonel Fön", // Değiştirildi
                    UrlSlug = "profesyonel-fon", // Değiştirildi
                    Price = 200, 
                    DurationMinutes = 30, 
                    Description = "Günlük veya özel günler için kalıcı ve hacimli fön.", // Değiştirildi
                    SeoDescription = "Saçınıza hacim ve şekil kazandıran profesyonel fön hizmeti.", // Değiştirildi
                    SeoKeywords = "Fön, Saç Şekillendirme", 
                    CategoryId = categories[0].Id 
                },
                new Service { 
                    Name = "Dalgalı & Bukleli Şekillendirme", // Değiştirildi
                    UrlSlug = "dalgali-bukleli-sekillendirme", // Değiştirildi
                    Price = 300, 
                    DurationMinutes = 45, 
                    Description = "Maşa ile doğal dalgalar veya belirgin bukleler.", // Değiştirildi
                    SeoDescription = "Maşa kullanarak saçlarınıza doğal dalgalar veya göz alıcı bukleler kazandırıyoruz.", // Değiştirildi
                    SeoKeywords = "Maşa, Dalgalı Saç, Bukle", 
                    CategoryId = categories[0].Id 
                },

                // El ve Ayak Bakımı
                new Service { 
                    Name = "Klasik Manikür", // Değiştirildi
                    UrlSlug = "klasik-manikur", // Değiştirildi
                    Price = 250, 
                    DurationMinutes = 45, 
                    Description = "Tırnaklarınıza estetik bir görünüm kazandıran temel manikür.", // Değiştirildi
                    SeoDescription = "El ve tırnak sağlığınız için profesyonel manikür hizmeti.", // Değiştirildi
                    SeoKeywords = "Manikür, El Bakımı, Tırnak", 
                    CategoryId = categories[1].Id 
                },
                new Service { 
                    Name = "Klasik Pedikür", // Değiştirildi
                    UrlSlug = "klasik-pedikur", // Değiştirildi
                    Price = 300, 
                    DurationMinutes = 60, 
                    Description = "Ayaklarınıza sağlık ve güzellik katan temel pedikür.", // Değiştirildi
                    SeoDescription = "Ayak sağlığınız ve estetiği için profesyonel pedikür hizmeti.", // Değiştirildi
                    SeoKeywords = "Pedikür, Ayak Bakımı", 
                    CategoryId = categories[1].Id 
                },

                // Cilt Bakımı
                new Service { 
                    Name = "Derinlemesine Cilt Bakımı", // Değiştirildi
                    UrlSlug = "derinlemesine-cilt-bakimi", // Değiştirildi
                    Price = 600, 
                    DurationMinutes = 90, 
                    Description = "Cildinizi temizleyen, nemlendiren ve yenileyen komple bakım.", // Değiştirildi
                    SeoDescription = "Cildinizin ihtiyaçlarına yönelik derinlemesine temizlik ve bakım hizmeti.", // Değiştirildi
                    SeoKeywords = "Cilt Bakımı, Yüz Temizleme", 
                    CategoryId = categories[2].Id 
                },

                // Ağda ve Epilasyon
                new Service { 
                    Name = "Tüm Vücut Ağda", // Değiştirildi
                    UrlSlug = "tum-vucut-agda", // Değiştirildi
                    Price = 600, 
                    DurationMinutes = 90, 
                    Description = "Komple sir ağda ile uzun süren pürüzsüzlük.", // Değiştirildi
                    SeoDescription = "Tüm vücut için profesyonel ve hijyenik sir ağda hizmeti.", // Değiştirildi
                    SeoKeywords = "Tüm Vücut Ağda, Komple Ağda, Sir Ağda", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "Dudak Üstü Ağda", // Değiştirildi
                    UrlSlug = "dudak-ustu-agda", // Değiştirildi
                    Price = 50, 
                    DurationMinutes = 15, 
                    Description = "Hassas bölge için özel sir ağda uygulaması.",
                    SeoDescription = "Dudak üstü bölgesi için hassas ve profesyonel ağda hizmeti.", // Değiştirildi
                    SeoKeywords = "Dudak Üstü, Bıyık Ağdası", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "Kaş Tasarımı (Ağda ile)", // Değiştirildi
                    UrlSlug = "kas-tasarimi-agda-ile", // Değiştirildi
                    Price = 100, 
                    DurationMinutes = 20, 
                    Description = "Bakışlarınıza anlam katan profesyonel kaş şekillendirme.",
                    SeoDescription = "Yüzünüze en uygun kaş şeklini ağda tekniği ile belirliyoruz.",
                    SeoKeywords = "Kaş Alma, Kaş Tasarımı, Kaş Ağdası", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "Kol Ağda", // Değiştirildi
                    UrlSlug = "kol-agda", // Değiştirildi
                    Price = 200, 
                    DurationMinutes = 30, 
                    Description = "Kollar için komple sir ağda uygulaması.",
                    SeoDescription = "Kollar için pürüzsüz bir görünüm sağlayan profesyonel sir ağda.",
                    SeoKeywords = "Kol Ağdası, Sir Ağda", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "Tüm Yüz Ağda", // Değiştirildi
                    UrlSlug = "tum-yuz-agda", // Değiştirildi
                    Price = 300, 
                    DurationMinutes = 45, 
                    Description = "Yüz bölgesindeki istenmeyen tüyler için komple ağda.",
                    SeoDescription = "Tüm yüz bölgesi için hassas ve profesyonel ağda hizmeti.",
                    SeoKeywords = "Yüz Ağdası, Komple Yüz", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "Genital Bölge Ağda", // Değiştirildi
                    UrlSlug = "genital-bolge-agda", // Değiştirildi
                    Price = 300, 
                    DurationMinutes = 30, 
                    Description = "Hassas bölge için hijyenik ve profesyonel ağda.",
                    SeoDescription = "Genital (özel) bölge için hijyenik ve profesyonel ağda hizmeti.",
                    SeoKeywords = "Özel Bölge Ağda, Genital Ağda", 
                    CategoryId = categories[3].Id 
                },
                new Service { 
                    Name = "Bacak Ağda", // Değiştirildi
                    UrlSlug = "bacak-agda", // Değiştirildi
                    Price = 200, 
                    DurationMinutes = 30, 
                    Description = "Bacaklar için komple sir ağda uygulaması.",
                    SeoDescription = "Bacaklar için uzun süreli pürüzsüzlük sağlayan profesyonel sir ağda.",
                    SeoKeywords = "Bacak Ağdası, Sir Ağda", 
                    CategoryId = categories[3].Id 
                }
            };

            context.Services.AddRange(services);
            context.SaveChanges();

            // Müşteri Yorumları (Testimonials)
            var testimonials = new Testimonial[]
            {
                new Testimonial { 
                    CustomerName = "Sezen Dayanç", 
                    Content = "Çok memnun kaldım, güler yüzlü sahibi, temiz ve özenli.", 
                    Response = "Teşekkür ederim 💞", 
                    IsApproved = true, 
                    DisplayOrder = 1 
                },
                // Diğer yorumlar buraya eklenebilir... (kısaltıldı)
            };

            context.Testimonials.AddRange(testimonials);
            context.SaveChanges();
        }
    }
}
