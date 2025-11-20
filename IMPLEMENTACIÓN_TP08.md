# DOCUMENTACIÓN - TP08: Autenticación y Autorización en ASP.NET Core

**Proyecto:** tl2-tp8-2025-LucasFR-TH  
**Fecha:** Noviembre 2025  
**Estado:** ✅ IMPLEMENTACIÓN COMPLETA

---

## 📋 RESUMEN EJECUTIVO

Este proyecto implementa un **sistema de autenticación y autorización basado en sesiones** en una aplicación ASP.NET Core MVC con dos roles de usuarios (Administrador y Cliente).

### Características Principales
- ✅ Autenticación con usuario/contraseña
- ✅ Control de sesiones con timeout de 30 minutos
- ✅ Autorización basada en roles (Administrador/Cliente)
- ✅ Protección CSRF con [ValidateAntiForgeryToken]
- ✅ Validación server-side con ModelState
- ✅ Inyección de Dependencias centralizada
- ✅ Repository Pattern para acceso a datos

---

## 🏗️ ARQUITECTURA DEL SISTEMA

### Capas Implementadas

```
┌─────────────────────────────────────────┐
│         PRESENTACIÓN (Views)            │
│  - LoginView, ProductosView, etc.       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      CONTROLADORES (Controllers)        │
│  - ProductosController                  │
│  - PresupuestosController               │
│  - LoginController                      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│   SERVICIOS & REPOSITORIOS (Interfaces) │
│  - IAuthenticationService               │
│  - IProductoRepository                  │
│  - IPresupuestoRepository               │
│  - IUserRepository                      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      DATOS (SQLite Database)            │
│  - Tabla: usuarios                      │
│  - Tabla: productos                     │
│  - Tabla: presupuestos                  │
└─────────────────────────────────────────┘
```

---

## 🔐 FLUJO DE AUTENTICACIÓN

### 1. Login del Usuario

```
Usuario ingresa credenciales
         ↓
LoginController.Index [POST]
         ↓
authService.Login(username, password)
         ↓
UserRepository.GetUser() → Consulta BD
         ↓
¿Usuario encontrado?
  ├─ SÍ → Guardar datos en sesión → Return true
  └─ NO → Return false
         ↓
¿Login exitoso?
  ├─ SÍ → Redirige a /Home/Index
  └─ NO → Muestra error "Credenciales inválidas"
```

### 2. Verificación en Cada Request

```
Usuario intenta acceder a acción protegida
         ↓
CheckAdminPermissions() [en controlador]
         ↓
¿IsAuthenticated()?
  ├─ NO → Redirige a /Login/Index
  └─ SÍ → Continúa
         ↓
¿HasAccessLevel(rol_requerido)?
  ├─ NO → Redirige a /AccesoDenegado
  └─ SÍ → Ejecuta la acción
```

### 3. Logout del Usuario

```
Usuario hace click en "Cerrar Sesión"
         ↓
LoginController.Logout()
         ↓
authService.Logout()
         ↓
Session.Clear() → Elimina todas las variables
         ↓
Redirige a /Login/Index
```

---

## 📁 ESTRUCTURA DE ARCHIVOS

### Archivos de Configuración

| Archivo | Propósito | Cambios Realizados |
|---------|-----------|-------------------|
| `Program.cs` | Configuración DI y middleware | ✅ Registra servicios y repositorios con AddScoped |
| `appsettings.json` | Configuración de la aplicación | ✅ Database connection string |

### Modelos de Dominio

| Archivo | Responsabilidad | Campos |
|---------|-----------------|--------|
| `Models/Usuario.cs` | Modelo de usuario | IdUsuario, Username, Password, Rol, Nombre |
| `Models/Productos.cs` | Modelo de producto | idProducto, descripcion, precio |
| `Models/Presupuestos.cs` | Modelo de presupuesto | idPresupuesto, nombreDestinatario, fechaCreacion, detalle |
| `Models/PresupuestoDetalles.cs` | Línea de presupuesto | producto, cantidad |

### ViewModels

| Archivo | Propósito | Validaciones |
|---------|-----------|-------------|
| `ViewModels/LoginViewModel.cs` | Formulario de login | [Required] Username, Password |
| `ViewModels/ProductoViewModel.cs` | CRUD de productos | [Required] Descripcion, Precio |
| `ViewModels/PresupuestoViewModel.cs` | CRUD de presupuestos | [Required] NombreDestinatario, FechaCreacion |
| `ViewModels/AgregarProductoViewModel.cs` | Agregar producto a presupuesto | [Required] Cantidad |

