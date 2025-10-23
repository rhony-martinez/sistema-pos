using SistemaPOS.Domain.Entities;

public interface ISedeRepository
{
    Task<IEnumerable<Sede>> GetAllAsync();
    Task<Sede?> GetByIdAsync(int id);
    Task AddAsync(Sede sede);
    Task UpdateAsync(Sede sede);
    Task DeleteAsync(int id);
    Task<Sede?> BuscarSedeAsync(int? SEDE_ID, string? SEDE_NOMBRE);
    Task<IEnumerable<Sede>> ListarAsync();
    Task<bool> ExisteDuplicadaAsync(string nombre, string ciudad);
    Task<long> CrearAsync(Sede sede);


}
