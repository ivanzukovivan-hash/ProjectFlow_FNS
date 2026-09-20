namespace ProjectFlow_FNS.Models
{
    /// <summary>
    /// Модель пользователя системы ProjectFlow.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string SecurityQuestion { get; set; }
    }
}
