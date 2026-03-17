using GacseMiniAgendaMedica.Models;

namespace GacseMiniAgendaMedica.Data.Repositories;

    public interface IPacienteRepository
    {
        Task<List<Paciente>> ObtenerTodos();

        Task<Paciente?> ObtenerPorId(int id);

        Task Crear(Paciente paciente);

        Task Actualizar(Paciente paciente);

        Task Eliminar(int id);
    }
