using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Services;
using Microsoft.AspNetCore.Mvc;
using GacseMiniAgendaMedica.Data.Repositories;

namespace GacseMiniAgendaMedica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;
    private readonly ICitaRepository _citaRepo;

    public CitasController(ICitaService citaService, ICitaRepository citaRepo)
    {
        _citaService = citaService;
        _citaRepo = citaRepo;
    }

    // POST: api/Citas/agendar
    [HttpPost("agendar")]
    public async Task<IActionResult> Agendar([FromBody] AgendarCitaDTO dto)
    {
        if (dto == null)
            return BadRequest(new { mensaje = "Datos de la cita inválidos" });

        var (exito, mensaje, alerta) = await _citaService.AgendarCita(dto);

        if (!exito)
        {
            return Conflict(new
            {
                mensaje
            });
        }

        return Ok(new { mensaje, alerta });
    }

    // PUT: api/Citas/cancelar/{id}
    [HttpPut("cancelar/{id}")]
    public async Task<IActionResult> Cancelar(int id, [FromBody] string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            return BadRequest(new { mensaje = "Debe especificar el motivo de cancelación" });

        var (exito, mensaje) = await _citaService.CancelarCita(id, motivo);

        if (!exito)
            return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }

    // GET: api/Citas/agenda-dia?medicoId=1&fecha=2026-03-16
    [HttpGet("agenda-dia")]
    public async Task<IActionResult> AgendaDelDia([FromQuery] int medicoId, [FromQuery] DateTime fecha)
    {
        var agenda = await _citaRepo.AgendaDelDia(medicoId, fecha);

        if (agenda == null || agenda.Count == 0)
            return NotFound(new { mensaje = "No hay citas para este médico en la fecha indicada" });

        return Ok(agenda);
    }

    // GET: api/Citas/historial-paciente/{pacienteId}
    [HttpGet("historial-paciente/{pacienteId}")]
    public async Task<IActionResult> HistorialPaciente(int pacienteId)
    {
        var historial = await _citaRepo.ObtenerHistorialPaciente(pacienteId);

        if (historial == null || historial.Count == 0)
            return NotFound(new { mensaje = "No se encontraron citas para este paciente" });

        var result = historial.Select(c => new
        {
            c.Id,
            c.Fecha,
            c.Hora,
            c.Estado,
            c.Motivo,
            Medico = c.Medico?.Nombre
        });

        return Ok(result);
    }

    // GET: api/Citas/horarios-disponibles?medicoId=1&fecha=2026-03-16
    [HttpGet("horarios-disponibles")]
    public async Task<IActionResult> HorariosDisponibles([FromQuery] int medicoId, [FromQuery] DateTime fecha)
    {
        var horarios = await _citaService.ObtenerProximosHorariosDisponibles(medicoId, fecha, 5);

        if (horarios == null || horarios.Count == 0)
            return NotFound(new { mensaje = "No hay horarios disponibles para este médico en la fecha indicada" });

        return Ok(horarios);
    }
}