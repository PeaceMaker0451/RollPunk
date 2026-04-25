using Godot;
using RollPunk.Debug;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client.Forms
{
	internal partial class Console : Form
	{
		[Export] Button sendCommandButton;
		[Export] RichTextLabel consoleField;
		[Export] LineEdit consoleWriteLine;
		public override void _Ready()
		{
			Client.Instance.Console.ConsoleUpdated += AddTextToConsole;
			sendCommandButton.Pressed += SendCommandButton_Pressed;
			consoleWriteLine.TextSubmitted += ConsoleWriteLine_TextSubmitted;
			UpdateConsole();
		}

		private void ConsoleWriteLine_TextSubmitted(string newText)
		{
			ExecuteCommand(consoleWriteLine.Text);
			consoleWriteLine.Text = "";
		}

		private void SendCommandButton_Pressed()
		{
			ExecuteCommand(consoleWriteLine.Text);
			consoleWriteLine.Text = "";
		}

		private void ExecuteCommand(string command)
		{
			try
			{
				string result = Client.Instance.CommandManager.ExecuteCommandAsync(command).Result;
				RPDebug.Log($"{command} =>\n{result}");
			}
			catch (Exception e)
			{
                RPDebug.Log($"{command} =>\n{e.Message}");
			}

		}

		public void UpdateConsoleField(string _consoleField)
		{
			this.CallDeferred(nameof(UpdateConsole));

		}

		private void UpdateConsole()
		{
			consoleField.Text = Client.Instance.Console.ConsoleBuffer.ToString();
		}

		private void AddTextToConsole(string text)
		{
			consoleField.AppendText(text);
		}
	}
}
