using Sitecore.Jobs.AsyncUI;

namespace UMT.Sitecore.Jobs
{
    public class UMTJobMessage : IMessage
    {
        public string Message { get; set; }

        public UMTJobMessage(string message)
        {
            Message = message;
        }
        
        public void Execute()
        {
        }
    }
}