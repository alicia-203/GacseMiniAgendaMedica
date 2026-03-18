namespace GacseMiniAgendaMedica.Models;

public class Especialidad
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Duracion { get; set; }
    public ICollection<Medico> Medicos { get; set; } // relación 
}
