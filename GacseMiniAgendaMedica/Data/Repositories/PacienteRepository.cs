using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;

namespace GacseMiniAgendaMedica.Data.Repositories;
    public class PacienteRepository : IPacienteRepository
    {
        private readonly AppDbContext _context;

        public PacienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Paciente>> ObtenerTodos()
        {
            return await _context.Pacientes
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Paciente?> ObtenerPorId(int id)
        {
            return await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task Crear(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(Paciente paciente)
        {
            var existente = await _context.Pacientes.FindAsync(paciente.Id);
            if (existente != null)
            {
                existente.Nombre = paciente.Nombre;
                existente.FechaNacimiento = paciente.FechaNacimiento;
                existente.Telefono = paciente.Telefono;
                existente.Email = paciente.Email;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Eliminar(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
            }
        }
    }