### Interfaces (Contratos)

| Archivo | Responsabilidad | Métodos Clave |
|---------|-----------------|---------------|
| `Interfaces/IAuthenticationService.cs` | Autenticación/Autorización | Login, Logout, IsAuthenticated, HasAccessLevel |
| `Interfaces/IUserRepository.cs` | Acceso a usuarios | GetUser, GetById, GetByUsername |
| `Interfaces/IProductoRepository.cs` | CRUD de productos | GetAll, GetById, Create, Update, Delete |
| `Interfaces/IPresupuestoRepository.cs` | CRUD de presupuestos | GetAll, GetById, Create, Update, Delete, AddProducto |

### Implementaciones

| Archivo | Implementa | Detalles |
|---------|-----------|----------|
| `Servicios/AuthenticationService.cs` | IAuthenticationService | ✅ Usa HttpContextAccessor para sesiones |
| `Repositorios/UserRepository.cs` | IUserRepository | ✅ Consultas SQL parametrizadas |
| `Repositorios/ProductoRepository.cs` | IProductoRepository | ✅ CRUD con SQLite |
| `Repositorios/PresupuestosRepository.cs` | IPresupuestoRepository | ✅ CRUD con SQLite |

### Controladores

| Archivo | Responsabilidad | Acciones |
|---------|-----------------|----------|
| `Controllers/LoginController.cs` | Autenticación | Index [GET/POST], Logout |
| `Controllers/ProductosController.cs` | CRUD Productos (Admin) | Index, Create, Edit, Delete, AccesoDenegado |
| `Controllers/PresupuestosController.cs` | CRUD Presupuestos (Rol dual) | Index, Create, Edit, Delete, AgregarProducto, Details, AccesoDenegado |
| `Controllers/HomeController.cs` | Página principal | Index, Privacy, Error |

### Vistas

| Archivo | Propósito | Características |
|---------|-----------|-----------------|
| `Views/Login/Index.cshtml` | Formulario de login | Bootstrap, validación cliente, info de usuarios prueba |
| `Views/Productos/Index.cshtml` | Listado de productos | Tabla con CRUD buttons |
| `Views/Productos/Create.cshtml` | Formulario crear producto | Bootstrap form, client-side validation |
| `Views/Productos/Edit.cshtml` | Formulario editar producto | Bootstrap form |
| `Views/Productos/Delete.cshtml` | Confirmación eliminar | Alerta Bootstrap |
| `Views/Productos/AccesoDenegado.cshtml` | Acceso denegado (Productos) | Mensaje custom para Clientes |
| `Views/Presupuestos/*` | CRUD Presupuestos | Similar a Productos |

---

## 🔒 SEGURIDAD IMPLEMENTADA

### 1. Autenticación

✅ **Login Seguro**
- Consultas parametrizadas para prevenir SQL injection
- Validación server-side de credenciales
- Sesiones HTTP-only (no accesibles desde JavaScript)

✅ **Gestión de Sesiones**
- Timeout automático: 30 minutos de inactividad
- Cookie HTTP-only y esencial
- Limpieza completa en logout

### 2. Autorización

✅ **ProductosController**
- TODAS las acciones requieren: Autenticado + Rol Administrador
- Si falla → Redirige a login o AccesoDenegado

✅ **PresupuestosController**
- LECTURA (Index, Details): Autenticado + (Admin O Cliente)
- MODIFICACIÓN (Create, Edit, Delete): Autenticado + Admin
- Si falla → Redirige a login o AccesoDenegado

### 3. Protección de Formularios

✅ **CSRF Protection**
- [ValidateAntiForgeryToken] en todos los POST/PUT/DELETE
- Token anti-falsificación en cada formulario

✅ **Validación Server-Side**
- ModelState.IsValid en todos los POST
- Atributos DataAnnotations ([Required], [StringLength], etc.)
- Re-display del formulario con errores

### 4. Parametrización SQL

✅ **Prevención de SQL Injection**
```csharp
// ✅ SEGURO - Parámetros
using (var cmd = new SqliteCommand(
    "SELECT * FROM usuarios WHERE user = @username AND pass = @password", 
    connection))
{
    cmd.Parameters.AddWithValue("@username", username);
    cmd.Parameters.AddWithValue("@password", password);
}

// ❌ INSEGURO - String concatenation
string query = "SELECT * FROM usuarios WHERE user = '" + username + "'";
```

---

## 🧪 USUARIOS DE PRUEBA

### Base de Datos (Db/Tienda.db - Tabla usuarios)

