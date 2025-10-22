namespace SistemaPOS.Application.Queries.Sedes
{
    public interface ISedeQueries
    {
        Task<IReadOnlyList<SedeDto>> GetAllAsync();
    }
}
