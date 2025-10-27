namespace SistemaPOS.Application.Services.Implementations
{
    public class SedeService : ISedeService
    {
        private readonly ISedeRepository _repo;

        public SedeService(ISedeRepository repo)
        {
            _repo = repo;
        }

        public async Task<string> InactivarSedeAsync(int sedeId)
        {
            return await _repo.InactivarSedeAsync(sedeId);
        }
    }
}
