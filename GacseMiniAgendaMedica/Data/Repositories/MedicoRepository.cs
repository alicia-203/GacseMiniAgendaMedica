using GacseMiniAgendaMedica.DTOs;
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

    public async Task<List<MedicoGetDTO>> GetAllAsync()
    {
        return await _context.Medicos
            .Include(m => m.HorariosMedico)          // Incluye horarios
            .Include(m => m.Especialidad)            // Incluye especialidad
            .Select(m => new MedicoGetDTO
            {
                Nombre = m.Nombre,
                EspecialidadId = m.EspecialidadId,
                Especialidad = new EspecialidadDTO
                {
                    Id = m.Especialidad.Id,
                    Nombre = m.Especialidad.Nombre,
                    Duracion = m.Especialidad.Duracion
                },
                Horarios = m.HorariosMedico.Select(h => new HorarioMedicoDTO
                {
                    DiaSemana = (int)h.DiaSemana,
                    HoraInicio = h.HoraInicio.ToString(@"hh\:mm"), // si es TimeSpan
                    HoraFin = h.HoraFin.ToString(@"hh\:mm")
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<MedicoGetDTO> GetByIdAsync(int id)
    {
        return await _context.Medicos
               .Where(m => m.Id == id)               // Filtramos por Id
               .Include(m => m.HorariosMedico)            // Incluye horarios
               .Include(m => m.Especialidad)              // Incluye especialidad
               .Select(m => new MedicoGetDTO
               {
                   Nombre = m.Nombre,
                   EspecialidadId = m.EspecialidadId,
                   Especialidad = new EspecialidadDTO
                   {
                       Id = m.Especialidad.Id,
                       Nombre = m.Especialidad.Nombre,
                       Duracion = m.Especialidad.Duracion
                   },
                   Horarios = m.HorariosMedico.Select(h => new HorarioMedicoDTO
                   {
                       DiaSemana = (int)h.DiaSemana,
                       HoraInicio = h.HoraInicio.ToString(@"hh\:mm"),
                       HoraFin = h.HoraFin.ToString(@"hh\:mm")
                   }).ToList()
               })
               .FirstOrDefaultAsync();
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
            .Include(m => m.HorariosMedico) //  horarios
            .FirstOrDefaultAsync(m => m.Id == medico.Id);

        if (existing == null) return false;

        // Actualiza medico
        existing.Nombre = medico.Nombre;
        existing.EspecialidadId = medico.EspecialidadId;

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