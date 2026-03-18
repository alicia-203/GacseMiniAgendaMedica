--EXEC sp_ValidarDisponibilidad @MedicoId=3, @Fecha='2026-03-18 00:00', @Hora='13:00', @Duracion=30;
CREATE OR ALTER PROCEDURE sp_ValidarDisponibilidad
    @MedicoId INT,
    @Fecha DATE,
    @Hora TIME,
    @Duracion INT
AS
BEGIN
    DECLARE @InicioNuevaCita DATETIME = CAST(@Fecha AS DATETIME) + CAST(@Hora AS DATETIME);
    DECLARE @FinNuevaCita DATETIME = DATEADD(MINUTE, @Duracion, @InicioNuevaCita); --se agrega el tiempo de duracion de la especialidad

    IF EXISTS (
        SELECT 1 
        FROM Citas
        WHERE MedicoId = @MedicoId
          AND Estado = 'Agendada'
          AND (
                @InicioNuevaCita < DATEADD(MINUTE, @Duracion, CAST(Fecha AS DATETIME) + CAST(Hora AS DATETIME))  --si la nueva empieza antes que termine la actual
                AND @FinNuevaCita > (CAST(Fecha AS DATETIME) + CAST(Hora AS DATETIME)) -- y si termina despues de que empiece la actual
              )
    )
    BEGIN
        SELECT 0 AS Disponible; --No disponible
    END
    ELSE
    BEGIN
        SELECT 1 AS Disponible; --Disponible
    END
END
GO