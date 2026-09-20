using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectFlow_FNS.Helpers;
using ProjectFlow_FNS.Models;
using ProjectFlow_FNS.Services;

namespace ProjectFlow_FNS.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly TaskService _taskService = new TaskService();
        private readonly ProjectService _projectService = new ProjectService();
        private readonly ReportService _reportService = new ReportService();
        private ObservableCollection<TaskItem> _tasks;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            TxtUserInfo.Text = $"{user.FullName} | {user.Role}";
            LoadTasks();
            LoadProjects();
        }

        private void LoadTasks()
        {
            try
            {
                _tasks = new ObservableCollection<TaskItem>(_taskService.GetAll());
                DgTasks.ItemsSource = _tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки задач: " + ex.Message);
            }
        }

        private void LoadProjects()
        {
            try
            {
                CbProject.ItemsSource = _projectService.GetAll();
                CbProject.DisplayMemberPath = "Name";
                CbProject.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки проектов: " + ex.Message);
            }
        }

        private void DgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxtStatusMsg.Text = "";
            if (DgTasks.SelectedItem is TaskItem task)
            {
                // Заполняем ComboBox только допустимыми переходами (математическая модель!)
                var allowed = TaskStateMachine.GetAllowedTransitions(task.Status);
                CbStatus.ItemsSource = allowed;
                CbStatus.SelectedIndex = allowed.Any() ? 0 : -1;
            }
        }

        private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (DgTasks.SelectedItem is TaskItem task && CbStatus.SelectedItem is string newStatus)
            {
                // Проверка соответствия математической модели (Задача 3.3)
                if (!TaskStateMachine.CanTransition(task.Status, newStatus))
                {
                    TxtStatusMsg.Text = $"⚠ Переход '{task.Status}' → '{newStatus}' запрещен моделью!";
                    return;
                }
                try
                {
                    if (_taskService.UpdateStatus(task.Id, newStatus))
                    {
                        task.Status = newStatus;
                        DgTasks.Items.Refresh();
                        TxtStatusMsg.Text = "";
                        TxtStatusMsg.Foreground = System.Windows.Media.Brushes.Green;
                        TxtStatusMsg.Text = "✓ Статус обновлён";
                    }
                }
                catch (Exception ex)
                {
                    TxtStatusMsg.Text = "Ошибка: " + ex.Message;
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу и новый статус!");
            }
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            new AddTaskWindow().ShowDialog();
            LoadTasks();
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (DgTasks.SelectedItem is TaskItem task)
            {
                var result = MessageBox.Show($"Удалить задачу '{task.Title}'?", "Подтверждение",
                                             MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _taskService.Delete(task.Id);
                        LoadTasks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для удаления!");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadTasks();

        private void BtnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            if (CbProject.SelectedItem is Project project)
            {
                try
                {
                    string path = _reportService.ExportToCsv(project.Id, project.Name);
                    TxtReportMsg.Text = $"✓ Отчёт сохранён: {path}";
                    TxtReportMsg.Foreground = System.Windows.Media.Brushes.Green;
                }
                catch (Exception ex)
                {
                    TxtReportMsg.Text = "Ошибка: " + ex.Message;
                    TxtReportMsg.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            else
            {
                MessageBox.Show("Выберите проект!");
            }
        }

        private void BtnSummaryReport_Click(object sender, RoutedEventArgs e)
        {
            if (CbProject.SelectedItem is Project project)
            {
                new ReportWindow(project).ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите проект!");
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}