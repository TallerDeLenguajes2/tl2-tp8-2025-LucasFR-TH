var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ==================== INYECCIÓN DE DEPENDENCIAS ====================
// Registrar IHttpContextAccessor para acceso a sesiones
builder.Services.AddHttpContextAccessor();

// Registrar servicios de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Sesión expira después de 30 minutos de inactividad
    options.Cookie.HttpOnly = true;                  // La cookie solo se envía a través de HTTP (no JavaScript)
    options.Cookie.IsEssential = true;               // Essential para GDPR compliance
});

// Registrar interfaces y sus implementaciones
builder.Services.AddScoped<repositorioProducto.ProductoRepository>();
builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IProductoRepository>(
    sp => sp.GetRequiredService<repositorioProducto.ProductoRepository>());

builder.Services.AddScoped<repositorioPresupuesto.PresupuestosRepository>();
builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IPresupuestoRepository>(
    sp => sp.GetRequiredService<repositorioPresupuesto.PresupuestosRepository>());

builder.Services.AddScoped<repositorioUsuario.UserRepository>();
builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IUserRepository>(
    sp => sp.GetRequiredService<repositorioUsuario.UserRepository>());

builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IAuthenticationService, servicioAutenticacion.AuthenticationService>();

// ==================== FIN INYECCIÓN DE DEPENDENCIAS ====================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usar sesiones ANTES de UseAuthorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
