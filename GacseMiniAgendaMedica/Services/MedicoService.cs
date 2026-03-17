using GacseMiniAgendaMedica.Data.Repositories;
using GacseMiniAgendaMedica.Models;

namespace GacseMiniAgendaMedica.Services;

public class MedicoService
{
    private readonly MedicoRepository _repository;

    public MedicoService(MedicoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Medico>> GetAll() => _repository.GetAllAsync();

    public Task<Medico> GetById(int id) => _repository.GetByIdAsync(id);

    public Task<Medico> Create(Medico medico) => _repository.CreateAsync(medico);

    public Task<bool> Update(Medico medico) => _repository.UpdateAsync(medico);

    public Task<bool> Delete(int id) => _repository.DeleteAsync(id);
}