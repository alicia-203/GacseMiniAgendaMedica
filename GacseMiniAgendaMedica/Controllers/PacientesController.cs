using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using GacseMiniAgendaMedica.Services;
using Microsoft.AspNetCore.Mvc;

namespace GacseMiniAgendaMedica.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pacientes = await _pacienteService.GetAllAsync();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var paciente = await _pacienteService.GetByIdAsync(id);
            if (paciente == null) return NotFound("Paciente no encontrado");
            return Ok(paciente);
        }

    [HttpPost]
    public async Task<IActionResult> Create(CrearPacienteDTO dto)
    {
        var nuevo = await _pacienteService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
    }

    [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Paciente paciente)
        {
            var actualizado = await _pacienteService.UpdateAsync(id, paciente);
            if (actualizado == null) return NotFound("Paciente no encontrado");
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _pacienteService.DeleteAsync(id);
            if (!eliminado) return NotFound("Paciente no encontrado");
            return NoContent();
        }
    }
