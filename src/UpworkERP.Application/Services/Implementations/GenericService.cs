using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Infrastructure.Data;
using UpworkERP.Infrastructure.Repositories;

namespace UpworkERP.Application.Services.Implementations;

/// <summary>
/// Generic service implementation
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class GenericService<T> : IService<T> where T : class
{
    private readonly IRepository<T> _repository;

    public GenericService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        return await _repository.AddAsync(entity);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        return await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
