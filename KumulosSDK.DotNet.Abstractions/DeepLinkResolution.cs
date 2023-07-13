namespace KumulosSDK.DotNet.Abstractions
{
    public enum DeepLinkResolution : long
    {
        LookupFailed,
        LinkNotFound,
        LinkExpired,
        LinkLimitExceeded,
        LinkMatched
    }
}