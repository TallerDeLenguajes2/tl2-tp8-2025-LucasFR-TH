# Script PowerShell para crear la tabla de Usuarios en SQLite
# Ejecutar desde PowerShell como administrador

# Ruta a la base de datos
$dbPath = "Db/Tienda.db"
$sqlScript = "Db/SetupDatabase.sql"

# Verificar si sqlite3 está disponible
try {
    $sqlite3Version = sqlite3 --version
    Write-Host "✓ SQLite3 encontrado: $sqlite3Version" -ForegroundColor Green
} catch {
    Write-Host "✗ SQLite3 no está disponible en el PATH" -ForegroundColor Red
    Write-Host "Instale SQLite3 o use SQLiteStudio para ejecutar el script" -ForegroundColor Yellow
    exit 1
}

# Verificar si la base de datos existe
if (Test-Path $dbPath) {
    Write-Host "✓ Base de datos encontrada: $dbPath" -ForegroundColor Green
} else {
    Write-Host "⚠ Base de datos no encontrada, se creará una nueva: $dbPath" -ForegroundColor Yellow
}

# Ejecutar el script SQL
Write-Host "`nEjecutando script SQL..." -ForegroundColor Cyan
try {
    # Leer el contenido del script SQL
    $sqlContent = Get-Content $sqlScript -Raw
    
    # Ejecutar el script
    $sqlContent | sqlite3 $dbPath
    
    Write-Host "✓ Script ejecutado correctamente" -ForegroundColor Green
    Write-Host "`nVerificando tabla de usuarios..." -ForegroundColor Cyan
    
    # Verificar los datos
    sqlite3 $dbPath "SELECT * FROM usuarios;" | Write-Host
    
} catch {
    Write-Host "✗ Error al ejecutar script: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n✓ Setup completado exitosamente" -ForegroundColor Green
