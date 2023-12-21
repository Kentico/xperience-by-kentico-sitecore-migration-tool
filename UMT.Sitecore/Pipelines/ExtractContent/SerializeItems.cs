using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Diagnostics;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SerializeItems
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor started");

            SaveSerializedLanguages(args.TargetLanguages, args.OutputFolderPath);
            SaveSerializedChannel(args.TargetChannel, args.OutputFolderPath);
            SaveSerializedContentItems(args.TargetItems, args.OutputFolderPath);
            SaveSerializedWebPages(args.TargetItems, args.OutputFolderPath);
            SaveSerializedMediaLibrary(args.TargetMediaLibrary, args.OutputFolderPath);
            SaveSerializedMediaItems(args.TargetMediaItems, args.OutputFolderPath);

            UMTLog.Info($"{nameof(SerializeItems)} pipeline processor finished");
        }

        protected virtual void SaveSerializedLanguages(List<ContentLanguage> languages, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/01.Configuration");
            var fileName = MainUtil.MapPath($"{folderPath}/01.Languages.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, languages);
            }
        }

        protected virtual void SaveSerializedChannel(TargetChannel channel, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/01.Configuration");
            var fileName = MainUtil.MapPath($"{folderPath}/02.Channel.{channel.Name}.{channel.Id:D}.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, channel.Elements);
            }
        }

        protected virtual void SaveSerializedContentItems(Dictionary<string, TargetItem> items, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/04.ContentItems");
            foreach (var item in items.Where(x => !x.Value.IsWebPage))
            {
                using (var file = File.CreateText(GenerateFileName(item.Value, folderPath)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, item.Value.Elements);
                }
            }
        }

        protected virtual void SaveSerializedWebPages(Dictionary<string, TargetItem> items, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/05.WebPages");
            foreach (var item in items.Where(x => x.Value.IsWebPage))
            {
                using (var file = File.CreateText(GenerateFileName(item.Value, folderPath)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, item.Value.Elements);
                }
            }
        }

        protected virtual string GenerateFileName(TargetItem item, string outputFolderPath)
        {
            return MainUtil.MapPath($"{outputFolderPath}/{item.DepthLevel:0000}.{item.Name}.{item.Id:D}.json");
        }
        
        protected virtual void SaveSerializedMediaLibrary(MediaLibrary mediaLibrary, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/03.Media");
            var fileName = MainUtil.MapPath(folderPath + $"/MediaLibrary.json");
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new[] { mediaLibrary });
            }
        }

        protected virtual void SaveSerializedMediaItems(IList<MediaFile> mediaItems, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/03.Media");
            foreach (var mediaItem in mediaItems)
            {
                var fileName = MainUtil.MapPath($"{folderPath}/{mediaItem.FileName}.{mediaItem.FileGUID:D}.json");
                using (var file = File.CreateText(fileName))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, new[] { mediaItem });
                }
            }
        }
        
        protected virtual string CreateFileExtractFolder(string outputFolder)
        {
            var folderPath = MainUtil.MapPath(outputFolder);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }
    }
}