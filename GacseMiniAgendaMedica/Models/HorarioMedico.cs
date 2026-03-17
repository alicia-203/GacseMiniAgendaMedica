using System.Globalization;

namespace GacseMiniAgendaMedica.Models;

public class HorarioMedico
{
    public int Id { get; set; }

    public int MedicoId { get; set; }

    public DayOfWeek DiaSemana { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public string DiaSemanaTexto
    {
        get
        {
            var cultura = new CultureInfo("es-ES");
            return cultura.DateTimeFormat.GetDayName(DiaSemana);
        }
    }
}
