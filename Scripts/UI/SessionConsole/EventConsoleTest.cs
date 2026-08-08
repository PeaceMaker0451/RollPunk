using Godot;
using RollPunk.Logs;
using System;
using System.Collections.Generic;

namespace RollPunk.Scripts.UI.SessionConsole
{
	internal partial class EventConsoleTest : Node
	{
		[Export] EventConsole _console;

		public override void _Ready()
		{
			List<Event> events = new()
			{
				new Event("Rollpunk", SourceType.System, "peacemkr_png join session", DateTime.UtcNow.AddDays(-2)),
				new Event("peacemkr_png", SourceType.User, "Killed an enemy!", DateTime.UtcNow),
				new Event("peacemkr_png", SourceType.User, "Healed", DateTime.Now),
				new Event("peacemkr_png", SourceType.User, "Died!", DateTime.Now),
			};

			_console.InsertLogCollection(events);
			_console.AddLog(new("Lina", SourceType.User, "солнышка такая", DateTime.Now));
			_console.AddLog(new("Rollpunk", SourceType.Error, "Error!", DateTime.Now));
		}
	}
}
