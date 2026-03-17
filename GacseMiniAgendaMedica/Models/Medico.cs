namespace GacseMiniAgendaMedica.Models;

public class Medico
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public String Especialidad { get; set; }

    public List<HorarioMedico> HorariosMedico { get; set; } = new List<HorarioMedico>();

}
