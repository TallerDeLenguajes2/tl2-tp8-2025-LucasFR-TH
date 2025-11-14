-- Script SQL para crear tabla de Usuarios directamente en SQLite
-- Ejecutar en SQLiteStudio o con: sqlite3 Db/Tienda.db < Db/SetupDatabase.sql

-- PASO 1: Crear tabla Usuarios (si no existe)
CREATE TABLE IF NOT EXISTS usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    User TEXT NOT NULL UNIQUE,
    Pass TEXT NOT NULL,
    Rol TEXT NOT NULL CHECK(Rol IN ('Administrador', 'Cliente'))
);

-- PASO 2: Limpiar datos anteriores (opcional - descomenta si necesitas reiniciar)
-- DELETE FROM usuarios;

-- PASO 3: Insertar datos de prueba
-- Administrador
INSERT OR IGNORE INTO usuarios (Nombre, User, Pass, Rol) 
VALUES ('Administrador Principal', 'admin', 'admin123', 'Administrador');

-- Clientes
INSERT OR IGNORE INTO usuarios (Nombre, User, Pass, Rol) 
VALUES ('Cliente Uno', 'cliente1', 'pass123', 'Cliente');

INSERT OR IGNORE INTO usuarios (Nombre, User, Pass, Rol) 
VALUES ('Cliente Dos', 'cliente2', 'pass456', 'Cliente');

-- PASO 4: Verificar datos insertados
SELECT * FROM usuarios;
