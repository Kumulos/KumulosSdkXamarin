namespace Com.Kumulos.Abstractions
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