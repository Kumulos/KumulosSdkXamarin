using ObjCRuntime;

namespace Com.Kumulos.iOS
{
    [Native]
    public enum KSDeepLinkResolution : long
    {
        ookupFailed,
        inkNotFound,
        inkExpired,
        inkLimitExceeded,
        inkMatched
    }

    [Native]
    public enum KSTargetType : long
    {
        NotOverridden,
        Debug,
        Release
    }

    [Native]
    public enum KSInAppConsentStrategy : long
    {
        NotEnabled,
        AutoEnroll,
        ExplicitByUser
    }

    [Native]
    public enum KSInAppMessagePresentationResult : long
    {
        Presented,
        Expired,
        Failed
    }
}