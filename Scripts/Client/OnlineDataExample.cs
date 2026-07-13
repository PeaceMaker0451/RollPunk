using Godot;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    /// <summary>
    /// Пример использования OnlineDataManager
    /// </summary>
    public partial class OnlineDataExample : Node
    {
        public override async void _Ready()
        {
            // Загружаем все данные при старте
            var success = await OnlineDataManager.Instance.LoadAllDataAsync();
            
            if (success)
            {
                GD.Print("Онлайн данные загружены успешно!");
                
                // Примеры использования
                ShowExampleUsage();
            }
            else
            {
                GD.PrintErr("Не удалось загрузить онлайн данные");
            }
        }

        private void ShowExampleUsage()
        {
            var manager = OnlineDataManager.Instance;

            // Получение текста авторов
            if (manager.IsAuthorsDataLoaded)
            {
                GD.Print($"Авторы: {manager.GetAuthorsText()}");
            }

            // Получение адреса сервера
            if (manager.IsConnectionConfigLoaded)
            {
                GD.Print($"Адрес сервера: {manager.GetDirectAddress()}");
            }

            // Получение случайного сообщения дня
            if (manager.IsMotdDataLoaded)
            {
                GD.Print($"Сообщение дня: {manager.GetRandomMotdMessage()}");
                
                // Или все сообщения
                var allMessages = manager.GetMotdMessages();
                GD.Print($"Всего сообщений: {allMessages.Length}");
            }

            // Получение логов обновлений
            if (manager.IsUpdateLogsLoaded)
            {
                GD.Print($"Последнее обновление: {manager.GetLatestUpdateLog()}");
                
                // Или все логи
                var allLogs = manager.GetUpdateLogs();
                foreach (var log in allLogs)
                {
                    GD.Print($"Версия {log.Key}: {log.Value}");
                }
            }
        }

        // Метод для принудительного обновления данных
        public async void RefreshData()
        {
            await OnlineDataManager.Instance.LoadAllDataAsync();
            ShowExampleUsage();
        }
    }
}
