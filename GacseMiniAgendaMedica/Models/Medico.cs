namespace GacseMiniAgendaMedica.Models;

public class Medico
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int EspecialidadId { get; set; }

    public Especialidad Especialidad { get; set; } // navegación
    public List<HorarioMedico> HorariosMedico { get; set; } = new List<HorarioMedico>();

}
