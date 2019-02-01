using System;
using ObjCRuntime;

namespace Com.Kumulos.iOS {
    [Native]
    public enum KSTargetType : long
    {
        NotOverridden,
        Debug,
        Release
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
}