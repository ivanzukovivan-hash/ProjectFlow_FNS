using System.Collections.Generic;

namespace ProjectFlow_FNS.Helpers
{
    /// <summary>
    /// Математическая модель конечного автомата (State Machine)
    /// для управления переходами статусов задачи.
    /// Реализует требования Задачи 3.1.
    /// </summary>
    public static class TaskStateMachine
    {
        // Матрица допустимых переходов (граф состояний)
        private static readonly Dictionary<string, List<string>> Transitions =
            new Dictionary<string, List<string>>
            {
                { "Новая",       new List<string> { "В работе", "Отклонена" } },
                { "В работе",    new List<string> { "На проверке", "Отклонена" } },
                { "На проверке", new List<string> { "Завершена", "В работе" } },
                { "Завершена",   new List<string>() }, // Терминальное состояние
                { "Отклонена",   new List<string>() }  // Терминальное состояние
            };

        /// <summary>
        /// Проверяет допустимость перехода между статусами.
        /// </summary>
        public static bool CanTransition(string currentStatus, string newStatus)
        {
            if (Transitions.ContainsKey(currentStatus))
                return Transitions[currentStatus].Contains(newStatus);
            return false;
        }

        /// <summary>
        /// Возвращает список допустимых следующих статусов.
        /// </summary>
        public static List<string> GetAllowedTransitions(string currentStatus)
        {
            return Transitions.ContainsKey(currentStatus) ? Transitions[currentStatus] : new List<string>();
        }
    }
}