namespace GacseMiniAgendaMedica.DTOs;

public class AgendarCitaDTO
{
    public int MedicoId { get; set; }
    public int PacienteId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Motivo { get; set; }
}
