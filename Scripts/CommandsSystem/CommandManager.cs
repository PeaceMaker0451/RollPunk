using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace PunkCommandSystem
{
    public class CommandManager
    {
        private readonly Dictionary<string, Command> _commands;

        public CommandManager()
        {
            _commands = new Dictionary<string, Command>();
        }

        public void AddCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (_commands.ContainsKey(command.Name()))
                throw new InvalidOperationException($"Command with name '{command.Name()}' already exists.");

            _commands.Add(command.Name(), command);
        }

        public void RemoveCommand(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentNullException(nameof(commandName));

            if (!_commands.ContainsKey(commandName))
                throw new KeyNotFoundException($"Command with name '{commandName}' not found.");

            _commands.Remove(commandName);
        }

        public async Task<string> ExecuteCommandAsync(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
                throw new ArgumentNullException(nameof(commandString));

            var commandName = ExtractCommandName(commandString);

            if (!_commands.TryGetValue(commandName, out var command))
                throw new KeyNotFoundException($"Command with name '{commandName}' not found.");

            var commandCopy = command.Clone();

            return await commandCopy.RunCommand(commandString);
        }

        public string ExecuteCommand(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
                throw new ArgumentNullException(nameof(commandString));

            var commandName = ExtractCommandName(commandString);

            if (!_commands.TryGetValue(commandName, out var command))
                throw new KeyNotFoundException($"Command with name '{commandName}' not found.");

            var commandCopy = command.Clone();

            return commandCopy.RunCommand(commandString).Result;
        }

        public List<Command> CommandsList()
        {
            List<Command> commands = new List<Command>();

            foreach ( var command in _commands.Values )
            {
                commands.Add(command.Clone());
            }

            return commands;
        }

        // Извлечь имя команды из строки команды
        private static string ExtractCommandName(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
                throw new ArgumentException("Command string is null or empty.", nameof(commandString));

            var parts = commandString.Split(new[] { ' ' }, 2);
            return parts[0];
        }
    }
}
