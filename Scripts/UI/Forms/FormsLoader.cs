using Godot;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace RollPunk.UI.Forms
{
    /// <summary>
    /// Инстанцирует формы по их типу, читая FormSceneAttribute.
    /// Кеширует PackedScene по типу, чтобы не грузить .tscn повторно.
    /// </summary>
    public class FormsLoader
    {
        private readonly ConcurrentDictionary<Type, PackedScene> _sceneCache = new();

        /// <summary>
        /// Создаёт инстанс формы указанного типа. Тип должен быть помечен FormSceneAttribute.
        /// </summary>
        public T Instantiate<T>() where T : Form
        {
            return (T)Instantiate(typeof(T));
        }

        /// <summary>
        /// Создаёт инстанс формы указанного типа. Тип должен быть помечен FormSceneAttribute.
        /// </summary>
        public Form Instantiate(Type formType)
        {
            if (formType == null)
                throw new ArgumentNullException(nameof(formType));

            if (!typeof(Form).IsAssignableFrom(formType))
                throw new ArgumentException($"Type '{formType.Name}' is not a Form", nameof(formType));

            var scene = ResolveScene(formType);
            var instance = scene.Instantiate();

            if (instance is Form form)
                return form;

            instance.QueueFree();
            throw new InvalidOperationException(
                $"Scene '{GetScenePath(formType)}' root is not a Form (actual: {instance.GetType().Name}).");
        }

        private PackedScene ResolveScene(Type formType)
        {
            return _sceneCache.GetOrAdd(formType, static type =>
            {
                var path = GetScenePath(type);
                var scene = GD.Load<PackedScene>(path);
                if (scene == null)
                    throw new InvalidOperationException(
                        $"Failed to load scene '{path}' for form '{type.Name}'.");
                return scene;
            });
        }

        private static string GetScenePath(Type formType)
        {
            var attr = formType.GetCustomAttribute<FormSceneAttribute>(inherit: true);
            if (attr == null)
                throw new InvalidOperationException(
                    $"Form '{formType.Name}' is not marked with [FormScene(...)] attribute.");
            return attr.ScenePath;
        }
    }
}
