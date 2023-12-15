using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class ImageFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            var mediaField = (ImageField)field;
            var result = new TargetFieldValue();
            if (field != null && mediaField.MediaItem != null)
            {
                long.TryParse(mediaField.MediaItem["Size"], out var size);
                int.TryParse(mediaField.MediaItem["Width"], out var width);
                int.TryParse(mediaField.MediaItem["Height"], out var height);
                var fieldValue = new[]
                {
                    new MediaField
                    {
                        Identifier = mediaField.MediaID.Guid,
                        Name = mediaField.MediaItem.Name,
                        Size = size,
                        Dimensions = new MediaFieldDimensions
                        {
                            Width = width,
                            Height = height
                        }
                    }
                };
                result.Value = JsonConvert.SerializeObject(fieldValue);
            }

            return result;
        }
    }
}
