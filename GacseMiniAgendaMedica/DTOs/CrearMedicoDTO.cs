using GacseMiniAgendaMedica.Models;

namespace GacseMiniAgendaMedica.DTOs;

public class CrearMedicoDTO
{

    public string Nombre { get; set; }

    public int EspecialidadId { get; set; }

    public List<HorarioMedicoDTO> Horarios { get; set; } = new List<HorarioMedicoDTO>();
}
