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
        using var conn = _context.Database.GetDbConnection();
        await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_ValidarDisponibilidad";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(new SqlParameter("@MedicoId", SqlDbType.Int) { Value = medicoId });
        cmd.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.Date) { Value = fecha.Date });
        cmd.Parameters.Add(new SqlParameter("@Hora", SqlDbType.Time) { Value = hora });
        cmd.Parameters.Add(new SqlParameter("@Duracion", SqlDbType.Int) { Value = duracion });

        var result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result) == 1;
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
        _context.Citas.Add(cita);
        await _context.SaveChangesAsync();
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
            .Select(m => m.Especialidad)
            .FirstOrDefaultAsync();
    }
}
