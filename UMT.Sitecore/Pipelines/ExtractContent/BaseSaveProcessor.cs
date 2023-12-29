using System.IO;
using Newtonsoft.Json;
using Sitecore;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class BaseSaveProcessor
    {
        protected string CreateFileExtractFolder(string outputFolder)
        {
            var folderPath = MainUtil.MapPath(outputFolder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        protected void SerializeToFile(object data, string fileName)
        {
            using (var file = File.CreateText(MainUtil.MapPath(fileName)))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }
        }
    }
}