CREATE OR ALTER PROCEDURE sp_ObtenerAgendaDia
    @MedicoId INT,
    @Fecha DATE
AS
BEGIN
    SELECT c.CitaId,
           c.Fecha,
           c.Hora,
           c.Motivo,
           p.Nombre AS Paciente,
           c.Estado
    FROM Citas c
    JOIN Pacientes p ON c.PacienteId = p.PacienteId
    WHERE c.MedicoId = @MedicoId
      AND CAST(c.Fecha AS DATE) = @Fecha
    ORDER BY c.Hora ASC
END
GO