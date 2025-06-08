using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class SiteSettings
    {
        public SiteSettings()
        {
            // Set the default values
            WhatsAppUrl = "905375890642";
            InstagramUrl = "Tina35izmir";
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SiteName { get; set; } = "Tina Bayan Kuaför Güzellik Salonu";

        [StringLength(500)]
        public string? SiteSlogan { get; set; } = "Burada her şey senin için... Çünkü herkesin güzellik anlayışı, cilt yapısı ve beklentisi farklı. O yüzden salonumda her bakım uygulamasını tamamen sana özel hazırlıyor, birebir ilgileniyor ve sadece senin iyi hissetmen için zaman ayırıyorum.";

        [StringLength(200)]
        public string? Address { get; set; } = "Onur, Salkım Sk. 9/A, 35330 Balçova/İzmir";

        [StringLength(20)]
        [Phone]
        public string? PhoneNumber { get; set; } = "0537 589 06 42";

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? GoogleMapsUrl { get; set; } = "https://www.google.com/maps/dir/?api=1&destination=38.391642271836524,27.051524376652498&destination_place_id=ChIJYTN3sDjKuRQR5zAVB5DQ08A&travelmode=driving";

        [StringLength(200)]
        public string? FacebookUrl { get; set; }

        private string? _instagramUrl;

        [StringLength(200)]
        public string? InstagramUrl
        {
            get => _instagramUrl;
            set => _instagramUrl = FormatInstagramUrl(value);
        }

        private string? _whatsAppUrl;

        [StringLength(200)]
        public string? WhatsAppUrl
        {
            get => _whatsAppUrl;
            set => _whatsAppUrl = FormatWhatsAppUrl(value);
        }

        [StringLength(1000)]
        public string? AboutContent { get; set; } = "Merhaba, ben Fatma. Tinakuaför'ü kurarken hayalim; müşterilerime sadece güzellik hizmeti sunmak değil, aynı zamanda kendilerini rahat, güvende ve özel hissedecekleri bir alan oluşturmaktı. ✨ Bugün bu hayalimi tek başıma, ama büyük bir özveriyle sürdürüyorum. Tinakuaför'de her şey birebir ilgi ve özenle gerçekleşir. ✂️ Saç kesimi 🎨 Renklendirme 💆‍♀️ Bakım uygulamaları 👗 Stil danışmanlığı ve daha birçok güzellik hizmetini, tamamen kişiye özel sunuyorum. Hijyen, samimiyet ve profesyonellikten taviz vermeden, gelen her misafirimle tek tek ilgileniyorum. Randevular bir sıraya değil, sana özel zamana göre planlanır. ⏰ Burada; dinlenebileceğin, kendinle ilgilenebileceğin ve yenilenebileceğin bir deneyim yaşaman için buradayım.";

        [StringLength(1000)]
        public string? SeoMetaDescription { get; set; } = "İzmir Balçova'da profesyonel kadın kuaförü ve güzellik salonu. Saç kesimi, boyama, bakım, manikür, pedikür ve daha fazlası için Tina Kuaför'e bekleriz.";

        [StringLength(500)]
        public string? SeoMetaKeywords { get; set; } = "İzmir Kuaför, İzmir Güzellik Salonu, İzmir Balçova Kuaför, İzmir Balçova Kadın Kuaför, İzmir Balçova Güzellik Salonu, İzmir Balçova Kadın Güzellik Salonu, İzmir Güvenilir Kuaför, İzmir Kalıcı Oje, İzmir Manikür, İzmir Pedikür";

        // Telegram settings
        [StringLength(500)]
        public string? TelegramApiKey { get; set; } = "7868590635:AAG8j0WA-thE5mbAKHxDFu5bRcyzgi3E4dY";

        [StringLength(500)]
        public string? TelegramChatId { get; set; } = "7464909625";

        // Helper method to parse comma-separated chat IDs
        public List<string> GetTelegramChatIds()
        {
            if (string.IsNullOrEmpty(TelegramChatId))
                return new List<string>();

            var chatIds = TelegramChatId.Split(',', ';')
                .Select(id => id.Trim())
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            // Ensure chat IDs start with @ for channel handles
            for (int i = 0; i < chatIds.Count; i++)
            {
                string id = chatIds[i];

                // If it's a username and doesn't start with @, add it
                if (!id.StartsWith("@") && !long.TryParse(id, out _))
                {
                    chatIds[i] = "@" + id;
                }
            }

            return chatIds;
        }

        // Helper method to format Instagram URL
        private string? FormatInstagramUrl(string? url)
        {
            // If URL is empty, return it as is
            if (string.IsNullOrWhiteSpace(url))
                return url;

            // Handle the specific format requested by the user (@https://www.instagram.com/tina35izmir/)
            if (url.Contains("@https://www.instagram.com/"))
                return url;

            // If it's already a full URL, return it as is
            if (url.StartsWith("http://") || url.StartsWith("https://"))
                return url;

            // Extract just the username part
            string username = url;

            // Remove @ symbol if present
            if (username.StartsWith("@"))
                username = username.Substring(1);

            // Extract username if it contains instagram.com
            if (username.Contains("instagram.com/"))
            {
                var parts = username.Split(new[] { "instagram.com/" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                    username = parts[parts.Length - 1];
            }

            // Clean up the username
            username = username.Trim('/').Trim();

            // Return with the exact format requested
            return $"@https://www.instagram.com/{username}/";
        }

        // Helper method to format WhatsApp URL
        private string? FormatWhatsAppUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            // If it already starts with http/https, return as is
            if (url.StartsWith("http://") || url.StartsWith("https://"))
                return url;

            // Clean up the phone number
            var phoneNumber = url.Replace(" ", "")
                                .Replace("-", "")
                                .Replace("(", "")
                                .Replace(")", "")
                                .Replace("+", "");

            // If it starts with 0, replace with 90 for Turkey
            if (phoneNumber.StartsWith("0"))
                phoneNumber = "9" + phoneNumber;

            // Return the full WhatsApp URL
            return $"https://wa.me/{phoneNumber}";
        }

        // Admin settings
        [StringLength(100)]
        public string? AdminPassword { get; set; } = "199034";
    }
}