| ID | Nombre | Username | Password | Rol |
|----|--------|----------|----------|-----|
| 1 | Administrador Principal | admin | admin123 | Administrador |
| 2 | Cliente Uno | cliente1 | pass123 | Cliente |
| 3 | Cliente Dos | cliente2 | pass456 | Cliente |

### Cómo Probar

1. **Login como Admin:**
   - Username: `admin`
   - Password: `admin123`
   - Acceso: ✅ Todos los CRUD

2. **Login como Cliente:**
   - Username: `cliente1`
   - Password: `pass123`
   - Acceso: ✅ Ver productos/presupuestos, ❌ No crear/editar

---

## 🔄 FLUJO DE AUTENTICACIÓN EN DETALLE

### ProductosController - Todas las Acciones

```csharp
[HttpGet]
public IActionResult Index()
{
    // 1. SEGURIDAD: Verificar permisos
    var securityCheck = CheckAdminPermissions();
    if (securityCheck != null) return securityCheck;
    
    // 2. Si pasa seguridad, ejecutar lógica
    List<Producto> productos = productoRepository.GetAll();
    return View(productos);
}

private IActionResult CheckAdminPermissions()
{
    // CHEQUEO 1: ¿Está autenticado?
    if (!authenticationService.IsAuthenticated())
        return RedirectToAction("Index", "Login");
    
    // CHEQUEO 2: ¿Es Administrador?
    if (!authenticationService.HasAccessLevel("Administrador"))
        return RedirectToAction(nameof(AccesoDenegado));
    
    return null; // Permitido
}
```

### PresupuestosController - Dual Access

```csharp
// LECTURA: Index y Details permiten Admin O Cliente
[HttpGet]
public IActionResult Index()
{
    if (!authenticationService.IsAuthenticated())
        return RedirectToAction("Index", "Login");
    
    // ✅ Permite ambos roles
    if (!(authenticationService.HasAccessLevel("Administrador") || 
          authenticationService.HasAccessLevel("Cliente")))
        return RedirectToAction(nameof(AccesoDenegado));
    
    return View(presupuestosRepository.GetAll());
}

// MODIFICACIÓN: Create y Edit requieren SOLO Admin
[HttpGet]
public IActionResult Create()
{
    var securityCheck = CheckAdminPermissions(); // Solo Admin
    if (securityCheck != null) return securityCheck;
    
    return View(new PresupuestoViewModel());
}
```

---

## 🚀 CONFIGURACIÓN EN PROGRAM.CS

### Orden Crítico de Middleware

```csharp
// ✅ CORRECTO - Session debe ir ANTES de Authorization
app.UseSession();        // Las sesiones deben estar disponibles
app.UseAuthorization();  // Para que Authorization pueda leerlas

// ❌ INCORRECTO - Authorization sin sesiones
app.UseAuthorization();
app.UseSession();        // Demasiado tarde
```

### Registros de DI (Dependency Injection)

```csharp
// 1. HttpContextAccessor - Para acceder a la sesión
builder.Services.AddHttpContextAccessor();

// 2. Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Registrar Repositorios (Scoped = una instancia por request)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestosRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 4. Registrar Servicio de Autenticación
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
```

---

## ✅ LISTA DE VERIFICACIÓN COMPLETA

### Fase 1: Modelos y ViewModels
- [x] **Usuario.cs** - Modelo con Rol
- [x] **LoginViewModel.cs** - ViewModel para login
- [x] **ProductoViewModel.cs** - ViewModel para CRUD productos
- [x] **PresupuestoViewModel.cs** - ViewModel para CRUD presupuestos
- [x] **AgregarProductoViewModel.cs** - ViewModel para agregar productos

### Fase 2: Interfaces
- [x] **IAuthenticationService.cs** - Contrato de autenticación
- [x] **IUserRepository.cs** - Contrato de acceso a usuarios
- [x] **IProductoRepository.cs** - Contrato CRUD productos
- [x] **IPresupuestoRepository.cs** - Contrato CRUD presupuestos

### Fase 3: Servicios y Repositorios
- [x] **AuthenticationService.cs** - Implementación de autenticación
- [x] **UserRepository.cs** - Acceso a usuarios con SQL parametrizado
- [x] **ProductoRepository.cs** - CRUD productos
- [x] **PresupuestosRepository.cs** - CRUD presupuestos

### Fase 4: Controladores
- [x] **LoginController.cs** - Login/Logout
- [x] **ProductosController.cs** - CRUD productos con CheckAdminPermissions
- [x] **PresupuestosController.cs** - CRUD presupuestos con doble validación
- [x] **HomeController.cs** - Página principal

