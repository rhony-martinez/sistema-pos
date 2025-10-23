using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Domain.Repositories;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Infrastructure.Persistence;

namespace SistemaPOS.Infrastructure.Repositories
{
    public class SedeRepository : ISedeRepository
    {
        private readonly SistemaPosContext _context;

        public SedeRepository(SistemaPosContext context)
        {
            _context = context;
        }

        // ✅ Obtener todas las sedes
        public async Task<IEnumerable<Sede>> GetAllAsync()
        {
            return await _context.Sedes.ToListAsync();
        }

        // ✅ Obtener una sede por ID
        public async Task<Sede?> GetByIdAsync(int SEDE_ID)
        {
            return await _context.Sedes.FindAsync(SEDE_ID);
        }

        // ✅ Agregar una sede
        public async Task AddAsync(Sede sede)
        {
            await _context.Sedes.AddAsync(sede);
            await _context.SaveChangesAsync();
        }

        // ✅ Actualizar una sede
        public async Task UpdateAsync(Sede sede)
        {
            _context.Sedes.Update(sede);
            await _context.SaveChangesAsync();
        }

        // ✅ Eliminar una sede
        public async Task DeleteAsync(int id)
        {
            var sede = await _context.Sedes.FindAsync(id);
            if (sede != null)
            {
                _context.Sedes.Remove(sede);
                await _context.SaveChangesAsync();
            }
        }

       
        public async Task<Sede?> BuscarSedeAsync(int? SEDE_ID, string? SEDE_NOMBRE)
        {
            IQueryable<Sede> query = _context.Sedes.AsQueryable();

            if (SEDE_ID.HasValue)
            {
                Console.WriteLine($"🧩 FILTRANDO POR ID: {SEDE_ID}");
                query = query.Where(s => s.SedeId == SEDE_ID.Value);
            }

            if (!string.IsNullOrWhiteSpace(SEDE_NOMBRE))
            {
                Console.WriteLine($"🧩 FILTRANDO POR NOMBRE: {SEDE_NOMBRE}");
                query = query.Where(s => EF.Functions.Like(s.SedeNombre.ToUpper(), $"%{SEDE_NOMBRE.ToUpper()}%"));
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
