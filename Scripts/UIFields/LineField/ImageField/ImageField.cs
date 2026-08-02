using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("Image")]
    public sealed class ImageField : LineField
    {
        public event Action ImageDataChanged;

        public byte[] ImageData { get; private set; }

        public ImageField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAccessLevel, byte[] imageData = null, int linePriority = 0, Dictionary<string, object> additionalData = null)
            : base(name, visibleName, viewAccessLevel, editAccessLevel, typeof(ImageFieldAPI), linePriority, additionalData)
        {
            ImageData = imageData;
        }

        public ImageField(EntityState fieldData) : base(fieldData, typeof(ImageFieldAPI)) { }

        public override object GetRawValue()
        {
            return ImageData;
        }

        public void SetImageData(byte[] imageData)
        {
            if (imageData == ImageData)
                return;

            ImageData = imageData;
            ImageDataChanged?.Invoke();
            RaiseValueChanged();
        }

        public void ClearImageData()
        {
            SetImageData(null);
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            var imageDataToken = payload.GetValueOrDefault(nameof(ImageData));
            if (imageDataToken != null && imageDataToken.Type == JTokenType.String)
            {
                try
                {
                    string base64String = imageDataToken.Value<string>();
                    ImageData = Convert.FromBase64String(base64String);
                }
                catch
                {
                    ImageData = null;
                }
            }
            else
            {
                ImageData = null;
            }
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            if (ImageData != null && ImageData.Length > 0)
            {
                string base64String = Convert.ToBase64String(ImageData);
                payload.Add(nameof(ImageData), base64String);
            }
            else
            {
                payload.Add(nameof(ImageData), null);
            }
        }
    }
}
