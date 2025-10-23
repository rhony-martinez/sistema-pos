using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace SistemaPOS.Infrastructure.Db
{
    public interface IOracleConnectionFactory
    {
        IDbConnection Create();
    }

    public class OracleConnectionFactory : IOracleConnectionFactory
    {
        private readonly string _connString;
        public OracleConnectionFactory(string connString) => _connString = connString;
        public IDbConnection Create() => new OracleConnection(_connString);
    }
}

