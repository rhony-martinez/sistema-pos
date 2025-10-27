namespace SistemaPOS.Application.Services
{
    public interface ISedeService
    {
        Task<string> InactivarSedeAsync(int sedeId);
    }
}
