-- Script para crear la tabla de Usuarios en SQLite
-- Ejecutar este script en SQLiteStudio o sqlite3

-- Crear tabla Usuarios
CREATE TABLE IF NOT EXISTS usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    User TEXT NOT NULL UNIQUE,
    Pass TEXT NOT NULL,
    Rol TEXT NOT NULL CHECK(Rol IN ('Administrador', 'Cliente'))
);

-- Insertar datos de prueba
INSERT INTO usuarios (Nombre, User, Pass, Rol) VALUES
    ('Administrador Principal', 'admin', 'admin123', 'Administrador'),
    ('Cliente Uno', 'cliente1', 'pass123', 'Cliente'),
    ('Cliente Dos', 'cliente2', 'pass456', 'Cliente');

-- Verificar datos insertados
SELECT * FROM usuarios;
