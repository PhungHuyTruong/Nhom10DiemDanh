using API.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Nhom10ModuleDiemDanh.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<ModuleDiemDanhDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC và Validation
builder.Services.AddControllersWithViews()
        .AddViewOptions(options =>
        {
            options.HtmlHelperOptions.ClientValidationEnabled = true;
        });

// HTTP Clients
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICoSoService, CoSoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7296/");
});
builder.Services.AddHttpClient<IBoMonCoSoService, BoMonCoSoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7296/");
});
builder.Services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7296/api/");
});

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];

    options.Events.OnCreatingTicket = async context =>
    {
        var email = context.Identity?.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;

        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ModuleDiemDanhDbContext>();

            // Ki?m tra t?ng b?ng, ? ðây ch? check PhuTrachXuong
            var user = await dbContext.PhuTrachXuongs
                .FirstOrDefaultAsync(u => u.EmailFE == email || u.EmailFPT == email);

            if (user != null)
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Role, "GiangVien"));
                context.Identity.AddClaim(new Claim(ClaimTypes.Name, user.TenNhanVien));
                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.IdNhanVien.ToString()));
            }
            // KHÔNG redirect t?i ðây
            // N?u không t?m th?y th? c? ð? login thành công nhýng không có claim Role, h? th?ng s? t? ch?n sau khi vào các controller có [Authorize(Roles="...")]
        }
    };

    options.Events.OnRemoteFailure = context =>
    {
        context.HandleResponse();
        context.Response.Redirect("/Home/Index");
        return Task.CompletedTask;
    };
});

// App
var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=Index}/{id?}");

app.Run();
