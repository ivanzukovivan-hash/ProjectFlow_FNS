using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ProjectFlow_FNS.Models;

namespace ProjectFlow_FNS.Services
{
    /// <summary>
    /// Сервис работы с проектами.
    /// </summary>
    public class ProjectService : DatabaseManager
    {
        public List<Project> GetAll()
        {
            List<Project> projects = new List<Project>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = "SELECT id, name, description FROM projects";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        projects.Add(new Project
                        {
                            Id = r.GetInt32(0),
                            Name = r.GetString(1),
                            Description = r.IsDBNull(2) ? "" : r.GetString(2)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка загрузки проектов: " + ex.Message);
            }
            return projects;
        }
    }
}