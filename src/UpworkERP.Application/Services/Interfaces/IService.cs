namespace UpworkERP.Application.Services.Interfaces;

/// <summary>
/// Generic service interface for CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
