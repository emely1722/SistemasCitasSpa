IF DB_ID('Biblioteca') IS NULL
BEGIN 
CREATE DATABASE SistemasCitasSpa;
END
GO

USE SistemasCitasSpa;
GO

CREATE TABLE Usuarios
(
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto NVARCHAR(120) NOT NULL,
    NombreUsuario NVARCHAR(50) NOT NULL,
    Correo NVARCHAR(120) NOT NULL,
    ClaveHash NVARCHAR(255) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo)
);
GO

CREATE TABLE Clientes
(
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(60) NOT NULL,
    Apellido NVARCHAR(60) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    Correo NVARCHAR(120) NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE CategoriasServicios
(
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_CategoriasServicios_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE Servicios
(
    IdServicio INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(300) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    DuracionMinutos INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Servicios_Categorias
        FOREIGN KEY (IdCategoria)
        REFERENCES CategoriasServicios(IdCategoria),

    CONSTRAINT CK_Servicios_Precio
        CHECK (Precio >= 0),

    CONSTRAINT CK_Servicios_Duracion
        CHECK (DuracionMinutos > 0),

    CONSTRAINT UQ_Servicios_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE Terapeutas
(
    IdTerapeuta INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(60) NOT NULL,
    Apellido NVARCHAR(60) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    Correo NVARCHAR(120) NULL,
    Especialidad NVARCHAR(120) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE HorariosTerapeutas
(
    IdHorario INT IDENTITY(1,1) PRIMARY KEY,
    IdTerapeuta INT NOT NULL,
    DiaSemana NVARCHAR(15) NOT NULL,
    HoraInicio TIME(0) NOT NULL,
    HoraFin TIME(0) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_HorariosTerapeutas_Terapeutas
        FOREIGN KEY (IdTerapeuta)
        REFERENCES Terapeutas(IdTerapeuta),

    CONSTRAINT CK_HorariosTerapeutas_Dia
        CHECK (DiaSemana IN
        (
            N'Lunes',
            N'Martes',
            N'Miércoles',
            N'Jueves',
            N'Viernes',
            N'Sábado',
            N'Domingo'
        )),

    CONSTRAINT CK_HorariosTerapeutas_Horas
        CHECK (HoraFin > HoraInicio)
);
GO

CREATE TABLE Salas
(
    IdSala INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Activa BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_Salas_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE MetodosPago
(
    IdMetodoPago INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(60) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_MetodosPago_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE Citas
(
    IdCita INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    Fecha DATE NOT NULL,
    Hora TIME(0) NOT NULL,
    ServicioId INT NOT NULL,
    DuracionMinutos INT NOT NULL,
    TerapeutaId INT NOT NULL,
    SalaId INT NOT NULL,
    MetodoPagoId INT NULL,
    FechaRegistro DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Citas_Clientes
        FOREIGN KEY (ClienteId)
        REFERENCES Clientes(IdCliente),

    CONSTRAINT FK_Citas_Servicios
        FOREIGN KEY (ServicioId)
        REFERENCES Servicios(IdServicio),

    CONSTRAINT FK_Citas_Terapeutas
        FOREIGN KEY (TerapeutaId)
        REFERENCES Terapeutas(IdTerapeuta),

    CONSTRAINT FK_Citas_Salas
        FOREIGN KEY (SalaId)
        REFERENCES Salas(IdSala),

    CONSTRAINT FK_Citas_MetodosPago
        FOREIGN KEY (MetodoPagoId)
        REFERENCES MetodosPago(IdMetodoPago),

    CONSTRAINT CK_Citas_Duracion
        CHECK (DuracionMinutos > 0),

    CONSTRAINT UQ_Citas_Terapeuta
        UNIQUE (TerapeutaId, Fecha, Hora),

    CONSTRAINT UQ_Citas_Sala
        UNIQUE (SalaId, Fecha, Hora)
);
GO

CREATE INDEX IX_Citas_Fecha
ON Citas(Fecha);
GO

CREATE INDEX IX_Citas_Cliente
ON Citas(ClienteId);
GO

CREATE INDEX IX_Citas_Terapeuta
ON Citas(TerapeutaId);
GO

CREATE INDEX IX_HorariosTerapeutas_Terapeuta
ON HorariosTerapeutas(IdTerapeuta);
GO

/* 
ERROR AL CREAR DOS ID DIFERENTES, CORREGIDO A CONTINUACIÓN
*/
USE SistemasCitasSpa;
GO

EXEC sp_rename 'dbo.Citas.ClienteId', 'IdCliente', 'COLUMN';
EXEC sp_rename 'dbo.Citas.ServicioId', 'IdServicio', 'COLUMN';
EXEC sp_rename 'dbo.Citas.TerapeutaId', 'IdTerapeuta', 'COLUMN';
EXEC sp_rename 'dbo.Citas.SalaId', 'IdSala', 'COLUMN';
EXEC sp_rename 'dbo.Citas.MetodoPagoId', 'IdMetodoPago', 'COLUMN';
GO
