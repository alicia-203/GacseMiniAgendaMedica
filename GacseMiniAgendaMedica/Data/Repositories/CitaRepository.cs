using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GacseMiniAgendaMedica.Data.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly AppDbContext _context;

    public CitaRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<bool> ExisteConflicto(int medicoId, DateTime fecha, TimeSpan hora, int duracion)
    {
        var result = (await _context.Set<ExisteConflictoDTO>()
            .FromSqlRaw("EXEC sp_ValidarDisponibilidad @MedicoId, @Fecha, @Hora, @Duracion",
                new SqlParameter("@MedicoId", medicoId),
                new SqlParameter("@Fecha", fecha.Date),
                new SqlParameter("@Hora", hora),
                new SqlParameter("@Duracion", duracion))
            .AsNoTracking()
            .ToListAsync()) 
            .FirstOrDefault(); 

        var value = result?.Resultado ?? 0;

        return value != 1;
    }

    public async Task<List<CitaAgendaDTO>> AgendaDelDia(int medicoId, DateTime fecha)
    {
        var parameters = new[]
        {
                new SqlParameter("@MedicoId", medicoId),
                new SqlParameter("@Fecha", fecha.Date)
            };

        var citas = await _context.CitasAgenda
        .FromSqlRaw("EXEC sp_ObtenerAgendaDia @MedicoId, @Fecha", parameters)
        .ToListAsync();

        return citas;
    }

    public async Task<int> CancelacionesPaciente(int pacienteId)
    {
        var fechaLimite = DateTime.Now.AddDays(-30);
        return await _context.Citas
            .CountAsync(c => c.PacienteId == pacienteId &&
                             c.Estado == "Cancelada" &&
                             c.Fecha >= fechaLimite.Date);
    }

    public async Task Agendar(Cita cita)
    {
        if (cita == null)
            throw new ArgumentNullException(nameof(cita));

        if (cita.MedicoId <= 0)
            throw new ArgumentException("MedicoId inválido");

        if (cita.PacienteId <= 0)
            throw new ArgumentException("PacienteId inválido");

        if (cita.Fecha == default)
            throw new ArgumentException("Fecha inválida");

        if (string.IsNullOrWhiteSpace(cita.Motivo))
            cita.Motivo = "No especificado"; // o lanzar excepción

        _context.Citas.Add(cita);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Esto te da el error real de SQL Server
            Console.WriteLine(ex.InnerException?.Message);
            throw;
        }
    }

    public async Task<Cita?> ObtenerPorId(int id)
    {
        return await _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task Cancelar(Cita cita)
    {
        cita.Estado = "Cancelada";
        await _context.SaveChangesAsync();
    }

    public async Task<List<Cita>> ObtenerHistorialPaciente(int pacienteId)
    {
        return await _context.Citas
            .Include(c => c.Medico)
            .Where(c => c.PacienteId == pacienteId)
            .OrderBy(c => c.Fecha).ThenBy(c => c.Hora)
            .ToListAsync();
    }

    public async Task<HorarioMedico?> ObtenerHorarioDelDia(int medicoId, DayOfWeek diaSemana)
    {
        return await _context.HorariosMedicos
            .FirstOrDefaultAsync(h => h.MedicoId == medicoId && h.DiaSemana == diaSemana);
    }

    public async Task<string> ObtenerEspecialidadMedico(int medicoId)
    {
        return await _context.Medicos
        .Where(m => m.Id == medicoId)
        .Select(m => m.Especialidad.Nombre)
        .FirstOrDefaultAsync();
    }
}
