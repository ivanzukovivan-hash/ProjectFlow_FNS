using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using ProjectFlow_FNS.Models;

namespace ProjectFlow_FNS.Services
{
    /// <summary>
    /// Сервис формирования отчетов (ReportModule).
    /// </summary>
    public class ReportService : DatabaseManager
    {
        /// <summary>
        /// Получение задач по проекту для отчета.
        /// </summary>
        public List<TaskItem> GetTasksByProject(int projectId)
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"SELECT t.id, t.title, t.status, t.priority, 
                                            ISNULL(u.full_name, 'Не назначен'), t.deadline
                                     FROM tasks t
                                     LEFT JOIN users u ON t.assignee_id = u.id
                                     WHERE t.project_id=@pid";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@pid", projectId);
                    conn.Open();
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = r.GetInt32(0),
                            Title = r.GetString(1),
                            Status = r.GetString(2),
                            Priority = r.GetString(3),
                            Assignee = r.GetString(4),
                            Deadline = r.IsDBNull(5) ? (DateTime?)null : r.GetDateTime(5)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения задач: " + ex.Message);
            }
            return tasks;
        }

        /// <summary>
        /// Экспорт отчета в CSV (аналог Excel).
        /// </summary>
        public string ExportToCsv(int projectId, string projectName)
        {
            try
            {
                var tasks = GetTasksByProject(projectId);
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                           $"Report_{projectName}_{DateTime.Now:yyyyMMdd_HHmm}.csv");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ID;Задача;Статус;Приоритет;Исполнитель;Срок");
                foreach (var t in tasks)
                {
                    sb.AppendLine($"{t.Id};{t.Title};{t.Status};{t.Priority};{t.Assignee};{t.Deadline?.ToString("dd.MM.yyyy") ?? "-"}");
                }
                File.WriteAllText(path, sb.ToString(), Encoding.GetEncoding(1251));

                // Сохраняем запись в БД
                SaveReportRecord(projectId, "CSV-экспорт", path);
                return path;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка экспорта: " + ex.Message);
            }
        }

        private void SaveReportRecord(int projectId, string type, string path)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = @"INSERT INTO reports (project_id, author_id, report_type, file_path)
                                 VALUES (@pid, 2, @t, @p)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pid", projectId);
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@p", path);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}