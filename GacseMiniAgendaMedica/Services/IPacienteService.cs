namespace GacseMiniAgendaMedica.Services;

using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

    public interface IPacienteService
    {
        Task<List<Paciente>> GetAllAsync();
        Task<Paciente> GetByIdAsync(int id);
        Task<Paciente> CreateAsync(CrearPacienteDTO dto);
        Task<Paciente> UpdateAsync(int id, Paciente paciente);
        Task<bool> DeleteAsync(int id);
    }
