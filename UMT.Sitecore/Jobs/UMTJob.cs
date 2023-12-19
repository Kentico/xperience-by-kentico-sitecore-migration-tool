using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Pipelines;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Pipelines.ExtractContent;

namespace UMT.Sitecore.Jobs
{
    public class UMTJob
    {
        private const string JobName = "UMT Extract Job";
        private static readonly TimeSpan JobAfterLife = new TimeSpan(0, 2, 0);
        public static Job Job => JobManager.GetJob(JobName);
        
        public void StartJob(string nameSpace, ChannelMap channel, List<string> contentPaths, List<Language> languages)
        {
            var options = new JobOptions(JobName, "ContentEditor", Context.Site.Name, this, "Run",
                new object[] { nameSpace, channel, contentPaths, languages });
            var job = JobManager.Start(options);
            job.Options.AfterLife = JobAfterLife;
        }

        public void Run(string nameSpace, ChannelMap channel, List<string> contentPaths, List<Language> languages)
        {
            var itemsArgs = new ExtractContentArgs
            {
                NameSpace = nameSpace,
                SourceChannel = channel,
                ContentPaths = contentPaths,
                //MediaPaths = new List<string> { MediaPaths.Text },
                SourceLanguages = languages,
                //SourceMediaLibrary = sourceMediaLibrary
            };
            CorePipeline.Run("umt.ExtractContent", itemsArgs);
        }
    }
}