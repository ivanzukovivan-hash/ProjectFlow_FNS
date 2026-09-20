using System;
using System.Windows;
using System.Windows.Controls;
using ProjectFlow_FNS.Models;
using ProjectFlow_FNS.Services;

namespace ProjectFlow_FNS.Views
{
    public partial class AddTaskWindow : Window
    {
        private readonly TaskService _taskService = new TaskService();
        private readonly ProjectService _projectService = new ProjectService();

        public AddTaskWindow()
        {
            InitializeComponent();
            CbProject.ItemsSource = _projectService.GetAll();
            CbProject.DisplayMemberPath = "Name";
            CbProject.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtTitle.Text) || CbProject.SelectedItem == null)
                {
                    MessageBox.Show("Заполните название и выберите проект!");
                    return;
                }

                var project = CbProject.SelectedItem as Project;
                var task = new TaskItem
                {
                    ProjectId = project.Id,
                    Title = TxtTitle.Text,
                    Description = TxtDescription.Text,
                    Priority = (CbPriority.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Средний",
                    Deadline = DpDeadline.SelectedDate,
                    AssigneeId = 3 // Временно инспектор
                };

                if (_taskService.Add(task))
                {
                    MessageBox.Show("Задача успешно создана!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}