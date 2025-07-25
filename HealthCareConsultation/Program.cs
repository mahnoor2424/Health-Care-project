using HealthCareConsultation.Data;
using HealthCareConsultation.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Add DbContext
builder.Services.AddDbContext<HealthcareDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session and HTTP context accessor
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<ChatHub>("/chatHub");

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/Dashboard",
    defaults: new { controller = "AdminDashboard", action = "Index" });

app.MapControllerRoute(
    name: "doctor",
    pattern: "Doctor/Dashboard",
    defaults: new { controller = "DoctorDashboard", action = "Index" });

app.MapControllerRoute(
    name: "patient",
    pattern: "Patient/Dashboard",
    defaults: new { controller = "PatientDashboard", action = "Index" });

app.Run();