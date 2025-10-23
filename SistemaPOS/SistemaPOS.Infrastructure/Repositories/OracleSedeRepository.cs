using Dapper;
using Oracle.ManagedDataAccess.Client;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Db;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SistemaPOS.Infrastructure.Repositories
{
    public class OracleSedeRepository : ISedeRepository
    {
        private readonly IOracleConnectionFactory _factory;
        public OracleSedeRepository(IOracleConnectionFactory factory) => _factory = factory;

        // Lista todas las sedes (Dapper)
        public async Task<IEnumerable<Sede>> GetAllAsync()
        {
            // reusa ListarAsync
            return await ListarAsync();
        }

        // Versión con el nombre "ListarAsync" (mantener compatibilidad)
        public async Task<IEnumerable<Sede>> ListarAsync()
        {
            const string sql = @"SELECT SEDE_ID, SEDE_NOMBRE, SEDE_DIRECCION, SEDE_CIUDAD,
                                        SEDE_DEPARTAMENTO, SEDE_UBICACION, SEDE_TELEFONO,
                                        SEDE_CORREO, SEDE_ESTADO
                                 FROM POS.SEDE ORDER BY SEDE_ID";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync(sql);

            var list = new List<Sede>();
            foreach (var r in rows)
            {
                var s = new Sede
                {
                    SedeId = Convert.ToInt32(r.SEDE_ID),
                    SedeNombre = (string)r.SEDE_NOMBRE,
                    SedeDireccion = (string?)r.SEDE_DIRECCION,
                    SedeCiudad = (string?)r.SEDE_CIUDAD,
                    SedeDepartamento = (string?)r.SEDE_DEPARTAMENTO,
                    SedeUbicacion = (string?)r.SEDE_UBICACION,
                    SedeTelefono = (string?)r.SEDE_TELEFONO,
                    SedeCorreo = (string?)r.SEDE_CORREO,
                    SedeEstado = (string?)r.SEDE_ESTADO
                };
                list.Add(s);
            }

            return list;
        }

        // Buscar por ID (reusa BuscarSedeAsync)
        public async Task<Sede?> GetByIdAsync(int id)
        {
            return await BuscarSedeAsync(id, null);
        }

        // Buscar por criterios
        public async Task<Sede?> BuscarSedeAsync(int? SEDE_ID, string? SEDE_NOMBRE)
        {
            var sql = @"SELECT SEDE_ID, SEDE_NOMBRE, SEDE_DIRECCION, SEDE_CIUDAD,
                               SEDE_DEPARTAMENTO, SEDE_UBICACION, SEDE_TELEFONO,
                               SEDE_CORREO, SEDE_ESTADO
                        FROM POS.SEDE
                        WHERE (:p_id IS NULL OR SEDE_ID = :p_id)
                          AND (:p_nombre IS NULL OR UPPER(TRIM(SEDE_NOMBRE)) LIKE '%' || UPPER(:p_nombre) || '%')";

            using var cn = _factory.Create();
            var r = await cn.QueryFirstOrDefaultAsync(sql, new { p_id = SEDE_ID, p_nombre = SEDE_NOMBRE });

            if (r == null) return null;

            return new Sede
            {
                SedeId = Convert.ToInt32(r.SEDE_ID),
                SedeNombre = (string)r.SEDE_NOMBRE,
                SedeDireccion = (string?)r.SEDE_DIRECCION,
                SedeCiudad = (string?)r.SEDE_CIUDAD,
                SedeDepartamento = (string?)r.SEDE_DEPARTAMENTO,
                SedeUbicacion = (string?)r.SEDE_UBICACION,
                SedeTelefono = (string?)r.SEDE_TELEFONO,
                SedeCorreo = (string?)r.SEDE_CORREO,
                SedeEstado = (string?)r.SEDE_ESTADO
            };
        }

        // Agregar (interfaz AddAsync) -> se apoya en CrearAsync
        public async Task AddAsync(Sede sede)
        {
            // CrearAsync devuelve id; asignarlo a sede.SedeId (si tu entidad usa int)
            var newId = await CrearAsync(sede);
            // si SedeId es int en la entidad, convertir:
            sede.SedeId = (int)newId;
        }

        // Crear (ya existente) - retorna id
        public async Task<long> CrearAsync(Sede s)
        {
            try
            {
                const string sql = @"
            INSERT INTO POS.SEDE
              (SEDE_NOMBRE, SEDE_DIRECCION, SEDE_CIUDAD, SEDE_DEPARTAMENTO, SEDE_UBICACION,
               SEDE_TELEFONO, SEDE_CORREO, SEDE_ESTADO)
            VALUES (:nombre,:dir,:ciudad,:depto,:ub,:tel,:correo,:estado)
            RETURNING SEDE_ID INTO :newId";

                using var cn = (OracleConnection)_factory.Create();
                await cn.OpenAsync();

                using var cmd = new OracleCommand(sql, cn) { BindByName = true };
                cmd.Parameters.Add(":nombre", s.SedeNombre);
                cmd.Parameters.Add(":dir", s.SedeDireccion);
                cmd.Parameters.Add(":ciudad", s.SedeCiudad);
                cmd.Parameters.Add(":depto", s.SedeDepartamento);
                cmd.Parameters.Add(":ub", s.SedeUbicacion);
                cmd.Parameters.Add(":tel", s.SedeTelefono);
                cmd.Parameters.Add(":correo", s.SedeCorreo);
                cmd.Parameters.Add(":estado", s.SedeEstado);

                var outId = new OracleParameter(":newId", OracleDbType.Int64)
                { Direction = System.Data.ParameterDirection.Output };
                cmd.Parameters.Add(outId);

                await cmd.ExecuteNonQueryAsync();
                return Convert.ToInt64(outId.Value.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 ERROR Oracle al crear sede: {ex.Message}");
                throw; // vuelve a lanzarlo para que el controlador lo capture
            }
        }


        // Actualizar
        public async Task UpdateAsync(Sede sede)
        {
            const string sql = @"
                UPDATE POS.SEDE
                SET SEDE_NOMBRE = :nombre,
                    SEDE_DIRECCION = :dir,
                    SEDE_CIUDAD = :ciudad,
                    SEDE_DEPARTAMENTO = :depto,
                    SEDE_UBICACION = :ub,
                    SEDE_TELEFONO = :tel,
                    SEDE_CORREO = :correo,
                    SEDE_ESTADO = :estado
                WHERE SEDE_ID = :id";

            using var cn = (OracleConnection)_factory.Create();
            await cn.OpenAsync();
            using var cmd = new OracleCommand(sql, cn) { BindByName = true };
            cmd.Parameters.Add(":nombre", sede.SedeNombre ?? string.Empty);
            cmd.Parameters.Add(":dir", sede.SedeDireccion ?? (object)DBNull.Value);
            cmd.Parameters.Add(":ciudad", sede.SedeCiudad ?? (object)DBNull.Value);
            cmd.Parameters.Add(":depto", sede.SedeDepartamento ?? (object)DBNull.Value);
            cmd.Parameters.Add(":ub", sede.SedeUbicacion ?? (object)DBNull.Value);
            cmd.Parameters.Add(":tel", sede.SedeTelefono ?? (object)DBNull.Value);
            cmd.Parameters.Add(":correo", sede.SedeCorreo ?? (object)DBNull.Value);
            cmd.Parameters.Add(":estado", sede.SedeEstado ?? "ACTIVA");
            cmd.Parameters.Add(":id", sede.SedeId);

            await cmd.ExecuteNonQueryAsync();
        }

        // Eliminar
        public async Task DeleteAsync(int id)
        {
            const string sql = @"DELETE FROM POS.SEDE WHERE SEDE_ID = :id";
            using var cn = (OracleConnection)_factory.Create();
            await cn.OpenAsync();
            using var cmd = new OracleCommand(sql, cn) { BindByName = true };
            cmd.Parameters.Add(":id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // Verifica duplicado
        public async Task<bool> ExisteDuplicadaAsync(string nombre, string ciudad)
        {
            const string sql = @"SELECT COUNT(1) FROM POS.SEDE
                                 WHERE UPPER(TRIM(SEDE_NOMBRE)) = UPPER(:p1)
                                   AND UPPER(TRIM(SEDE_CIUDAD))  = UPPER(:p2)";
            using var cn = _factory.Create();
            var count = await cn.ExecuteScalarAsync<int>(sql, new { p1 = nombre, p2 = ciudad });
            return count > 0;
        }
    }
}
