
using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;

namespace GacseMiniAgendaMedica.Data.Repositories;

public interface ICitaRepository
{
    Task<bool> ExisteConflicto(int medicoId, DateTime fecha, TimeSpan hora, int duracion);

    Task<int> CancelacionesPaciente(int pacienteId);

    Task Agendar(Cita cita);

    Task<Cita?> ObtenerPorId(int id);

    Task Cancelar(Cita cita);

    Task<List<CitaAgendaDTO>> AgendaDelDia(int medicoId, DateTime fecha);

    Task<List<Cita>> ObtenerHistorialPaciente(int pacienteId);

    Task<HorarioMedico?> ObtenerHorarioDelDia(int medicoId, DayOfWeek diaSemana);
    Task<string> ObtenerEspecialidadMedico(int medicoId);
}
