using System;

namespace RollPunk.UI.Forms
{
    /// <summary>
    /// Указывает путь к .tscn-сцене для класса формы.
    /// Используется FormsLoader для инстанцирования формы по её типу.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class FormSceneAttribute : Attribute
    {
        public string ScenePath { get; }

        public FormSceneAttribute(string scenePath)
        {
            if (string.IsNullOrWhiteSpace(scenePath))
                throw new ArgumentException("Scene path must be non-empty", nameof(scenePath));

            ScenePath = scenePath;
        }
    }
}
