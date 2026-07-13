using Godot;
using RollPunk.Client.Forms;
using RollPunk.Client.Runtime;
using RollPunk.Debug;
using RollPunk.UI.Forms;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    internal enum MainMenuTab
    {
        Main,
        EnterSession,
        CreateSession,
    }

    internal partial class MainMenu : Form
    {
        [Export] private MainSubMenu _mainMenu;
        [Export] private EnterSessionSubMenu _enterMenu;
        [Export] private MainSubMenu _createMenu;
        
        [Export] private RichTextLabel _motdLabel;
        [Export] private RichTextLabel _updateLogsLabel;
        [Export] private RichTextLabel _authorsLabel;
        [Export] private RichTextLabel _versionLabel;

        public event Action CreateSessionRequested;
        public event Action<string> EnterSessionRequested;
        public event Action ExitSessionRequested;

        public override void _Ready()
        {
            base._Ready();
            Initialize();
        }

        public async void Initialize()
        {
            _mainMenu.Initialize(this);
            _mainMenu.CreateSessionPressed += () => CreateSessionRequested?.Invoke();
            _mainMenu.ExitSessionPressed += () => ExitSessionRequested?.Invoke();

            _enterMenu.Initialize(this);
            _enterMenu.EnterSessionRequested += (adress) => EnterSessionRequested?.Invoke(adress);

            SetMenu(MainMenuTab.Main);
            
            // Сначала показываем кешированные данные или заглушки
            ShowInitialContent();
            
            // Затем загружаем свежие данные из сети
            await LoadDynamicDataAsync();
            
            GD.Print("Главное меню инициализировано");
        }

        public void SetMenu(MainMenuTab tab)
        {
            GD.Print($"Main menu set menu {tab}");

            DisableAllMenus();

            switch (tab)
            {
                case (MainMenuTab.Main):
                    _mainMenu.Show();
                    break;

                case (MainMenuTab.EnterSession):
                    _enterMenu.Show();
                    break;

                case (MainMenuTab.CreateSession):
                    _mainMenu.Show();
                    break;

                default:
                    RPDebug.LogError($"Menu for this button is not exists yet - {tab}");
                    break;
            }
        }

        public void SetInSession(bool isInSession)
        {
            _mainMenu.SetInSession(isInSession);
        }

        public void SetMenuData(string data)
        {

        }

        private void DisableAllMenus()
        {
            _mainMenu.Hide();
            _enterMenu.Hide();
        }

        private void ShowInitialContent()
        {
            var manager = OnlineDataManager.Instance;
            
            // Загружаем кешированные данные
            manager.LoadAllFromCache();
            
            if (manager.HasCachedData)
            {
                // Показываем кешированные данные
                UpdateDynamicContent(isFromCache: true);
            }
            else
            {
                // Показываем заглушки загрузки
                ShowLoadingContent();
            }
        }

        private async Task LoadDynamicDataAsync()
        {
            try
            {
                // Загружаем свежие данные из сети
                var success = await OnlineDataManager.Instance.LoadAllDataAsync();
                
                if (success)
                {
                    GD.Print("Динамические данные загружены успешно из сети");
                    // Обновляем UI свежими данными
                    UpdateDynamicContent(isFromCache: false);
                }
                else
                {
                    GD.PrintErr("Не удалось загрузить динамические данные из сети");
                    // Если не было кешированных данных, показываем ошибку
                    if (!OnlineDataManager.Instance.HasCachedData)
                    {
                        ShowErrorContent();
                    }
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка при загрузке динамических данных: {ex.Message}");
                // Если не было кешированных данных, показываем ошибку
                if (!OnlineDataManager.Instance.HasCachedData)
                {
                    ShowErrorContent();
                }
            }
        }

        private void ShowLoadingContent()
        {
            if (_motdLabel != null)
            {
                _motdLabel.Text = "[center][color=yellow]Сообщение дня[/color][/center]\n[color=gray]Загрузка...[/color]";
            }

            if (_updateLogsLabel != null)
            {
                _updateLogsLabel.Text = "[center][color=cyan]Обновления[/color][/center]\n[color=gray]Загрузка...[/color]";
            }

            if (_authorsLabel != null)
            {
                _authorsLabel.Text = "[center][color=green]Авторы[/color][/center]\n[color=gray]Загрузка...[/color]";
            }

            if (_versionLabel != null)
            {
                var currentVersion = ClientConfig.ClientVersion;
                _versionLabel.Text = $"[center][color=orange]Версия[/color][/center]\n{currentVersion}";
            }
        }

        private void ShowErrorContent()
        {
            if (_motdLabel != null)
            {
                _motdLabel.Text = "[center][color=yellow]Сообщение дня[/color][/center]\n[color=red]Не удалось загрузить данные[/color]";
            }

            if (_updateLogsLabel != null)
            {
                _updateLogsLabel.Text = "[center][color=cyan]Обновления[/color][/center]\n[color=red]Не удалось загрузить данные[/color]";
            }

            if (_authorsLabel != null)
            {
                _authorsLabel.Text = "[center][color=green]Авторы[/color][/center]\n[color=red]Не удалось загрузить данные[/color]";
            }

            if (_versionLabel != null)
            {
                var currentVersion = ClientConfig.ClientVersion;
                _versionLabel.Text = $"[center][color=orange]Версия[/color][/center]\n{currentVersion}";
            }
        }

        private void UpdateDynamicContent(bool isFromCache = false)
        {
            var manager = OnlineDataManager.Instance;
            var cacheIndicator = isFromCache ? " [color=gray](кеш)[/color]" : "";

            // Обновляем MOTD
            if (_motdLabel != null)
            {
                var motdMessage = manager.GetRandomMotdMessage();
                _motdLabel.Text = $"[center][color=yellow]Сообщение дня{cacheIndicator}[/color][/center]\n{motdMessage}";
            }

            // Обновляем логи обновлений
            if (_updateLogsLabel != null)
            {
                var updateLog = GetRelevantUpdateLog(manager);
                _updateLogsLabel.Text = $"[center][color=cyan]Обновления{cacheIndicator}[/color][/center]\n{updateLog}";
            }

            // Обновляем информацию об авторах
            if (_authorsLabel != null)
            {
                var authorsText = manager.GetAuthorsText();
                _authorsLabel.Text = $"[center][color=green]Авторы{cacheIndicator}[/color][/center]\n{authorsText}";
            }

            // Обновляем версию
            if (_versionLabel != null)
            {
                var versionText = GetVersionText(manager);
                _versionLabel.Text = $"[center][color=orange]Версия{cacheIndicator}[/color][/center]\n{versionText}";
            }
        }

        private string GetRelevantUpdateLog(OnlineDataManager manager)
        {
            var logs = manager.GetUpdateLogs();
            var currentVersion = ClientConfig.ClientVersion;

            // Если есть лог для текущей версии, показываем его
            if (logs.ContainsKey(currentVersion))
            {
                return $"{currentVersion}:\n{logs[currentVersion]}";
            }

            // Иначе ищем наиболее подходящую версию
            var bestMatch = FindBestVersionMatch(logs.Keys.ToArray(), currentVersion);
            if (bestMatch != null && logs.ContainsKey(bestMatch))
            {
                return $"{bestMatch}:\n{logs[bestMatch]}";
            }

            // Если ничего не найдено, показываем последнюю доступную
            return manager.GetLatestUpdateLog();
        }

        private string GetVersionText(OnlineDataManager manager)
        {
            var currentVersion = ClientConfig.ClientVersion;
            var availableLogs = manager.GetUpdateLogs();

            if (availableLogs.ContainsKey(currentVersion))
            {
                return $"{currentVersion} [color=green](актуальная)[/color]";
            }

            // Проверяем, есть ли более новая версия
            var newerVersion = FindNewerVersion(availableLogs.Keys.ToArray(), currentVersion);
            if (newerVersion != null)
            {
                return $"{currentVersion} [color=red](доступно обновление: {newerVersion})[/color]";
            }

            return $"{currentVersion}";
        }

        private string FindBestVersionMatch(string[] availableVersions, string currentVersion)
        {
            if (availableVersions.Length == 0) return null;

            // Пытаемся найти версию с наибольшим совпадением
            var currentVersionNumber = ExtractVersionNumber(currentVersion);
            if (currentVersionNumber == null) return availableVersions.FirstOrDefault();

            string bestMatch = null;
            double bestScore = -1;

            foreach (var version in availableVersions)
            {
                var versionNumber = ExtractVersionNumber(version);
                if (versionNumber != null)
                {
                    var score = CalculateVersionSimilarity(currentVersionNumber, versionNumber);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMatch = version;
                    }
                }
            }

            return bestMatch ?? availableVersions.FirstOrDefault();
        }

        private string FindNewerVersion(string[] availableVersions, string currentVersion)
        {
            var currentVersionNumber = ExtractVersionNumber(currentVersion);
            if (currentVersionNumber == null) return null;

            string newestVersion = null;
            Version newestVersionNumber = null;

            foreach (var version in availableVersions)
            {
                var versionNumber = ExtractVersionNumber(version);
                if (versionNumber != null && versionNumber > currentVersionNumber)
                {
                    if (newestVersionNumber == null || versionNumber > newestVersionNumber)
                    {
                        newestVersion = version;
                        newestVersionNumber = versionNumber;
                    }
                }
            }

            return newestVersion;
        }

        private Version ExtractVersionNumber(string versionString)
        {
            if (string.IsNullOrEmpty(versionString)) return null;

            // Ищем паттерн версии типа "0.6.0" или "1.2.3"
            var match = Regex.Match(versionString, @"(\d+)\.(\d+)\.(\d+)");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int major) &&
                    int.TryParse(match.Groups[2].Value, out int minor) &&
                    int.TryParse(match.Groups[3].Value, out int patch))
                {
                    return new Version(major, minor, patch);
                }
            }

            // Пытаемся найти паттерн "0.6"
            match = Regex.Match(versionString, @"(\d+)\.(\d+)");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int major) &&
                    int.TryParse(match.Groups[2].Value, out int minor))
                {
                    return new Version(major, minor, 0);
                }
            }

            return null;
        }

        private double CalculateVersionSimilarity(Version v1, Version v2)
        {
            // Простая метрика схожести версий
            double score = 0;
            
            if (v1.Major == v2.Major) score += 100;
            if (v1.Minor == v2.Minor) score += 10;
            if (v1.Build == v2.Build) score += 1;
            
            // Штраф за разность версий
            score -= Math.Abs(v1.Major - v2.Major) * 50;
            score -= Math.Abs(v1.Minor - v2.Minor) * 5;
            score -= Math.Abs(v1.Build - v2.Build) * 0.5;

            return score;
        }

        // Публичный метод для обновления данных извне
        public async void RefreshDynamicData()
        {
            await LoadDynamicDataAsync();
        }
    }
}
