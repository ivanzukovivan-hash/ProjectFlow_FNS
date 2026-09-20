using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ProjectFlow_FNS.Services
{
    /// <summary>
    /// Базовый класс для работы с БД.
    /// Обеспечивает единый механизм подключения.
    /// </summary>
    public class DatabaseManager
    {
        protected string ConnectionString;

        public DatabaseManager()
        {
            try
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["FNS_DB"].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка чтения строки подключения: " + ex.Message);
            }
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}