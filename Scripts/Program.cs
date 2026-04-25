using Godot;
using RollPunk.Client;
using System;
using System.Linq;

public partial class Program : Node
{
	public override void _Ready()
	{
		var arguments = OS.GetCmdlineArgs();

		if (arguments.Contains("-s"))
		{
            PackedScene packedServerScene = ResourceLoader.Load<PackedScene>("res://Scenes/roll_punk_server.tscn");
            Node serverInstance = packedServerScene.Instantiate();
            AddChild(serverInstance);
        }
		else
		{
			Client client = new(this);
        }
	}
}
