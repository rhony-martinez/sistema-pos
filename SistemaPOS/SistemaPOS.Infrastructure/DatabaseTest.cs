using System;
using Oracle.ManagedDataAccess.Client;

namespace SistemaPOS.Infrastructure
{
    public class DatabaseTest
    {
        public static void ProbarConexion()
        {
            string connectionString = "User Id=POS;Password=software1;Data Source=localhost:1521/xepdb1";

            try
            {
                using var connection = new OracleConnection(connectionString);
                connection.Open();
                Console.WriteLine("✅ Conexión exitosa a Oracle!");

                using var command = new OracleCommand("SELECT SEDE_ID, SEDE_NOMBRE FROM POS.SEDE", connection);
                using var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("📋 Sedes encontradas:");
                    while (reader.Read())
                    {
                        Console.WriteLine($" - {reader.GetInt32(0)} | {reader.GetString(1)}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ No se encontraron sedes en la tabla POS.SEDE.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al conectar con la base de datos:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
