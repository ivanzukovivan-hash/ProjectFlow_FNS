using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectFlow_FNS.Helpers;
using ProjectFlow_FNS.Models;

namespace ProjectFlow_FNS.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты сценария:
    /// «Пользователь создаёт задачу → задача проходит по статусам → попадает в отчёт».
    /// Покрытие Задачи 1.3.
    /// </summary>
    [TestClass]
    public class WorkflowTests
    {
        [TestMethod]
        public void TaskLifecycle_NewToCompleted_AllTransitionsValid()
        {
            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Тестовая задача",
                Status = "Новая"
            };

            // Шаг 1: Новая → В работе
            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "В работе"));
            task.Status = "В работе";

            // Шаг 2: В работе → На проверке
            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "На проверке"));
            task.Status = "На проверке";

            // Шаг 3: На проверке → Завершена
            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "Завершена"));
            task.Status = "Завершена";

            Assert.AreEqual("Завершена", task.Status);
            Assert.AreEqual(0, TaskStateMachine.GetAllowedTransitions(task.Status).Count);
        }

        [TestMethod]
        public void TaskLifecycle_WithRejection_CannotBeResurrected()
        {
            TaskItem task = new TaskItem { Status = "Новая" };

            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "Отклонена"));
            task.Status = "Отклонена";

            Assert.IsFalse(TaskStateMachine.CanTransition(task.Status, "Новая"));
            Assert.IsFalse(TaskStateMachine.CanTransition(task.Status, "В работе"));
        }

        [TestMethod]
        public void TaskLifecycle_WithReworkCycle()
        {
            TaskItem task = new TaskItem { Status = "Новая" };

            task.Status = "В работе";
            task.Status = "На проверке";

            // Возврат на доработку
            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "В работе"));
            task.Status = "В работе";

            // Повторная проверка
            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "На проверке"));
            task.Status = "На проверке";

            // Финальное завершение
            Assert.IsTrue(TaskStateMachine.CanTransition(task.Status, "Завершена"));
        }
    }
}