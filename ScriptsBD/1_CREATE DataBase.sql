CREATE DATABASE GacseMiniAgendaMedicaDB;
GO

USE GacseMiniAgendaMedicaDB;
GO

CREATE TABLE Especialidades (
    EspecialidadId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Duracion INT NOT NULL -- en minutos
);

CREATE TABLE Medicos (
    MedicoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    EspecialidadId INT NOT NULL
);

CREATE TABLE HorariosMedicos (
    HorarioId INT IDENTITY PRIMARY KEY,
    MedicoId INT NOT NULL FOREIGN KEY REFERENCES Medicos(MedicoId),
    DiaSemana TINYINT NOT NULL, -- 1=Lunes ... 7=Domingo
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL
);

CREATE TABLE Pacientes (
    PacienteId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono NVARCHAR(20),
    Email NVARCHAR(100)
);

CREATE TABLE Citas (
    CitaId INT IDENTITY PRIMARY KEY,
    MedicoId INT NOT NULL FOREIGN KEY REFERENCES Medicos(MedicoId),
    PacienteId INT NOT NULL FOREIGN KEY REFERENCES Pacientes(PacienteId),
    Fecha DATETIME NOT NULL,
    Hora TIME NOT NULL,
    Motivo NVARCHAR(200) NULL,
	Estado NVARCHAR(50)DEFAULT 'Agendada', -- Agendada / Cancelada
    MotivoCancelacion NVARCHAR(200) NULL
);