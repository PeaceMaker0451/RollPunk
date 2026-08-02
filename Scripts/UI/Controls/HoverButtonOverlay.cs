using Godot;

namespace RollPunk.UI.Controls
{
    /// <summary>
    /// Компонент для отображения кнопок при наведении на элемент
    /// </summary>
    public partial class HoverButtonOverlay : Control
    {
        [Export] public Color HoverBackgroundColor = new Color(0, 0, 0, 0.5f);
        [Export] public float AnimationSpeed = 0.3f;

        private float _currentAlpha = 0;
        private bool _isHovering = false;

        public override void _Ready()
        {
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
            Modulate = new Color(1, 1, 1, 0);
        }

        public override void _Process(double delta)
        {
            float targetAlpha = _isHovering ? 1 : 0;
            _currentAlpha = Mathf.Lerp(_currentAlpha, targetAlpha, (float)delta / AnimationSpeed);

            var modulate = Modulate;
            modulate.A = _currentAlpha;
            Modulate = modulate;
        }

        private void OnMouseEntered()
        {
            _isHovering = true;
        }

        private void OnMouseExited()
        {
            _isHovering = false;
        }
    }
}
