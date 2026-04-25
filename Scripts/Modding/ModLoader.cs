using Godot;
using MoonSharp.Interpreter;
using RollPunk.Debug;
using RollPunk.Modding.APIs;
using System;
using System.Linq;
using System.Reflection;
using static Godot.ClassDB;

namespace RollPunk.Modding
{
    public class ModLoader
    {
        public ModLoader()
        {
            RegisterAPITypes();
            RegisterAPIExtensionsTypes();
        }

        public void LoadMod(Mod mod)
        {
            string modPath = mod.modPath;
            RPDebug.Log($"[color=medium_slate_blue]{this} - Loading Mod '{mod.modData.Name}' at {modPath}...[/color]");

            ForbidUnsafeLibraries(mod.scriptSpace);

            string initPath = $"{modPath}/init.lua";

            if (!FileAccess.FileExists(initPath))
            {
                RPDebug.LogError($"{this} - init.lua not found at {initPath}");
                return;
            }

            ExecuteLuaFile(initPath, mod);
        }

        public static void RegisterAPIType(Type apiType)
        {
            RPDebug.DebugLog($"Регистрируем тип {apiType}");
            UserData.RegisterType(apiType);
        }

        private void ExecuteLuaFile(string filePath, Mod mod)
        {
            var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                RPDebug.LogError($"{this} - Failed to open Lua script at {filePath}");
                return;
            }

            try
            {
                string scriptContent = file.GetAsText();

                mod.scriptSpace.DoString(
                    scriptContent,
                    codeFriendlyName: filePath
                );

                RPDebug.Log($"[color=medium_slate_blue]{this} - Executed Lua entry: {filePath}[/color]");
            }
            catch (Exception ex)
            {
                LuaErrorsHandler.Handle(ex);
            }
            finally
            {
                file.Close();
            }
        }

        private static void RegisterAPITypes()
        {
            var apiBaseType = typeof(API);

            var apiTypesByAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    apiBaseType.IsAssignableFrom(t)));

            foreach (var assemblyTypes in apiTypesByAssemblies)
            {
                foreach(var type in assemblyTypes)
                {
                    RegisterAPIType(type);
                }
                    
            }
        }

        private static void RegisterAPIExtensionsTypes()
        {
            var apiExtensionAttributeType = typeof(APIExtensionAttribute);

            var apiTypesByAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    t.GetCustomAttribute(apiExtensionAttributeType) != null));

            foreach (var assemblyTypes in apiTypesByAssemblies)
            {
                foreach (var type in assemblyTypes)
                    UserData.RegisterExtensionType(type);
            }
        }

        private void ForbidUnsafeLibraries(MoonSharp.Interpreter.Script script)
        {
            // Отключаем доступ к глобальному окружению (включая возможность менять глобальные переменные)
            script.Globals["_G"] = DynValue.Nil;

            // Отключаем функции, связанные с доступом к файловой системе
            script.Globals["dofile"] = DynValue.Nil;
            script.Globals["loadfile"] = DynValue.Nil;
            script.Globals["loadstring"] = DynValue.Nil;
            script.Globals["load"] = DynValue.Nil;

            // Отключаем функции, которые могут влиять на работу сборщика мусора
            script.Globals["collectgarbage"] = DynValue.Nil;

            // Отключаем доступ к системным командам и процессам
            script.Globals["os"] = DynValue.Nil;
            script.Globals["io"] = DynValue.Nil;

            // Отключаем функции, связанные с изменением или доступом к окружению
            script.Globals["setfenv"] = DynValue.Nil;
            script.Globals["getfenv"] = DynValue.Nil;

            // Отключаем метаметоды, которые могут быть использованы для модификации метатаблиц
            script.Globals["getmetatable"] = DynValue.Nil;
            //script.Globals["setmetatable"] = DynValue.Nil;
            script.Globals["rawget"] = DynValue.Nil;
            script.Globals["rawset"] = DynValue.Nil;
            script.Globals["rawequal"] = DynValue.Nil;

            // Отключаем функции, связанные с модульной системой, которая может дать доступ к глобальному окружению
            //script.Globals["require"] = DynValue.Nil;
            script.Globals["module"] = DynValue.Nil;
            script.Globals["package"] = DynValue.Nil;

            // Отключаем функции отладки, которые могут быть использованы для манипуляции окружением
            script.Globals["debug"] = DynValue.Nil;

            // Отключаем другие небезопасные функции из math и string, которые могут взаимодействовать с кодом за пределами песочницы
            script.Globals["string"] = DynValue.Nil;
            //script.Globals["math.randomseed"] = DynValue.Nil;
            script.Globals["newproxy"] = DynValue.Nil; // скрытая функция, которая может быть небезопасной
        }
    }
}