using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    public class OnlineDataManager
    {
        private const string BASE_URL = "https://raw.githubusercontent.com/PeaceMaker0451/rollpunk-online/main/";
        private const string CACHE_FOLDER = "online_cache";
        
        private static OnlineDataManager _instance;
        public static OnlineDataManager Instance => _instance ??= new OnlineDataManager();

        private AuthorsData _authorsData;
        private ConnectionConfigData _connectionConfigData;
        private MotdData _motdData;
        private UpdateLogsData _updateLogsData;
        
        private bool _loadedFromNetwork = false;

        private OnlineDataManager() { }

        public async Task<bool> LoadAllDataAsync()
        {
            // Сначала загружаем из кеша
            LoadAllFromCache();
            
            // Сбрасываем флаг перед попыткой загрузки из сети
            _loadedFromNetwork = false;
            
            var tasks = new[]
            {
                LoadAuthorsDataAsync(),
                LoadConnectionConfigAsync(),
                LoadMotdDataAsync(),
                LoadUpdateLogsAsync()
            };

            var results = await Task.WhenAll(tasks);
            
            // Возвращаем true только если хотя бы один файл загрузился успешно из сети
            var successFromNetwork = false;
            foreach (var result in results)
            {
                if (result)
                {
                    successFromNetwork = true;
                    break;
                }
            }
            
            // Устанавливаем флаг только если была успешная загрузка из сети
            if (successFromNetwork)
            {
                _loadedFromNetwork = true;
            }
            
            return successFromNetwork;
        }

        public async Task<bool> LoadAuthorsDataAsync()
        {
            var data = await LoadDataAsync<AuthorsData>("authors.json");
            if (data != null)
            {
                _authorsData = data;
                return true;
            }
            return false;
        }

        public async Task<bool> LoadConnectionConfigAsync()
        {
            var data = await LoadDataAsync<ConnectionConfigData>("connection-config.json");
            if (data != null)
            {
                _connectionConfigData = data;
                return true;
            }
            return false;
        }

        public async Task<bool> LoadMotdDataAsync()
        {
            var data = await LoadDataAsync<MotdData>("motd.json");
            if (data != null)
            {
                _motdData = data;
                return true;
            }
            return false;
        }

        public async Task<bool> LoadUpdateLogsAsync()
        {
            var data = await LoadDataAsync<UpdateLogsData>("update-logs.json");
            if (data != null)
            {
                _updateLogsData = data;
                return true;
            }
            return false;
        }

        private async Task<T> LoadDataAsync<T>(string fileName) where T : class
        {
            try
            {
                // Пытаемся загрузить из интернета
                var httpRequest = new HttpRequest();
                
                // Используем CallDeferred для безопасного добавления узла
                GetTree().Root.CallDeferred("add_child", httpRequest);
                
                // Ждем, пока узел будет готов
                await WaitForNodeReady(httpRequest);
                
                var url = BASE_URL + fileName;
                var error = httpRequest.Request(url);
                
                if (error == Error.Ok)
                {
                    var response = await WaitForHttpResponse(httpRequest);
                    if (response != null)
                    {
                        var data = JsonConvert.DeserializeObject<T>(response);
                        if (data != null)
                        {
                            // Сохраняем в кеш
                            SaveToCache(fileName, response);
                            httpRequest.QueueFree();
                            return data;
                        }
                    }
                }
                
                httpRequest.QueueFree();
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка загрузки {fileName} из интернета: {ex.Message}");
            }

            // Если не удалось загрузить из интернета, пытаемся загрузить из кеша
            return LoadFromCache<T>(fileName);
        }

        private async Task WaitForNodeReady(Node node)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            // Если узел уже готов, возвращаемся сразу
            if (node.IsInsideTree())
            {
                return;
            }
            
            // Иначе ждем сигнала ready
            node.Ready += () => tcs.SetResult(true);
            
            // Таймаут 5 секунд на готовность узла
            await Task.WhenAny(tcs.Task, Task.Delay(5000));
        }

        private async Task<string> WaitForHttpResponse(HttpRequest httpRequest)
        {
            var tcs = new TaskCompletionSource<string>();
            
            httpRequest.RequestCompleted += (long result, long responseCode, string[] headers, byte[] body) =>
            {
                if (responseCode == 200 && body.Length > 0)
                {
                    var response = System.Text.Encoding.UTF8.GetString(body);
                    tcs.SetResult(response);
                }
                else
                {
                    tcs.SetResult(null);
                }
            };

            // Таймаут 10 секунд
            await Task.WhenAny(tcs.Task, Task.Delay(10000));
            
            if (tcs.Task.IsCompleted)
                return tcs.Task.Result;
            
            return null;
        }

        private void SaveToCache(string fileName, string data)
        {
            try
            {
                GodotSaveManager.Save(SaveFolder.application, $"{CACHE_FOLDER}/{fileName}", data);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка сохранения {fileName} в кеш: {ex.Message}");
            }
        }

        private T LoadFromCache<T>(string fileName) where T : class
        {
            try
            {
                if (GodotSaveManager.TryLoad(SaveFolder.application, $"{CACHE_FOLDER}/{fileName}", out string cachedData))
                {
                    return JsonConvert.DeserializeObject<T>(cachedData);
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Ошибка загрузки {fileName} из кеша: {ex.Message}");
            }
            
            return null;
        }

        private SceneTree GetTree()
        {
            return Engine.GetMainLoop() as SceneTree;
        }

        // Публичные методы для получения данных
        public string GetAuthorsText()
        {
            return _authorsData?.Authors ?? "Авторы не загружены";
        }

        public string GetDirectAddress()
        {
            return _connectionConfigData?.DirectAddress ?? "";
        }

        public string[] GetMotdMessages()
        {
            return _motdData?.Messages ?? new string[] { "Сообщения дня не загружены" };
        }

        public string GetRandomMotdMessage()
        {
            var messages = GetMotdMessages();
            if (messages.Length == 0) return "Нет сообщений";
            
            var random = new Random();
            return messages[random.Next(messages.Length)];
        }

        public Dictionary<string, string> GetUpdateLogs()
        {
            return _updateLogsData?.UpdateLogs ?? new Dictionary<string, string>();
        }

        public string GetLatestUpdateLog()
        {
            var logs = GetUpdateLogs();
            if (logs.Count == 0) return "Логи обновлений не загружены";
            
            // Возвращаем первый элемент (предполагается что они отсортированы по версиям)
            foreach (var kvp in logs)
            {
                return $"{kvp.Key}:\n{kvp.Value}";
            }
            
            return "Нет логов обновлений";
        }

        public void LoadAllFromCache()
        {
            _authorsData = LoadFromCache<AuthorsData>("authors.json");
            _connectionConfigData = LoadFromCache<ConnectionConfigData>("connection-config.json");
            _motdData = LoadFromCache<MotdData>("motd.json");
            _updateLogsData = LoadFromCache<UpdateLogsData>("update-logs.json");
        }

        // Проверка доступности данных
        public bool IsAuthorsDataLoaded => _authorsData != null;
        public bool IsConnectionConfigLoaded => _connectionConfigData != null;
        public bool IsMotdDataLoaded => _motdData != null;
        public bool IsUpdateLogsLoaded => _updateLogsData != null;
        
        // Проверка наличия кешированных данных
        public bool HasCachedData => IsAuthorsDataLoaded || IsConnectionConfigLoaded || IsMotdDataLoaded || IsUpdateLogsLoaded;
        
        // Проверка, были ли данные загружены из сети
        public bool LoadedFromNetwork => _loadedFromNetwork;
    }

    // Классы для десериализации JSON данных
    public class AuthorsData
    {
        [JsonProperty("authors")]
        public string Authors { get; set; }
    }

    public class ConnectionConfigData
    {
        [JsonProperty("direct_adress")]
        public string DirectAddress { get; set; }
    }

    public class MotdData
    {
        [JsonProperty("messages")]
        public string[] Messages { get; set; }
    }

    public class UpdateLogsData
    {
        [JsonProperty]
        public Dictionary<string, string> UpdateLogs { get; set; }

        // Конструктор для правильной десериализации словаря
        public UpdateLogsData()
        {
            UpdateLogs = new Dictionary<string, string>();
        }

        // Метод для десериализации всех свойств как элементы словаря
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalData
        {
            get => null;
            set
            {
                if (value != null)
                {
                    foreach (var kvp in value)
                    {
                        UpdateLogs[kvp.Key] = kvp.Value?.ToString() ?? "";
                    }
                }
            }
        }
    }
}
