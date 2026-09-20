namespace ProjectFlow_FNS.Models
{
    /// <summary>
    /// Модель проекта (налоговой проверки/аудита).
    /// </summary>
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}