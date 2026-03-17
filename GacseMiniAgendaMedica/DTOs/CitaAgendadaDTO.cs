namespace GacseMiniAgendaMedica.DTOs;

public class CitaAgendaDTO
{
    public int CitaId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Motivo { get; set; }
    public string Paciente { get; set; }
    public string Estado { get; set; }
}