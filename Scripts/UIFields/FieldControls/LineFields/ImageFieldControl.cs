using Godot;
using RollPunk.Client;
using RollPunk.UI.ImageUtils;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    internal partial class ImageFieldControl : FieldControl
    {
        [Export] private TextureRect _imageDisplay;
        [Export] private Control _hoverContainer;
        [Export] private Button _loadButton;
        [Export] private Button _clearButton;
        [Export] private TextureRect _defaultImage;

        private ImageField _field;
        private ImageDataConverter _imageConverter;
        private Vector2 _displaySize = new(256, 256);
        private TextureRect.ExpandModeEnum _fitMode = TextureRect.ExpandModeEnum.IgnoreSize;

        public void Initialize(ImageField field)
        {
            InitializeImageConverter();
            UpdateValue();
            SetupUI();

            _field = field;
            UpdateValue();
            AddSubscriptions();
            LoadDisplaySettings();
        }

        public override LineField GetField()
        {
            return _field;
        }

        protected override void SetVisible(bool visible)
        {
            Visible = visible;
        }

        protected override void SetEditable(bool editable)
        {
            _loadButton.Disabled = !editable;
            _clearButton.Disabled = !editable;
        }

        protected override void SetName(string name)
        {
            // ImageFieldControl не имеет видимого имени в UI
        }

        protected override void UpdateValue()
        {
            var field = GetField() as ImageField;
            if (field?.ImageData != null && field.ImageData.Length > 0)
            {
                var texture = _imageConverter.BytesToTexture(field.ImageData);
                _imageDisplay.Texture = texture;
                _defaultImage.Visible = false;
            }
            else
            {
                _imageDisplay.Texture = null;
                _defaultImage.Visible = true;
            }
        }

        protected override void OnAdditionalDataChanged(string dataName)
        {
            LoadDisplaySettings();
        }

        private void InitializeImageConverter()
        {
            var validator = new ImageValidator(
                ClientConfig.ImageSettings.MaxFileSizeBytes,
                ClientConfig.ImageSettings.MaxValidationDimension
            );
            var resizer = new ImageResizer(ClientConfig.ImageSettings.MaxDimension);
            _imageConverter = new ImageDataConverter(validator, resizer);
        }

        private void SetupUI()
        {
            _loadButton.Pressed += OnLoadButtonPressed;
            _clearButton.Pressed += OnClearButtonPressed;
            _imageDisplay.MouseEntered += OnImageMouseEntered;
            _imageDisplay.MouseExited += OnImageMouseExited;
            _hoverContainer.Visible = false;
        }

        private void LoadDisplaySettings()
        {
            var field = GetField();
            if (field?.AdditionalData == null)
                return;

            // Загружаем размер отображения
            if (field.AdditionalData.TryGetValue("display_width", out var widthObj) &&
                field.AdditionalData.TryGetValue("display_height", out var heightObj))
            {
                if (TryParseFloat(widthObj, out float width) && TryParseFloat(heightObj, out float height))
                {
                    _displaySize = new Vector2(width, height);
                    _imageDisplay.CustomMinimumSize = _displaySize;
                }
            }

            // Загружаем режим вписывания
            if (field.AdditionalData.TryGetValue("fit_mode", out var fitModeObj))
            {
                if (fitModeObj is string fitModeStr)
                {
                    _fitMode = fitModeStr.ToLower() switch
                    {
                        "contain" => TextureRect.ExpandModeEnum.IgnoreSize,
                        "cover" => TextureRect.ExpandModeEnum.FitHeight,
                        "stretch" => TextureRect.ExpandModeEnum.FitWidth,
                        _ => TextureRect.ExpandModeEnum.IgnoreSize
                    };
                    _imageDisplay.ExpandMode = _fitMode;
                }
            }
        }

        private bool TryParseFloat(object obj, out float result)
        {
            result = 0;
            if (obj == null)
                return false;

            if (obj is float f)
            {
                result = f;
                return true;
            }

            if (obj is double d)
            {
                result = (float)d;
                return true;
            }

            if (obj is int i)
            {
                result = i;
                return true;
            }

            if (obj is string s && float.TryParse(s, out float parsed))
            {
                result = parsed;
                return true;
            }

            return false;
        }

        private void OnLoadButtonPressed()
        {
            var dialog = new FileDialog
            {
                FileMode = FileDialog.FileModeEnum.OpenFile,
                Access = FileDialog.AccessEnum.Filesystem,
                UseNativeDialog = true,
                Filters = new[] { "*.png ; PNG Images", "*.jpg, *.jpeg ; JPEG Images", "*.bmp ; BMP Images" }
            };

            dialog.FileSelected += (path) =>
            {
                OnImageFileSelected(path);
                dialog.QueueFree();
            };

            GetTree().Root.AddChild(dialog);
            dialog.PopupCentered(new Vector2I(800, 600));
        }

        private void OnImageFileSelected(string paths)
        {
            if (paths.Length == 0)
                return;

            string filePath = paths;
            var imageData = _imageConverter.LoadImageFileToBytes(filePath, out string errorMessage);

            if (imageData != null)
            {
                (GetField() as ImageField).SetImageData(imageData);
            }
            else
            {
                GD.PrintErr($"Ошибка при загрузке изображения: {errorMessage}");
            }
        }

        private void OnClearButtonPressed()
        {
            (GetField() as ImageField).ClearImageData();
        }

        private void OnImageMouseEntered()
        {
            _hoverContainer.Visible = true;
        }

        private void OnImageMouseExited()
        {
            _hoverContainer.Visible = false;
        }
    }
}
