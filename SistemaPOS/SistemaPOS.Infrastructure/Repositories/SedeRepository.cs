using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Domain.Repositories;
using SistemaPOS.Infrastructure.Persistence;

namespace SistemaPOS.Infrastructure.Repositories
{
    public class SedeRepository : ISedeRepository
    {
        private readonly SistemaPOSDbContext _context;

        public SedeRepository(SistemaPOSDbContext context)
        {
            _context = context;
        }

        // ✅ Obtener todas las sedes
        public async Task<IEnumerable<Sede>> GetAllAsync()
        {
            return await _context.Sede.ToListAsync();
        }

        // ✅ Obtener una sede por ID
        public async Task<Sede?> GetByIdAsync(int SEDE_ID)
        {
            return await _context.Sede.FindAsync(SEDE_ID);
        }

        // ✅ Buscar una sede por ID o por nombre
        public async Task<Sede?> BuscarSedeAsync(int? SEDE_ID, string? SEDE_NOMBRE)
        {
            IQueryable<Sede> query = _context.Sede.AsQueryable();

            if (SEDE_ID.HasValue)
            {
                Console.WriteLine($"🧩 FILTRANDO POR ID: {SEDE_ID}");
                query = query.Where(s => s.SEDE_ID == SEDE_ID.Value);
            }

            if (!string.IsNullOrWhiteSpace(SEDE_NOMBRE))
            {
                Console.WriteLine($"🧩 FILTRANDO POR NOMBRE: {SEDE_NOMBRE}");
                query = query.Where(s => EF.Functions.Like(s.SEDE_NOMBRE.ToUpper(), $"%{SEDE_NOMBRE.ToUpper()}%"));
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
