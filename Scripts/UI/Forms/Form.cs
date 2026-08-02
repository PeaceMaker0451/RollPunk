using Godot;
using System;

namespace RollPunk.UI.Forms
{
	public enum ResizeMode
	{
		none,
		fixedAspect,
		full
	}

	public partial class Form : Control
	{
		private bool _formSizeSaved = false;

		[Export] public string Title { get; protected set; } = string.Empty;

		public Form() { }
		
		public Form(string title)
		{
			Title = title;
		}

		public virtual void OnShow() { }

		public virtual void OnHide() { }
	}
}
