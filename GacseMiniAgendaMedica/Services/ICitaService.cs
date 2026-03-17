using GacseMiniAgendaMedica.DTOs;

namespace GacseMiniAgendaMedica.Services;


    public interface ICitaService
    {
        Task<(bool exito, string mensaje, bool alerta)> AgendarCita(AgendarCitaDTO dto);

        Task<(bool exito, string mensaje)> CancelarCita(int citaId, string motivo);

        Task<List<TimeSpan>> ObtenerProximosHorariosDisponibles(int medicoId, DateTime fecha, int cantidad);

   
    }
