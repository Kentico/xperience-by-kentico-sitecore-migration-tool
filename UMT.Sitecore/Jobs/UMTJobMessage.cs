using Sitecore.Jobs.AsyncUI;

namespace UMT.Sitecore.Jobs
{
    public class UMTJobMessage : IMessage
    {
        public string Message { get; set; }
        
        public UMTJobManualCheck ManualCheck { get; set; }

        public bool IsManualCheck => ManualCheck != null;

        public UMTJobMessage(string message)
        {
            Message = message;
        }
        
        public UMTJobMessage(UMTJobManualCheck manualCheck)
        {
            ManualCheck = manualCheck;
        }
        
        public void Execute()
        {
        }
    }
}