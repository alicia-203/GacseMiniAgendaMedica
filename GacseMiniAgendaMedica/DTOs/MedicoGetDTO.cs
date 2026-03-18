using GacseMiniAgendaMedica.Models;

namespace GacseMiniAgendaMedica.DTOs;

public class MedicoGetDTO
{
    public string Nombre { get; set; }
    public int EspecialidadId { get; set; }
    public EspecialidadDTO Especialidad { get; set; } 
    public List<HorarioMedicoDTO> Horarios { get; set; } = new List<HorarioMedicoDTO>();
}
