using Godot;
using RollPunk.Client.Game;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController : IFormPresenter<MainMenu>
    {
        private MainMenu _view;

        private void OnStateChanged()
        {
            _view.SetInSession(Root.Runtime.SessionState == RuntimeSessionState.InSession);
        }

        public async void Attach(MainMenu form)
        {
            _view = form;

            _view.Initialize(Root.ReadedMods);
            _view.CreateSessionRequested += Root.Sessions.CreateLocal;
            _view.EnterSessionRequested += (adress) => _ = Root.Sessions.CreateOnline(adress);
            _view.ExitSessionRequested += () => Root.Sessions.Destroy();

            Root.Runtime.SessionStateChanged += OnStateChanged;
            OnStateChanged();

            ShowInitialContent();
            await LoadDynamicDataAsync();

            _view.VisibilityChanged += RefreshDynamicData;
        }

        public async void RefreshDynamicData()
        {
            await LoadDynamicDataAsync();
        }

        private void ShowInitialContent()
        {
            var manager = OnlineDataManager.Instance;

            manager.LoadAllFromCache();

            if (manager.HasCachedData)
            {
                UpdateDynamicContent(isFromCache: true);
            }
            else
            {
                ShowLoadingContent();
            }
        }

        private async Task LoadDynamicDataAsync()
        {
            try
            {
                // Загружаем свежие данные из сети
                var success = await OnlineDataManager.Instance.LoadAllDataAsync();
                var manager = OnlineDataManager.Instance;

                if (success && manager.LoadedFromNetwork)
                {
                    GD.Print("Динамические данные загружены успешно из сети");
                    // Обновляем UI свежими данными без пометки кеша
                    UpdateDynamicContent(isFromCache: false);
                }
                else if (manager.HasCachedData)
                {
                    GD.Print("Используются кешированные данные");
                    // Показываем кешированные данные с пометкой кеша
                    UpdateDynamicContent(isFromCache: true);
                }
                else
                {
                    GD.PrintErr("Не удалось загрузить динамические данные из сети и нет кеша");
                    ShowErrorContent();
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
            string loading = "[color=gray]Загрузка...[/color]";
            var currentVersion = ClientConfig.ClientVersion;

            _view.SetMotdText(loading);
            _view.SetUpdateLogsText(loading);
            _view.SetAuthorsText(loading);
            _view.SetVersionText($"[center][color=orange]Версия[/color][/center]\n{currentVersion}");
        }

        private void ShowErrorContent()
        {
            string unnableToLoad = "[color=red]Не удалось загрузить данные[/color]";
            var currentVersion = ClientConfig.ClientVersion;

            _view.SetMotdText(unnableToLoad);
            _view.SetUpdateLogsText(unnableToLoad);
            _view.SetAuthorsText(unnableToLoad);
            _view.SetVersionText($"[center][color=orange]Версия[/color][/center]\n{currentVersion}");
        }

        private void UpdateDynamicContent(bool isFromCache = false)
        {
            var manager = OnlineDataManager.Instance;
            var cacheIndicator = isFromCache ? "" : "";

            var motdMessage = manager.GetRandomMotdMessage();
            _view.SetMotdText($"{motdMessage}{cacheIndicator}");

            var updateLog = GetRelevantUpdateLog(manager);
            _view.SetUpdateLogsText($"{updateLog}{cacheIndicator}");

            var authorsText = manager.GetAuthorsText();
            _view.SetAuthorsText($"{authorsText}{cacheIndicator}");

            var versionText = GetVersionText(manager);
            _view.SetVersionText($"[center][color=orange]Версия[/color][/center]\n{versionText}{cacheIndicator}");
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
    }
}
