using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectFlow_FNS.Helpers;
using System.Collections.Generic;

namespace ProjectFlow_FNS.Tests.Helpers
{
    /// <summary>
    /// Тесты математической модели конечного автомата (State Machine).
    /// Покрывает требование Задачи 3.2: тестирование переходов состояний.
    /// </summary>
    [TestClass]
    public class TaskStateMachineTests
    {
        // ===== ДОПУСТИМЫЕ ПЕРЕХОДЫ (8 сценариев по заданию) =====

        [TestMethod]
        public void CanTransition_New_To_InProgress_ReturnsTrue()
        {
            Assert.IsTrue(TaskStateMachine.CanTransition("Новая", "В работе"));
        }

        [TestMethod]
        public void CanTransition_New_To_Rejected_ReturnsTrue()
        {
            Assert.IsTrue(TaskStateMachine.CanTransition("Новая", "Отклонена"));
        }

        [TestMethod]
        public void CanTransition_InProgress_To_OnReview_ReturnsTrue()
        {
            Assert.IsTrue(TaskStateMachine.CanTransition("В работе", "На проверке"));
        }

        [TestMethod]
        public void CanTransition_InProgress_To_Rejected_ReturnsTrue()
        {
            Assert.IsTrue(TaskStateMachine.CanTransition("В работе", "Отклонена"));
        }

        [TestMethod]
        public void CanTransition_OnReview_To_Completed_ReturnsTrue()
        {
            Assert.IsTrue(TaskStateMachine.CanTransition("На проверке", "Завершена"));
        }

        [TestMethod]
        public void CanTransition_OnReview_Back_To_InProgress_ReturnsTrue()
        {
            // Возврат на доработку
            Assert.IsTrue(TaskStateMachine.CanTransition("На проверке", "В работе"));
        }

        // ===== ЗАПРЕЩЁННЫЕ ПЕРЕХОДЫ (граничные случаи, Задача 3.3) =====

        [TestMethod]
        public void CanTransition_Completed_To_New_ReturnsFalse()
        {
            Assert.IsFalse(TaskStateMachine.CanTransition("Завершена", "Новая"));
        }

        [TestMethod]
        public void CanTransition_Rejected_To_InProgress_ReturnsFalse()
        {
            Assert.IsFalse(TaskStateMachine.CanTransition("Отклонена", "В работе"));
        }

        [TestMethod]
        public void CanTransition_New_To_Completed_ReturnsFalse()
        {
            // Нельзя пропустить промежуточные статусы
            Assert.IsFalse(TaskStateMachine.CanTransition("Новая", "Завершена"));
        }

        [TestMethod]
        public void CanTransition_Completed_To_OnReview_ReturnsFalse()
        {
            Assert.IsFalse(TaskStateMachine.CanTransition("Завершена", "На проверке"));
        }

        // ===== ПРОВЕРКА GetAllowedTransitions =====

        [TestMethod]
        public void GetAllowedTransitions_ForNew_ReturnsTwoOptions()
        {
            List<string> allowed = TaskStateMachine.GetAllowedTransitions("Новая");
            Assert.AreEqual(2, allowed.Count);
            CollectionAssert.Contains(allowed, "В работе");
            CollectionAssert.Contains(allowed, "Отклонена");
        }

        [TestMethod]
        public void GetAllowedTransitions_ForCompleted_ReturnsEmpty()
        {
            List<string> allowed = TaskStateMachine.GetAllowedTransitions("Завершена");
            Assert.AreEqual(0, allowed.Count,
                "Терминальное состояние не должно иметь переходов");
        }

        [TestMethod]
        public void GetAllowedTransitions_ForUnknown_ReturnsEmpty()
        {
            List<string> allowed = TaskStateMachine.GetAllowedTransitions("Неизвестный");
            Assert.AreEqual(0, allowed.Count);
        }

        // ===== ПОЛНОЕ ПОКРЫТИЕ МАТРИЦЫ ПЕРЕХОДОВ =====

        [TestMethod]
        public void AllValidTransitions_AreAllowed()
        {
            var validTransitions = new Dictionary<string, List<string>>
            {
                { "Новая",       new List<string> { "В работе", "Отклонена" } },
                { "В работе",    new List<string> { "На проверке", "Отклонена" } },
                { "На проверке", new List<string> { "Завершена", "В работе" } },
                { "Завершена",   new List<string>() },
                { "Отклонена",   new List<string>() }
            };

            foreach (var from in validTransitions)
            {
                foreach (var to in from.Value)
                {
                    Assert.IsTrue(TaskStateMachine.CanTransition(from.Key, to),
                        $"Переход '{from.Key}' → '{to}' должен быть разрешён");
                }
            }
        }
    }
}