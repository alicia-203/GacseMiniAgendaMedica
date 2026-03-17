using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;

namespace GacseMiniAgendaMedica.Data.Repositories;

public class MedicoRepository
{
    private readonly AppDbContext _context;

    public MedicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Medico>> GetAllAsync()
    {
        return await _context.Medicos.Include(m => m.HorariosMedico).ToListAsync();
    }

    public async Task<Medico> GetByIdAsync(int id)
    {
        return await _context.Medicos.Include(m => m.HorariosMedico)
                                     .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Medico> CreateAsync(Medico medico)
    {
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();
        return medico;
    }

    public async Task<bool> UpdateAsync(Medico medico)
    {
        var existing = await _context.Medicos
            .Include(m => m.HorariosMedico) // ⚠️ Incluir horarios
            .FirstOrDefaultAsync(m => m.Id == medico.Id);

        if (existing == null) return false;

        // Actualiza medico
        existing.Nombre = medico.Nombre;
        existing.Especialidad = medico.Especialidad;

     //Modifica horarios
        // Elimina horarios
        var horariosToRemove = existing.HorariosMedico
            .Where(h => !medico.HorariosMedico.Any(mh => mh.Id == h.Id))
            .ToList();

        _context.HorariosMedicos.RemoveRange(horariosToRemove);

        // modifica o agrega horarios nuevos
        foreach (var h in medico.HorariosMedico)
        {
            var existingHorario = existing.HorariosMedico.FirstOrDefault(eh => eh.Id == h.Id);
            if (existingHorario != null)
            {
                // modifica horario existente
                existingHorario.DiaSemana = h.DiaSemana;
                existingHorario.HoraInicio = h.HoraInicio;
                existingHorario.HoraFin = h.HoraFin;
            }
            else
            {
                // Agrega nuevo horario
                existing.HorariosMedico.Add(new HorarioMedico
                {
                    DiaSemana = h.DiaSemana,
                    HoraInicio = h.HoraInicio,
                    HoraFin = h.HoraFin
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var medico = await _context.Medicos
            .Include(m => m.HorariosMedico)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medico == null) return false;

        _context.HorariosMedicos.RemoveRange(medico.HorariosMedico); // elimina los horarios primero
        _context.Medicos.Remove(medico);                             // luego elimina el médico

        await _context.SaveChangesAsync();
        return true;
    }
}