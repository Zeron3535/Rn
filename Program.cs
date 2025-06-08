using Microsoft.EntityFrameworkCore;
using TinaKuafor.Data;
using TinaKuafor.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Detaylı loglama ekle
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Railway için port yapılandırması
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"Configuring app to listen on port: {port}");
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Configure antiforgery
builder.Services.AddAntiforgery(options => {
    options.HeaderName = "X-CSRF-TOKEN";
});

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add PostgreSQL database connection
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL")
    ?? "Host=localhost;Database=tinaKuafor;Username=postgres;Password=password";

// Convert Railway DATABASE_URL format to connection string format if needed
if (connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}

// Add domain handling configuration
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                              Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Railway ortam değişkenlerini kontrol et ve logla
Console.WriteLine("Environment variables:");
foreach (var env in Environment.GetEnvironmentVariables().Keys)
{
    Console.WriteLine($"{env}: {Environment.GetEnvironmentVariable(env?.ToString() ?? "")}");
}

Console.WriteLine($"Using PostgreSQL connection: {connectionString.Replace(connectionString.Split("Password=")[1].Split(";")[0], "***")}");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// Add background services
builder.Services.AddHostedService<AppointmentReminderService>();
builder.Services.AddHostedService<DailyStatsService>();

// Override configuration with environment variables if available
var telegramApiKey = Environment.GetEnvironmentVariable("TELEGRAM_API_KEY");
var telegramChatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

if (!string.IsNullOrEmpty(telegramApiKey))
{
    builder.Configuration["TelegramBot:ApiKey"] = telegramApiKey;
    Console.WriteLine("Using Telegram API Key from environment variable");
}

if (!string.IsNullOrEmpty(telegramChatId))
{
    builder.Configuration["TelegramBot:ChatId"] = telegramChatId;
    Console.WriteLine("Using Telegram Chat ID from environment variable");
}

if (!string.IsNullOrEmpty(adminPassword))
{
    builder.Configuration["AdminSettings:Password"] = adminPassword;
    Console.WriteLine("Using Admin Password from environment variable");
}

// Sağlam healthcheck ekle
builder.Services.AddHealthChecks()
    .AddCheck("Railway Health", () => HealthCheckResult.Healthy("Application is running correctly on Railway"), tags: new[] { "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Railway ortamında HTTPS yönlendirmesine gerek yok
    // app.UseHsts();
}

// Use forwarded headers for proxy scenarios (Railway, etc.)
app.UseForwardedHeaders();

// Railway ortamında HTTPS yönlendirmesine gerek yok
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Use session
app.UseSession();

// Açık healthcheck endpoint'i - JSON formatında sağlık durumu döndüren versiyon
app.MapGet("/health", () => Results.Json(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// Root path için açık bir endpoint - daha etkili yönlendirme
app.MapGet("/", context =>
{
    context.Response.Redirect("/anasayfa", permanent: true);
    return Task.CompletedTask;
});

// Türkçe URL yapısı için özel rotalar
app.MapControllerRoute(
    name: "anasayfa",
    pattern: "anasayfa",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "hizmetler",
    pattern: "hizmetler",
    defaults: new { controller = "Services", action = "Index" });

app.MapControllerRoute(
    name: "hizmet-kategori",
    pattern: "hizmetler/{slug}",
    defaults: new { controller = "Services", action = "Category" });

app.MapControllerRoute(
    name: "hizmet-detay",
    pattern: "hizmet/{slug}",
    defaults: new { controller = "Services", action = "Details" });

app.MapControllerRoute(
    name: "randevu",
    pattern: "randevu",
    defaults: new { controller = "Appointments", action = "Index" });

app.MapControllerRoute(
    name: "iletisim",
    pattern: "iletisim",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "hakkimda",
    pattern: "hakkimda",
    defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
    name: "kullanim-kosullari",
    pattern: "kullanim-kosullari",
    defaults: new { controller = "Home", action = "TermsOfService" });

app.MapControllerRoute(
    name: "gizlilik-politikasi",
    pattern: "gizlilik-politikasi",
    defaults: new { controller = "Home", action = "PrivacyPolicy" });

app.MapControllerRoute(
    name: "cerez-politikasi",
    pattern: "cerez-politikasi",
    defaults: new { controller = "Home", action = "CookiePolicy" });

// Varsayılan route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created
try 
{
    Console.WriteLine("Starting application initialization...");
    Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
    Console.WriteLine("Connecting to PostgreSQL database...");

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            Console.WriteLine("Getting database context...");
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Run database migrations
            Console.WriteLine("Running database migrations...");
            context.Database.Migrate();

            // Örnek veri ekle
            Console.WriteLine("Initializing database with sample data...");
            DbInitializer.Initialize(context);

            // Log başarılı veritabanı oluşturma
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("PostgreSQL database initialized successfully");
            Console.WriteLine("PostgreSQL database initialized successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while connecting to PostgreSQL database");
            Console.WriteLine($"ERROR connecting to PostgreSQL database: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    // Başlangıç mesajı
    Console.WriteLine($"Application initialization completed. Listening on port {port}");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"CRITICAL ERROR - Application failed to start: {ex.Message}");
    Console.WriteLine(ex.StackTrace);

    // Log to a file as well in case console output is not captured
    try {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDir);
        var logFile = Path.Combine(logDir, "startup_error.log");
        File.WriteAllText(logFile, $"Application failed to start: {ex.Message}\n{ex.StackTrace}");
    }
    catch { /* Ignore errors writing to log file */ }

    throw;
}
