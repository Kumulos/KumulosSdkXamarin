namespace KumulosSDK.DotNet.Abstractions
{
    public class InAppInboxSummary
    {
        public int Unread { private set; get; }
        public int Total { private set; get; }

        public InAppInboxSummary(int unread, int total)
        {
            Unread = unread;
            Total = total;
        }
    }
}
