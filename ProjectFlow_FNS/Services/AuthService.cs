using System;
using System.Data.SqlClient;
using ProjectFlow_FNS.Models;
using ProjectFlow_FNS.Helpers;

namespace ProjectFlow_FNS.Services
{
    /// <summary>
    /// Сервис авторизации (AuthModule).
    /// Реализует вход, регистрацию, восстановление пароля.
    /// </summary>
    public class AuthService : DatabaseManager
    {
        /// <summary>
        /// Аутентификация пользователя по логину и паролю.
        /// </summary>
        public User Login(string username, string password)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"SELECT id, full_name, email, role, security_question 
                                     FROM users WHERE username=@u";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Проверка пароля (для простоты в учебном проекте)
                        // В production — сравнение хэшей
                        string storedHash = GetUserPasswordHash(username);
                        if (PasswordHasher.Verify(password, storedHash))
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Role = reader.GetString(3),
                                SecurityQuestion = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка авторизации: " + ex.Message);
            }
            return null;
        }

        private string GetUserPasswordHash(string username)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT password_hash FROM users WHERE username=@u";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                conn.Open();
                return cmd.ExecuteScalar()?.ToString() ?? "";
            }
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        public bool Register(string username, string password, string fullName, string email,
                     string role, string secQuestion, string secAnswer)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string query = @"INSERT INTO users (username, password_hash, full_name, email, role, security_question, security_answer_hash)
                             VALUES (@u, @p, @f, @e, @r, @sq, @sa)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(password));
                    cmd.Parameters.AddWithValue("@f", fullName);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@r", role);
                    cmd.Parameters.AddWithValue("@sq", secQuestion);
                    // ВАЖНО: хэшируем ответ БЕЗ ToLower()
                    cmd.Parameters.AddWithValue("@sa", PasswordHasher.Hash(secAnswer));
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка регистрации: " + ex.Message);
            }
        }

        /// <summary>
        /// Восстановление пароля через контрольный вопрос.
        /// </summary>
        public bool ResetPassword(string username, string answer, string newPassword)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Получаем хэш ответа из БД
                    string query = "SELECT security_answer_hash FROM users WHERE username=@u";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    conn.Open();
                    string storedAnswer = cmd.ExecuteScalar()?.ToString();

                    if (storedAnswer != null)
                    {
                        // Хэшируем введённый ответ (точно так же, как при регистрации!)
                        string inputHash = PasswordHasher.Hash(answer);

                        // Сравниваем хэши
                        if (inputHash == storedAnswer)
                        {
                            // Обновляем пароль
                            string update = "UPDATE users SET password_hash=@p WHERE username=@u";
                            SqlCommand cmdUpd = new SqlCommand(update, conn);
                            cmdUpd.Parameters.AddWithValue("@p", PasswordHasher.Hash(newPassword));
                            cmdUpd.Parameters.AddWithValue("@u", username);
                            return cmdUpd.ExecuteNonQuery() > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка сброса пароля: " + ex.Message);
            }
            return false;
        }

        public string GetSecurityQuestion(string username)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT security_question FROM users WHERE username=@u";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                conn.Open();
                return cmd.ExecuteScalar()?.ToString() ?? "";
            }
        }
    }
}