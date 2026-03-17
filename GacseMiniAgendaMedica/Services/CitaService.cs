using GacseMiniAgendaMedica.Data;
using GacseMiniAgendaMedica.Data.Repositories;
using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;

namespace GacseMiniAgendaMedica.Services;

    public class CitaService : ICitaService
    {
        private readonly AppDbContext _context;
        private readonly ICitaRepository _repo;

        public CitaService(AppDbContext context, ICitaRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        public async Task<(bool exito, string mensaje, bool alerta)> AgendarCita(AgendarCitaDTO dto)
        {
            var fechaHora = dto.Fecha.Date.Add(dto.Hora);
            if (fechaHora < DateTime.Now)
                return (false, "No se pueden agendar citas en el pasado", false);

            var medico = await _context.Medicos.FindAsync(dto.MedicoId);
            if (medico == null)
                return (false, "Médico no encontrado", false);

            int duracion = medico.Especialidad switch
            {
                "Medicina General" => 20,
                "Cardiología" => 30,
                "Cirugía" => 45,
                "Pediatría" => 20,
                "Ginecología" => 30,
                _ => 20
            };

            // Validar horario del médico
            var diaSemana = dto.Fecha.DayOfWeek;
            var horario = await _context.HorariosMedicos
                .FirstOrDefaultAsync(h => h.MedicoId == dto.MedicoId && h.DiaSemana == diaSemana);

            if (horario == null)
                return (false, "El médico no atiende ese día", false);

            var horaFinCita = dto.Hora.Add(TimeSpan.FromMinutes(duracion));
            if (dto.Hora < horario.HoraInicio || horaFinCita > horario.HoraFin)
                return (false, "La cita está fuera del horario del médico", false);

            // Validar conflictos usando SP
            var conflicto = await _repo.ExisteConflicto(dto.MedicoId, dto.Fecha, dto.Hora, duracion);
            if (conflicto)
                return (false, "Conflicto de horario con otra cita", false);

            // Alertar si paciente tiene >=3 cancelaciones
            var cancelaciones = await _repo.CancelacionesPaciente(dto.PacienteId);
            bool alerta = cancelaciones >= 3;

            var cita = new Cita
            {
                MedicoId = dto.MedicoId,
                PacienteId = dto.PacienteId,
                Fecha = dto.Fecha.Date,
                Hora = dto.Hora,
                Motivo = dto.Motivo,
                Estado = "Agendada"
            };

            await _repo.Agendar(cita);

            return (true, "Cita agendada correctamente", alerta);
        }


        public async Task<(bool exito, string mensaje)> CancelarCita(int citaId, string motivo)
        {
            var cita = await _repo.ObtenerPorId(citaId);
            if (cita == null)
                return (false, "Cita no encontrada");

            if (cita.Estado != "Agendada")
                return (false, "Solo se pueden cancelar citas agendadas");

            cita.Estado = "Cancelada";
            cita.MotivoCancelacion = motivo;

            await _repo.Cancelar(cita);

            return (true, "Cita cancelada correctamente");
        }

    public async Task<List<TimeSpan>> ObtenerProximosHorariosDisponibles(int medicoId, DateTime fecha, int cantidad)
    {
        // Traer médico junto con todos sus horarios
        var medico = await _context.Medicos
            .Include(m => m.HorariosMedico)
            .FirstOrDefaultAsync(m => m.Id == medicoId);

        if (medico == null)
            return new List<TimeSpan>();

        var diaSemana = fecha.DayOfWeek;

        // Obtener solo los horarios del día solicitado
        var horarioDelDia = medico.HorariosMedico
            .FirstOrDefault(h => h.DiaSemana == diaSemana);

        if (horarioDelDia == null)
            return new List<TimeSpan>(); // el médico no atiende ese día

        var citas = await _repo.AgendaDelDia(medicoId, fecha);

        // Duración según especialidad
        int duracion = medico.Especialidad switch
        {
            "Medicina General" => 20,
            "Cardiología" => 30,
            "Cirugía" => 45,
            "Pediatría" => 20,
            "Ginecología" => 30,
            _ => 20
        };

        var disponibles = new List<TimeSpan>();
        var horaActual = horarioDelDia.HoraInicio;

        //  horarios disponibles
        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horarioDelDia.HoraFin
               && disponibles.Count < cantidad)
        {
            bool conflicto = citas.Any(c =>
            {
                var inicio = c.Hora;
                var fin = inicio.Add(TimeSpan.FromMinutes(duracion));
                var nuevaFin = horaActual.Add(TimeSpan.FromMinutes(duracion));
                return horaActual < fin && nuevaFin > inicio;
            });

            if (!conflicto)
                disponibles.Add(horaActual);

            horaActual = horaActual.Add(TimeSpan.FromMinutes(5)); 
        }

        return disponibles;
    }

 
    }
