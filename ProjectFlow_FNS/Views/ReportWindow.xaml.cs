using System.Linq;
using System.Windows;
using ProjectFlow_FNS.Models;
using ProjectFlow_FNS.Services;

namespace ProjectFlow_FNS.Views
{
    public partial class ReportWindow : Window
    {
        public ReportWindow(Project project)
        {
            InitializeComponent();
            TxtTitle.Text = $"Сводный отчёт: {project.Name}";
            var service = new ReportService();
            var tasks = service.GetTasksByProject(project.Id);
            DgReport.ItemsSource = tasks;
            int total = tasks.Count;
            int done = tasks.Count(t => t.Status == "Завершена");
            int inProgress = tasks.Count(t => t.Status == "В работе");
            TxtSummary.Text = $"Всего: {total} | Завершено: {done} | В работе: {inProgress}";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}