using GacseMiniAgendaMedica.Models;

namespace GacseMiniAgendaMedica.Data.Repositories;

public interface IEspecialidadRepository
{
    Task<List<Especialidad>> ObtenerTodasAsync();
    Task<Especialidad?> ObtenerPorIdAsync(int id);
}
