using ObjCRuntime;

namespace Com.Kumulos.iOS
{
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
    public enum KSErrorCode : long
    {
        NetworkError,
        RpcError,
        UnknownError,
        ValidationError,
        HttpBadStatus
    }

    [Native]
    public enum KSInAppMessagePresentationResult : long
    {
        Presented,
        Expired,
        Failed
    }
}