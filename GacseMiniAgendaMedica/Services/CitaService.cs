using GacseMiniAgendaMedica.Data;
using GacseMiniAgendaMedica.Data.Repositories;
using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;

namespace GacseMiniAgendaMedica.Services;

public class CitaService : ICitaService
{
    private readonly AppDbContext _context;
    private readonly ICitaRepository _citaRepository;

    public CitaService(AppDbContext context, ICitaRepository citaRepository)
    {
        _context = context;
        _citaRepository = citaRepository;
    }

    public async Task<(bool exito, string mensaje, bool alerta)> AgendarCita(AgendarCitaDTO dto)
    {
        var fechaHora = dto.Fecha.Date.Add(dto.Hora);
        if (fechaHora < DateTime.Now)
            return (false, "No se pueden agendar citas en el pasado", false);

        var medico = await _context.Medicos.FindAsync(dto.MedicoId);
        if (medico == null)
            return (false, "Médico no encontrado", false);

        int duracion = await _context.Especialidades //obtiene la duracion por especialidad del medico
            .Where(e => e.Id == medico.EspecialidadId)
            .Select(e => e.Duracion)
            .FirstOrDefaultAsync();

        // Validar horario del médico
        var diaSemana = dto.Fecha.DayOfWeek;
        var horario = await _context.HorariosMedicos
            .FirstOrDefaultAsync(h => h.MedicoId == dto.MedicoId && h.DiaSemana == diaSemana);

        if (horario == null)
            return (false, "El médico no atiende ese día", false);

        var horaFinCita = dto.Hora.Add(TimeSpan.FromMinutes(duracion));
        if (dto.Hora < horario.HoraInicio || horaFinCita > horario.HoraFin)
            return (false, "La cita está fuera del horario del médico", false);

        // Validar si la cita no se cruza con otra en SP
        var conflicto = await _citaRepository.ExisteConflicto(dto.MedicoId, dto.Fecha, dto.Hora, duracion);
        if (conflicto)
            return (false, "Conflicto de horario con otra cita", false);

        // Alertar si paciente tiene >=3 cancelaciones
        bool alerta = await _citaRepository.CancelacionesPaciente(dto.PacienteId) >= 3;

        var cita = new Cita
        {
            MedicoId = dto.MedicoId,
            PacienteId = dto.PacienteId,
            Fecha = dto.Fecha.Date,
            Hora = dto.Hora,
            Motivo = dto.Motivo,
            Estado = "Agendada"
        };

        await _citaRepository.Agendar(cita);

        return (true, "Cita agendada correctamente", alerta);
    }


    public async Task<(bool exito, string mensaje)> CancelarCita(int citaId, string motivo)
    {
        var cita = await _citaRepository.ObtenerPorId(citaId);
        if (cita == null)
            return (false, "Cita no encontrada");

        if (cita.Estado != "Agendada")
            return (false, "Solo se pueden cancelar citas agendadas");

        cita.Estado = "Cancelada";
        cita.MotivoCancelacion = motivo;

        await _citaRepository.Cancelar(cita);

        return (true, "Cita cancelada correctamente");
    }

    public async Task<List<TimeSpan>> ObtenerProximosHorariosDisponibles(int medicoId, DateTime fecha, int cantidad)
    {
        // Traer médico con todos sus horarios
        var medico = await _context.Medicos
            .Include(m => m.HorariosMedico)
            .FirstOrDefaultAsync(m => m.Id == medicoId);

        if (medico == null)
            return new List<TimeSpan>();

        var diaSemanaSolicitada = fecha.DayOfWeek;

        // Obtener los horarios del medico en el día solicitado
        var horariosDiaSolicitado = medico.HorariosMedico
            .FirstOrDefault(h => h.DiaSemana == diaSemanaSolicitada);

        if (horariosDiaSolicitado == null)
            return new List<TimeSpan>(); // el médico no atiende ese día

        //citas del medico en esa fecha
        var citasDelDia = await _citaRepository.AgendaDelDia(medicoId, fecha);

        //obtiene la duracion por especialidad del medico
        int duracion = await _context.Especialidades
            .Where(e => e.Id == medico.EspecialidadId)
            .Select(e => e.Duracion)
            .FirstOrDefaultAsync();

        var horariosDisponibles = new List<TimeSpan>();
        var hora = horariosDiaSolicitado.HoraInicio; //primer horario del medico en ese dia

        //  horarios disponibles
        // (hora inicio = primer horario del medico) la cita no puede superar la hora fin del medico
        while (hora <= horariosDiaSolicitado.HoraFin)
        {
            // Valida si el horario esta disponible con SP
            bool conflicto = await _citaRepository.ExisteConflicto(medicoId, fecha, hora, duracion);

            if (!conflicto)
                horariosDisponibles.Add(hora);

            hora = hora.Add(TimeSpan.FromMinutes(duracion));
        }

        return horariosDisponibles.Take(cantidad).ToList();
    }
}

 
    
