﻿using System;
using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class ExtractContentArgs : PipelineArgs
    {
        public string OutputFolderPath { get; set; }
        public string NameSpace { get; set; }
        public List<string> ContentPaths { get; set; }
        public List<string> MediaPaths { get; set; }
        public ChannelMap SourceChannel { get; set; }
        public TargetChannel TargetChannel { get; set; }
        public List<Language> SourceLanguages { get; set; }
        public List<ContentLanguage> TargetLanguages { get; set; }
        public List<Item> SourceItems { get; set; }
        public Dictionary<string, TargetItem> TargetItems { get; set; }
        public Dictionary<Guid, Template> SourceTemplates { get; set; }
        public Dictionary<Guid, TargetContentType> TargetTemplates { get; set; }
        public MediaMap SourceMediaLibrary { get; set; }
        public MediaLibrary TargetMediaLibrary { get; set; }
        public List<MediaItem> SourceMediaItems { get; set; }
        public List<MediaFile> TargetMediaItems { get; set; }
    }
}