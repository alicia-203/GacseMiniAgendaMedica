BEGIN TRANSACTION;
-- Médicos
INSERT INTO Medicos (Nombre, Especialidad) VALUES
('Dr. Juan Pérez', 1), --Medicina General
('Dra. María García', 2); --Cardiología

-- Horarios
INSERT INTO HorariosMedicos (MedicoId, DiaSemana, HoraInicio, HoraFin) VALUES
(1, 1, '08:00', '13:00'), -- Lunes
(1, 2, '09:00', '14:00'), -- Martes
(2, 1, '11:00', '18:00'); -- Lunes

-- Pacientes
INSERT INTO Pacientes (Nombre, FechaNacimiento, Telefono, Email) VALUES
('Luis Sánchez', '1985-06-12', '555123456', 'luis@mail.com'),
('Carlos Martínez', '2000-02-09', '555567891', 'carlos@mail.com');

INSERT INTO Especialidades (Nombre, Duracion)
VALUES 
('Medicina General', 20),
('Cardiología', 30),
('Cirugía', 45),
('Pediatría', 20),
('Ginecología', 30),
('Traumatologia', 35),
('Dermatología', 30);

COMMIT TRANSACTION;