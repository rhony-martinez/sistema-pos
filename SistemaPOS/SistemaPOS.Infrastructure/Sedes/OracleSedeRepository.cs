using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Oracle.ManagedDataAccess.Client;
using SistemaPOS.Application.Sedes;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Db;

namespace SistemaPOS.Infrastructure.Sedes
{
    public class OracleSedeRepository : ISedeRepository
    {
        private readonly IOracleConnectionFactory _factory;
        public OracleSedeRepository(IOracleConnectionFactory factory) => _factory = factory;

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
                var s = new Sede(
                    (string)r.SEDE_NOMBRE,
                    (string?)r.SEDE_DIRECCION,
                    (string?)r.SEDE_CIUDAD,
                    (string?)r.SEDE_DEPARTAMENTO,
                    (string?)r.SEDE_UBICACION,
                    (string?)r.SEDE_TELEFONO,
                    (string?)r.SEDE_CORREO,
                    (string?)r.SEDE_ESTADO
                );
                s.SetId(Convert.ToInt64(r.SEDE_ID));
                list.Add(s);
            }
            return list;
        }

        public async Task<bool> ExisteDuplicadaAsync(string nombre, string ciudad)
        {
            const string sql = @"SELECT COUNT(1) FROM POS.SEDE
                                 WHERE UPPER(TRIM(SEDE_NOMBRE)) = UPPER(:p1)
                                   AND UPPER(TRIM(SEDE_CIUDAD))  = UPPER(:p2)";
            using var cn = _factory.Create();
            var count = await cn.ExecuteScalarAsync<int>(sql, new { p1 = nombre, p2 = ciudad });
            return count > 0;
        }

        public async Task<long> CrearAsync(Sede s)
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
    }
}