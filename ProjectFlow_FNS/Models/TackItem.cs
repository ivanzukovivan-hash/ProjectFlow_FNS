using System;

namespace ProjectFlow_FNS.Models
{
    /// <summary>
    /// Модель задачи в системе ProjectFlow.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Assignee { get; set; }
        public int? AssigneeId { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}