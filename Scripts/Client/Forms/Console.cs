using Godot;
using PunkCommandSystem;
using RollPunk.Debug;
using RollPunk.Scripts.Client.Forms;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client.Forms
{
    [FormScene("res://Scenes/FormsScenes/Console.tscn")]
    internal partial class Console : Form
	{
		[Export] Button _sendCommandButton;
		[Export] RichTextLabel _consoleField;
		[Export] LineEdit _consoleWriteLine;

		private ClientConsole _console;
		private CommandManager _commandManager;

        public override void _Ready()
        {
            _sendCommandButton.Pressed += SendCommandButton_Pressed;
            _consoleWriteLine.TextSubmitted += ConsoleWriteLine_TextSubmitted;
        }
		
		public void Initialize(ClientConsole console, CommandManager commandManager = null)
		{
			if (_console != null)
				_console.ConsoleUpdated -= AddTextToConsole;
			
			_commandManager = commandManager;

			if(_commandManager == null)
			{
				_consoleWriteLine.Visible = false;
				_sendCommandButton.Visible = false;
			}
			
			_console = console;
            _console.ConsoleUpdated += AddTextToConsole;
            UpdateConsole();
        }

		private void ConsoleWriteLine_TextSubmitted(string newText)
		{
			ExecuteCommand(_consoleWriteLine.Text);
			_consoleWriteLine.Text = "";
		}

		private void SendCommandButton_Pressed()
		{
			ExecuteCommand(_consoleWriteLine.Text);
			_consoleWriteLine.Text = "";
		}

		private void ExecuteCommand(string command)
		{
			try
			{
				string result = _commandManager.ExecuteCommandAsync(command).Result;
                AddTextToConsole($"{command} =>\n{result}");
			}
			catch (Exception e)
			{
                AddTextToConsole($"{command} =>\n{e.Message}");
			}
		}

		public void UpdateConsoleField(string _consoleField)
		{
			this.CallDeferred(nameof(UpdateConsole));
		}

		private void UpdateConsole()
		{
			_consoleField.Text = _console.ConsoleBuffer;
		}

		private void AddTextToConsole(string text)
		{
			_consoleField.CallDeferred(RichTextLabel.MethodName.AppendText, text);
		}
	}
}
