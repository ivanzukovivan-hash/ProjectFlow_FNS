using System;

namespace ProjectFlow_FNS.Models
{
    /// <summary>
    /// Модель отчета.
    /// </summary>
    public class Report
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Author { get; set; }
        public string ReportType { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}