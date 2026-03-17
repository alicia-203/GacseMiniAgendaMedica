namespace GacseMiniAgendaMedica.DTOs
{
    public class HorarioMedicoDTO
    {
        public int DiaSemana { get; set; }   // "1 = Lunes", "2 = Martes", etc.
        public string HoraInicio { get; set; }  
        public string HoraFin { get; set; }    
    }
}
