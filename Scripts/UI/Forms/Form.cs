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
		[Export] public ResizeMode resizable { get; protected set; } = ResizeMode.fixedAspect;

		public Vector2I formSize { get; protected set; }
		public Vector2 MinSize { get; protected set; }
		//[Export] public Vector2 MaxSize { get; protected set; } = new(3840, 2160);

		public Form()
		{
			formSize = (Vector2I)this.GetRect().Size;

			MinSize = GetMinimumSize();
			MinimumSizeChanged += () => MinSize = GetMinimumSize();

			_formSizeSaved = true;
		}

		public Form(string title, ResizeMode resizeMode = ResizeMode.fixedAspect)
		{
			Title = title;
			resizable = resizeMode;
		}

		public virtual void OnShow() { }

		public virtual void OnHide() { }
	}
}
