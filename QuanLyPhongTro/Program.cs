using System.Diagnostics;
using QuanLyPhongTro.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register DAL services as Scoped
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<ITaiKhoanDAL, TaiKhoanDAL>();
builder.Services.AddScoped<IPhongTroDAL, PhongTroDAL>();
builder.Services.AddScoped<IKhachThueDAL, KhachThueDAL>();
builder.Services.AddScoped<IHopDongDAL, HopDongDAL>();
builder.Services.AddScoped<IHoaDonDAL, HoaDonDAL>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var initializer = new DatabaseInitializer(scope.ServiceProvider.GetRequiredService<IConfiguration>());
    initializer.Initialize();
}

// Use CORS
app.UseCors("AllowAll");

// Use static files from wwwroot
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

// Map fallback to index.html for SPA support
app.MapFallbackToFile("login.html");

// Open Chrome/Edge in Desktop Mode (--app)
try
{
    var url = "http://localhost:5000/login.html";
    var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");

    // Try Edge first, then Chrome
    string[] browserPaths = new[]
    {
        @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
        @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
        @"C:\Program Files\Google\Chrome\Application\chrome.exe",
        @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
    };

    string? browserPath = browserPaths.FirstOrDefault(File.Exists);

    if (browserPath != null)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = browserPath,
            Arguments = $"--app={url}",
            UseShellExecute = false
        });
    }
}
catch
{
    // Ignore browser launch errors
}

app.Run();
