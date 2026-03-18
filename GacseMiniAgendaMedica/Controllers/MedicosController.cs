using Microsoft.AspNetCore.Mvc;
using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using GacseMiniAgendaMedica.Services;

namespace GacseMiniAgendaMedica.Controllers;

[ApiController]
[Route("api/medicos")]
public class MedicosController : ControllerBase
{
    private readonly MedicoService _service;

    public MedicosController(MedicoService service)
    {
        _service = service;
    }

    [HttpGet] 
    public async Task<IActionResult> GetAll()
    {
        var medicos = await _service.GetAll();
        return Ok(medicos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var medico = await _service.GetById(id);
        if (medico == null) return NotFound("Médico no encontrado");
        return Ok(medico);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearMedicoDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.EspecialidadId <= 0)
            return BadRequest("Nombre y Especialidad son obligatorios");

        // Crear Medico
        var medico = new Medico
        {
            Nombre = dto.Nombre,
            EspecialidadId = dto.EspecialidadId,
            HorariosMedico = dto.Horarios.Select(h => new HorarioMedico
            {
                DiaSemana = (DayOfWeek)((h.DiaSemana % 7)), //  1 = lunes, 0 = domingo
                HoraInicio = TimeSpan.Parse(h.HoraInicio),
                HoraFin = TimeSpan.Parse(h.HoraFin)
            }).ToList()
        };

        var creado = await _service.Create(medico);

        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CrearMedicoDTO dto)
    {
        var medico = new Medico { Id = id, Nombre = dto.Nombre, EspecialidadId = dto.EspecialidadId };
        var actualizado = await _service.Update(medico);
        if (!actualizado) return NotFound("Médico no encontrado");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await _service.Delete(id);
        if (!eliminado) return NotFound("Médico no encontrado");
        return NoContent();
    }
}