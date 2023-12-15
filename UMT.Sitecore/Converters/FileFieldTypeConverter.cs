using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class FileFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            var mediaField = (FileField)field;
            var result = new TargetFieldValue();
            if (field != null && mediaField.MediaItem != null)
            {
                long.TryParse(mediaField.MediaItem["Size"], out var size);
                var fieldValue = new[]
                {
                    new MediaField
                    {
                        Identifier = mediaField.MediaID.Guid,
                        Name = mediaField.MediaItem.Name,
                        Size = size,
                        Dimensions = new MediaFieldDimensions()
                    }
                };
                result.Value = JsonConvert.SerializeObject(fieldValue);
            }

            return result;
        }
    }
}
