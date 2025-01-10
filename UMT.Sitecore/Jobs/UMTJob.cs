using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Abstractions;
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
        private const string PipelineName = "umt.ExtractContent";
        private static readonly TimeSpan JobAfterLife = new TimeSpan(0, 2, 0);
        public static BaseJob Job => JobManager.GetJob(JobName);
        
        public void StartJob(string nameSpace, ChannelMap channel, List<string> contentPaths, List<Language> languages, List<string> mediaPaths)
        {
            var options = new DefaultJobOptions(JobName, "UMT", Context.Site.Name, this, "Run",
                new object[] { nameSpace, channel, contentPaths, languages, mediaPaths });
            var job = JobManager.Start(options);
            job.Options.AfterLife = JobAfterLife;
        }

        public void Run(string nameSpace, ChannelMap channel, List<string> contentPaths, List<Language> languages, List<string> mediaPaths)
        {
            var itemsArgs = new ExtractContentArgs
            {
                NameSpace = nameSpace,
                SourceChannel = channel,
                ContentPaths = contentPaths,
                MediaPaths = mediaPaths,
                SourceLanguages = languages
            };
            CorePipeline.Run(PipelineName, itemsArgs);
        }

        public static void IncreaseTotalItems(int count = 1)
        {
            var job = Job;
            if (job != null)
            {
                if (job.Status.Total < 0)
                {
                    job.Status.Total = count;
                }
                else
                {
                    job.Status.Total += count;
                }
            }
        }
        
        public static void IncreaseProcessedItems(int count = 1)
        {
            var job = Job;
            if (job != null)
            {
                if (job.Status.Processed < 0)
                {
                    job.Status.Processed = count;
                }
                else
                {
                    job.Status.Processed += count;
                }
            }
        }
    }
}