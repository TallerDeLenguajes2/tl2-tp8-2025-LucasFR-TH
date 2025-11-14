# Setup de Base de Datos - Tabla Usuarios

## 📋 Descripción

Este directorio contiene los scripts SQL necesarios para configurar la tabla de `usuarios` en la base de datos SQLite.

## 📁 Archivos

- **SetupDatabase.sql**: Script SQL con definición de tabla e inserts de datos de prueba
- **SetupDatabase.ps1**: Script PowerShell para ejecutar automaticamente el SQL
- **Tienda.db**: Archivo de base de datos SQLite (generado)

## 🚀 Instrucciones de Uso

### Opción 1: Usando SQLiteStudio (GUI)

1. Abre SQLiteStudio
2. Conecta a la base de datos `Db/Tienda.db`
3. Copia el contenido de `SetupDatabase.sql`
4. Pega y ejecuta en la pestaña SQL de SQLiteStudio

### Opción 2: Usando sqlite3 desde Terminal

```powershell
cd "ruta/al/proyecto"
Get-Content Db/SetupDatabase.sql | sqlite3 Db/Tienda.db
```

### Opción 3: Usando el Script PowerShell

```powershell
cd "ruta/al/proyecto"
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\Db\SetupDatabase.ps1
```

## 📊 Tabla de Usuarios

### Estructura

```sql
CREATE TABLE usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    User TEXT NOT NULL UNIQUE,
    Pass TEXT NOT NULL,
    Rol TEXT NOT NULL CHECK(Rol IN ('Administrador', 'Cliente'))
);
```

### Campos

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | INTEGER | Identificador único (autoincremento) |
| Nombre | TEXT | Nombre completo del usuario |
| User | TEXT | Nombre de usuario para login (único) |
| Pass | TEXT | Contraseña del usuario |
| Rol | TEXT | Rol: 'Administrador' o 'Cliente' |

## 🧑‍💻 Datos de Prueba

| User | Pass | Rol | Nombre |
|------|------|-----|--------|
| admin | admin123 | Administrador | Administrador Principal |
| cliente1 | pass123 | Cliente | Cliente Uno |
| cliente2 | pass456 | Cliente | Cliente Dos |

## ⚠️ Notas Importantes

- Las contraseñas en este archivo están en texto plano por propósitos educativos. **En producción deben estar hasheadas**.
- El campo `User` tiene restricción UNIQUE: no puede haber dos usuarios con el mismo nombre de usuario.
- El campo `Rol` solo acepta dos valores: 'Administrador' o 'Cliente'.
- Si deseas reiniciar los datos, descomenta la línea `DELETE FROM usuarios;` en el script SQL.

## 🔍 Verificar Datos

Para verificar que la tabla se creó correctamente y contiene los datos:

```powershell
sqlite3 Db/Tienda.db "SELECT * FROM usuarios;"
```

O para ver la estructura:

```powershell
sqlite3 Db/Tienda.db ".schema usuarios"
```

## 🔗 Conexión desde C#

La aplicación usa esta cadena de conexión:

```csharp
string cadenaConexion = "Data Source = Db/Tienda.db";
```

El `UserRepository` lee automáticamente desde esta tabla para autenticar usuarios.
