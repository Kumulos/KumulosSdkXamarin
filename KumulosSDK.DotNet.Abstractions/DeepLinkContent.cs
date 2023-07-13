namespace KumulosSDK.DotNet.Abstractions
{
    public class DeepLinkContent
    {
        public DeepLinkContent(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; }
        public string Description { get; }
    }
}
