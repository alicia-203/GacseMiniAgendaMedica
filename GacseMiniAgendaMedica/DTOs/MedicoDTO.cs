namespace GacseMiniAgendaMedica.DTOs;

public class MedicoDTO
{
    public string Nombre { get; set; }
    public string Especialidad { get; set; }

    public List<HorarioMedicoDTO> Horarios { get; set; } = new List<HorarioMedicoDTO>();
}
