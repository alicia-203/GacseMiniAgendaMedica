using System.Globalization;

namespace GacseMiniAgendaMedica.DTOs;

public class HorarioMedicoDTO
{
    public int DiaSemana { get; set; }   // "1 = Lunes", "2 = Martes", etc.
    public string DiaSemanaTxt
    {
        get
        {
            // 1 = lunes ... 7 = domingo
            //int diaIndex = (DiaSemana % 7); // Domingo = 0
            var cultura = new CultureInfo("es-ES");
            return cultura.DateTimeFormat.DayNames[DiaSemana];
        }
    }
    public string HoraInicio { get; set; }  
    public string HoraFin { get; set; }    
}
