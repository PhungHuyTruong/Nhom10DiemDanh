using API.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Nhom10ModuleDiemDanh.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<ModuleDiemDanhDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpClient
builder.Services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7296/api/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
});

builder.Services.AddHttpClient<ICoSoService, CoSoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7296/"); // Ð?m b?o kh?p v?i API project
});

builder.Services.AddHttpClient<IBoMonCoSoService, BoMonCoSoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7296/");
});

builder.Services.AddHttpClient<IKeHoachService, KeHoachService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7286/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IKeHoachService, KeHoachService>();

//builder.Services.AddHttpClient("MyApi", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7296/api/");
//});
 

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });


builder.Services.AddHttpClient();

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
builder.Services.AddSession();
app.UseSession();

app.UseAuthentication();  
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DuAns}/{action=Index}/{id?}");

app.Run();