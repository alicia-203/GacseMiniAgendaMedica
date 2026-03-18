using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;

namespace GacseMiniAgendaMedica.Data.Repositories;

public class EspecialidadRepository : IEspecialidadRepository
{
    private readonly AppDbContext _context;

    public EspecialidadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Especialidad>> ObtenerTodasAsync()
    {
        return await _context.Especialidades.ToListAsync();
    }

    public async Task<Especialidad?> ObtenerPorIdAsync(int id)
    {
        return await _context.Especialidades.FindAsync(id);
    }
}
