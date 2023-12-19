using System;
using log4net;
using Sitecore.Diagnostics;
using UMT.Sitecore.Jobs;

namespace UMT.Sitecore.Diagnostics
{
    public static class UMTLog
    {
        private static readonly ILog Log;

        static UMTLog()
        {
            Log = LogManager.GetLogger("UMT");
        }

        public static void Debug(string message, bool sendToJob = false, Exception exception = null)
        {
            Assert.IsNotNull(Log, "Logger implementation was not initialized");
            Assert.ArgumentNotNull(message, "message");
            if (exception == null)
            {
                Log.Debug(FormatMessage(message));
            }
            else
            {
                Log.Debug(FormatMessage(message), exception);
            }

            if (sendToJob)
            {
                UMTJob.Job?.MessageQueue.PutMessage(new UMTJobMessage(message));
            }
        }

        private static string FormatMessage(string message)
        {
            return message;
        }

        public static void Error(string message, bool sendToJob = false, Exception exception = null)
        {
            Assert.IsNotNull(Log, "Logger implementation was not initialized");
            if (exception == null)
            {
                Log.Error(FormatMessage(message));
            }
            else
            {
                Log.Error(FormatMessage(message), exception);
            }

            if (sendToJob)
            {
                UMTJob.Job?.MessageQueue.PutMessage(new UMTJobMessage(message));
            }
        }

        public static void Fatal(string message, bool sendToJob = false, Exception exception = null)
        {
            Assert.IsNotNull(Log, "Logger implementation was not initialized");
            if (exception == null)
            {
                Log.Fatal(FormatMessage(message));
            }
            else
            {
                Log.Fatal(FormatMessage(message), exception);
            }

            if (sendToJob)
            {
                UMTJob.Job?.MessageQueue.PutMessage(new UMTJobMessage(message));
            }
        }

        public static void Info(string message, bool sendToJob = false, Exception exception = null)
        {
            Assert.IsNotNull(Log, "Logger implementation was not initialized");
            if (exception == null)
            {
                Log.Info(FormatMessage(message));
            }
            else
            {
                Log.Info(FormatMessage(message), exception);
            }

            if (sendToJob)
            {
                UMTJob.Job?.MessageQueue.PutMessage(new UMTJobMessage(message));
            }
        }

        public static void Warn(string message, bool sendToJob = false, Exception exception = null)
        {
            Assert.IsNotNull(Log, "Logger implementation was not initialized");
            if (exception == null)
            {
                Log.Warn(FormatMessage(message));
            }
            else
            {
                Log.Warn(FormatMessage(message), exception);
            }

            if (sendToJob)
            {
                UMTJob.Job?.MessageQueue.PutMessage(new UMTJobMessage(message));
            }
        }
    }
}