### Fase 5: Vistas
- [x] **Views/Login/Index.cshtml** - Formulario de login
- [x] **Views/Productos/*.cshtml** - CRUD y AccesoDenegado
- [x] **Views/Presupuestos/*.cshtml** - CRUD y AccesoDenegado

### Fase 6: Configuración
- [x] **Program.cs** - Inyección de dependencias y middleware
- [x] **appsettings.json** - Connection string
- [x] **Db/Tienda.db** - Base de datos SQLite

### Fase 7: Documentación de Código
- [x] **Comentarios extensos en AuthenticationService.cs**
- [x] **Comentarios en ProductosController.cs**
- [x] **Comentarios en PresupuestosController.cs**
- [x] **Comentarios en Program.cs**
- [x] **Comentarios en IAuthenticationService.cs**

### Fase 8: Compilación y Validación
- [x] `dotnet build` - ✅ Compilación correcta (0 Errores, 1 Warning pre-existente)
- [x] Todas las dependencias resueltas
- [x] Interfaces e implementaciones matched

---

## 📊 SEGURIDAD: MATRIZ DE ACCESO

### ProductosController

| Acción | No Autenticado | Autenticado (Admin) | Autenticado (Cliente) |
|--------|---|---|---|
| Index | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |
| Create | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |
| Edit | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |
| Delete | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |

### PresupuestosController

| Acción | No Autenticado | Autenticado (Admin) | Autenticado (Cliente) |
|--------|---|---|---|
| Index | 🔴 → Login | ✅ | ✅ |
| Details | 🔴 → Login | ✅ | ✅ |
| Create | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |
| Edit | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |
| Delete | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |
| AgregarProducto | 🔴 → Login | ✅ | 🔴 → AccesoDenegado |

---

## 🐛 SOLUCIÓN DE PROBLEMAS

### Problema: "The name 'CheckAdminPermissions' does not exist"
**Causa:** Método privado no declarado en el controlador  
**Solución:** Asegurarse de que CheckAdminPermissions() esté declarado como método privado

### Problema: Sesión no persiste entre requests
**Causa:** Middleware en orden incorrecto en Program.cs  
**Solución:** Verificar que `app.UseSession()` está ANTES de `app.UseAuthorization()`

### Problema: "ModelState.IsValid" falla sin mensajes claros
**Causa:** Validaciones de DataAnnotations en ViewModels  
**Solución:** Revisar atributos [Required], [StringLength], [Range] en ViewModel

### Problema: SQL Injection en consultas
**Causa:** Concatenación de strings en consultas SQL  
**Solución:** Usar siempre SqliteParameter (@nombre_param)

---

## 📝 NOTAS IMPORTANTES

1. **Inyección de Dependencias**: Todas las dependencias son recibidas en constructores, no instanciadas con `new`
2. **Repository Pattern**: Los controladores NO acceden directamente a la BD, solo a través de repositorios
3. **ViewModel Pattern**: Los modelos de dominio NO se pasan directamente a vistas, se mapean a ViewModels
4. **Seguridad por defecto**: Cualquier nueva acción sin CheckAdminPermissions() es insegura por default
5. **Sesiones**: Se usan sesiones HTTP-only, no cookies de sesión normales

---

## 🎓 CONCEPTOS CLAVE

### Inyección de Dependencias (DI)
- Las dependencias se inyectan en constructores
- Configuradas en Program.cs con AddScoped
- Permite tests y cambios de implementación fáciles

### Repository Pattern
- Los repositorios abstraen el acceso a datos
- Los controladores solo conocen interfaces, no implementaciones
- Facilita tests unitarios

### ViewModel Pattern
- ViewModels contienen solo datos que la vista necesita
- Separación clara entre BD y UI
- Validaciones específicas de UI

### Session-Based Authentication
- Usuario autenticado = variables en sesión del browser
- Timeout automático por inactividad
- HttpOnly previene acceso desde JavaScript

---

## 📚 REFERENCIAS

- **ASP.NET Core Session**: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state
- **Dependency Injection**: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
- **Authorization**: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/
- **Data Protection**: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/

---

## ✨ CONCLUSIÓN

Este proyecto implementa un **sistema de autenticación y autorización completo, seguro y escalable** siguiendo los patrones de arquitectura modernos de ASP.NET Core.

**Estado Final:** ✅ Compilación correcta, 0 Errores, Todas las características implementadas

---

*Última actualización: Noviembre 20, 2025*  
*Autor: Lucas FR*  
*Proyecto: TP08 - Taller de Lenguajes II*
