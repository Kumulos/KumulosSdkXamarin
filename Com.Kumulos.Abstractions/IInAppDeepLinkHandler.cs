namespace Com.Kumulos.Abstractions
{
    public interface IInAppDeepLinkHandler
    {
        void Handle(InAppButtonPress buttonPress);
    }
}
