using UpworkERP.Core.Interfaces;

namespace UpworkERP.Application.Services.Interfaces;

/// <summary>
/// Generic service interface following Service pattern
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IService<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
