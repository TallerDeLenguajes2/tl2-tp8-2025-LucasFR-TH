var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ==================== INYECCIÓN DE DEPENDENCIAS ====================
// DESCRIPCIÓN: Esta sección configura el contenedor de DI de ASP.NET Core.
// Todos los servicios se registran con ciclo de vida SCOPED (nueva instancia por HTTP request).
// Esto permite que cada request tenga su propia instancia de los repositorios y servicios.

// PASO 1: Registrar IHttpContextAccessor
// PROPÓSITO: Permite acceder al HttpContext desde cualquier servicio inyectado.
// USO: AuthenticationService lo usa para leer/escribir variables de sesión.
builder.Services.AddHttpContextAccessor();

// PASO 2: Registrar servicios de sesión
// PROPÓSITO: Configurar el sistema de sesiones de ASP.NET Core.
// CONFIGURACIÓN:
//   - IdleTimeout: Sesión expira después de 30 minutos de inactividad (seguridad)
//   - HttpOnly: La cookie solo se envía a través de HTTP (previene XSS attacks)
//   - IsEssential: Marca como esencial para GDPR compliance
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // 30 minutos de timeout
    options.Cookie.HttpOnly = true;                  // Seguridad: previene acceso desde JavaScript
    options.Cookie.IsEssential = true;               // GDPR: cookie esencial
});

// PASO 3: Registrar Repositorio de Productos
// PATRÓN: Implementación del patrón Repository Pattern
// - IProductoRepository: interfaz que define el contrato
// - ProductoRepository: implementación concreta que accede a la BD
builder.Services.AddScoped<repositorioProducto.ProductoRepository>();
builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IProductoRepository>(
    sp => sp.GetRequiredService<repositorioProducto.ProductoRepository>());

// PASO 4: Registrar Repositorio de Presupuestos
builder.Services.AddScoped<repositorioPresupuesto.PresupuestosRepository>();
builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IPresupuestoRepository>(
    sp => sp.GetRequiredService<repositorioPresupuesto.PresupuestosRepository>());

// PASO 5: Registrar Repositorio de Usuarios
// PROPÓSITO: Acceso a datos de usuarios para autenticación
// MÉTODO: GetUser(username, password) usa consultas parametrizadas para seguridad
builder.Services.AddScoped<repositorioUsuario.UserRepository>();
builder.Services.AddScoped<tl2_tp8_2025_LucasFR_TH.Interfaces.IUserRepository>(
    sp => sp.GetRequiredService<repositorioUsuario.UserRepository>());

// PASO 6: Registrar Servicio de Autenticación
// PROPÓSITO: Centralizar la lógica de autenticación y autorización
// FUNCIONALIDAD:
//   - Login: autentica usuario y crea sesión
//   - IsAuthenticated: verifica si hay sesión válida
//   - HasAccessLevel: verifica si el usuario tiene un rol específico
//   - GetCurrentUser/GetCurrentUserRole: obtiene info del usuario autenticado
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

// ORDEN CRÍTICO: UseSession DEBE ir ANTES de UseAuthorization
// Razón: Las sesiones deben estar disponibles cuando se evalúan autorización
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

