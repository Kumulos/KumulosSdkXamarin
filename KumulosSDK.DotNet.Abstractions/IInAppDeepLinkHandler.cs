namespace KumulosSDK.DotNet.Abstractions
{
    public interface IInAppDeepLinkHandler
    {
        void Handle(InAppButtonPress buttonPress);
    }
}
