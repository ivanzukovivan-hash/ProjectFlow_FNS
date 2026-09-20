using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ProjectFlow_FNS.Models;

namespace ProjectFlow_FNS.Services
{
    /// <summary>
    /// Сервис управления задачами (TaskModule).
    /// Реализует CRUD-операции и изменение статусов.
    /// </summary>
    public class TaskService : DatabaseManager
    {
        public List<TaskItem> GetAll()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"SELECT t.id, t.project_id, p.name, t.title, t.status, 
                                            t.priority, ISNULL(u.full_name, 'Не назначен'), 
                                            t.assignee_id, t.deadline, t.created_at
                                     FROM tasks t
                                     LEFT JOIN projects p ON t.project_id = p.id
                                     LEFT JOIN users u ON t.assignee_id = u.id
                                     ORDER BY t.created_at DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = r.GetInt32(0),
                            ProjectId = r.GetInt32(1),
                            ProjectName = r.IsDBNull(2) ? "" : r.GetString(2),
                            Title = r.GetString(3),
                            Status = r.GetString(4),
                            Priority = r.GetString(5),
                            Assignee = r.GetString(6),
                            AssigneeId = r.IsDBNull(7) ? (int?)null : r.GetInt32(7),
                            Deadline = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8),
                            CreatedAt = r.GetDateTime(9)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка загрузки задач: " + ex.Message);
            }
            return tasks;
        }

        public bool Add(TaskItem task)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"INSERT INTO tasks (project_id, title, description, status, priority, assignee_id, creator_id, deadline)
                                     VALUES (@pid, @t, @d, 'Новая', @pr, @a, @c, @dl)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@pid", task.ProjectId);
                    cmd.Parameters.AddWithValue("@t", task.Title);
                    cmd.Parameters.AddWithValue("@d", (object)task.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pr", task.Priority);
                    cmd.Parameters.AddWithValue("@a", task.AssigneeId.HasValue ? (object)task.AssigneeId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@c", 2); // Временно менеджер
                    cmd.Parameters.AddWithValue("@dl", task.Deadline.HasValue ? (object)task.Deadline.Value : DBNull.Value);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка добавления задачи: " + ex.Message);
            }
        }

        public bool UpdateStatus(int taskId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = "UPDATE tasks SET status=@s, updated_at=GETDATE() WHERE id=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@s", newStatus);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка обновления статуса: " + ex.Message);
            }
        }

        public bool Delete(int taskId)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = "DELETE FROM tasks WHERE id=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка удаления задачи: " + ex.Message);
            }
        }
    }
}