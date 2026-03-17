namespace GacseMiniAgendaMedica.Services;

using GacseMiniAgendaMedica.Data;
using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


    public class PacienteService : IPacienteService
    {
        private readonly AppDbContext _context;

        public PacienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Paciente>> GetAllAsync()
        {
            return await _context.Pacientes.ToListAsync();
        }

        public async Task<Paciente> GetByIdAsync(int id)
        {
            return await _context.Pacientes.FindAsync(id);
        }

    public async Task<Paciente> CreateAsync(CrearPacienteDTO dto)
    {
        var paciente = new Paciente
        {
            Nombre = dto.Nombre,
            FechaNacimiento = dto.FechaNacimiento,
            Telefono = dto.Telefono,
            Email = dto.Email
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return paciente;
    }

    public async Task<Paciente> UpdateAsync(int id, Paciente paciente)
        {
            var existing = await _context.Pacientes.FindAsync(id);
            if (existing == null)
                return null;

            existing.Nombre = paciente.Nombre;
            existing.FechaNacimiento = paciente.FechaNacimiento;
            existing.Telefono = paciente.Telefono;
            existing.Email = paciente.Email;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
                return false;

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
