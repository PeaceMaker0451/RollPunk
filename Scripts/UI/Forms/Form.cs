using Godot;
using RollPunk.UI.Frames;
using System;

namespace RollPunk.UI.Forms
{
	/// <summary>
	/// Базовый класс формы UI. Форма — это самодостаточное окно с содержимым.
	///
	/// Через методы формы делаются все операции над ней как над окном:
	/// Close, MoveToNewWindow, MoveToMainTab, SetTitle.
	/// Через события формы отслеживается её жизненный цикл: Shown, Closed, LocationChanged.
	///
	/// Содержимое формы (данные, реакция на действия пользователя) — задача
	/// конкретной формы и/или её презентера/контроллера.
	/// </summary>
	public partial class Form : Control
	{
		[Export] public string Title { get; protected set; } = string.Empty;

		/// <summary>Открыта ли форма (принадлежит какому-то контейнеру).</summary>
		public bool IsOpen { get; internal set; }

		/// <summary>Где именно показана форма в данный момент.</summary>
		public FormLocation Location { get; internal set; } = FormLocation.Closed;

		/// <summary>Возникает после того, как форма показана в контейнере.</summary>
		public event Action Shown;

		/// <summary>Возникает после того, как форма закрыта (удалена из всех контейнеров).</summary>
		public event Action Closed;

		/// <summary>Возникает после смены Location (например, MainTab -> NewWindow).</summary>
		public event Action LocationChanged;

		/// <summary>Ссылка на менеджер форм. Устанавливается менеджером при первом показе.</summary>
		internal IFormHost Host { get; set; }

		/// <summary>Фрейм, содержащий форму, если она показана в отдельном окне. Иначе null.</summary>
		internal Frame ContainingFrame => Host?.GetContainingFrame(this);

		public Form() { }
		
		public Form(string title)
		{
			Title = title;
		}

		/// <summary>Закрывает форму и освобождает ресурсы.</summary>
		public void Close()
		{
			if (!IsOpen) return;
			EnsureHost();
			Host.RequestClose(this);
		}

		/// <summary>Перемещает форму в отдельное окно.</summary>
		public void MoveToNewWindow()
		{
			EnsureHost();
			if (Location == FormLocation.NewWindow) return;
			Host.RequestMoveToNewWindow(this);
		}

		/// <summary>Перемещает форму во вкладку главного фрейма.</summary>
		public void MoveToMainTab(int priority = 0)
		{
			EnsureHost();
			if (Location == FormLocation.MainTab) return;
			Host.RequestMoveToMainTab(this, priority);
		}

		/// <summary>Устанавливает заголовок формы.</summary>
		public void SetTitle(string title)
		{
			Title = title ?? string.Empty;
			OnTitleChanged();
		}

		/// <summary>Пытается получить фрейм, содержащий форму. Возвращает false, если форма во вкладке или закрыта.</summary>
		public bool TryGetFrame(out Frame frame)
		{
			frame = ContainingFrame;
			return frame != null;
		}

		/// <summary>Хук: вызывается перед показом (сохранён для совместимости с Frame).</summary>
		public virtual void OnShow() { }

		/// <summary>Хук: вызывается перед скрытием (сохранён для совместимости с Frame).</summary>
		public virtual void OnHide() { }

		/// <summary>Хук для наследников: вызывается после показа формы в контейнере.</summary>
		protected virtual void OnShown() { }

		/// <summary>Хук для наследников: вызывается после закрытия формы.</summary>
		protected virtual void OnClosed() { }

		/// <summary>Хук для наследников: вызывается после смены Location.</summary>
		protected virtual void OnLocationChanged() { }

		/// <summary>Хук для наследников: вызывается после смены Title.</summary>
		protected virtual void OnTitleChanged() { }

		// Вызывается менеджером после того, как форма помещена в контейнер.
		internal void RaiseShown()
		{
			OnShown();
			Shown?.Invoke();
		}

		// Вызывается менеджером после того, как форма удалена из всех контейнеров.
		internal void RaiseClosed()
		{
			OnClosed();
			Closed?.Invoke();
		}

		// Вызывается менеджером после смены Location.
		internal void RaiseLocationChanged()
		{
			OnLocationChanged();
			LocationChanged?.Invoke();
		}

		private void EnsureHost()
		{
			if (Host == null)
				throw new InvalidOperationException(
					$"Form '{GetType().Name}' has no host. " +
					"Open the form via IFormsManager before performing window operations.");
		}
	}
}
