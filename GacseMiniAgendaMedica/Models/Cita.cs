namespace GacseMiniAgendaMedica.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Cita
{
    public int Id { get; set; }

    public int MedicoId { get; set; }

    public int PacienteId { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan Hora { get; set; }

    public string Motivo { get; set; }

    public string Estado { get; set; } = "Agendada";

    public string MotivoCancelacion { get; set; }

    public Medico Medico { get; set; }

    public Paciente Paciente { get; set; }
}
