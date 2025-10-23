using SistemaPOS.Domain.Entities;

public interface ISedeRepository
{
    Task<IEnumerable<Sede>> GetAllAsync();
    Task<Sede?> GetByIdAsync(int id);
    Task AddAsync(Sede sede);
    Task UpdateAsync(Sede sede);
    Task DeleteAsync(int id);
}
