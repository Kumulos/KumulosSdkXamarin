using System;
using Foundation;
using ObjCRuntime;
using CoreLocation;

namespace Com.Kumulos.iOS
{

    // typedef void (^ _Nullable)(KSAPIResponse * _Nonnull, KSAPIOperation * _Nonnull) KSAPIOperationSuccessBlock;
    delegate void KSAPIOperationSuccessBlock(KSAPIResponse arg0, KSAPIOperation arg1);

    // typedef void (^ _Nullable)(NSError * _Nonnull, KSAPIOperation * _Nonnull) KSAPIOperationFailureBlock;
    delegate void KSAPIOperationFailureBlock(NSError arg0, KSAPIOperation arg1);

    // @interface KSConfig : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface KSConfig
    {
        // @property (readonly, nonatomic) NSString * _Nonnull apiKey;
        [Export("apiKey")]
        string ApiKey { get; }

        // @property (readonly, nonatomic) NSString * _Nonnull secretKey;
        [Export("secretKey")]
        string SecretKey { get; }

        // @property (readonly, nonatomic) BOOL crashReportingEnabled;
        [Export("crashReportingEnabled")]
        bool CrashReportingEnabled { get; }

        // @property (readonly, nonatomic) NSUInteger sessionIdleTimeoutSeconds;
        [Export("sessionIdleTimeoutSeconds")]
        nuint SessionIdleTimeoutSeconds { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nullable runtimeInfo;
        [NullAllowed, Export("runtimeInfo")]
        NSDictionary RuntimeInfo { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nullable sdkInfo;
        [NullAllowed, Export("sdkInfo")]
        NSDictionary SdkInfo { get; }

        // @property (readonly, nonatomic) KSTargetType targetType;
        [Export("targetType")]
        KSTargetType TargetType { get; }

        // +(instancetype _Nullable)configWithAPIKey:(NSString * _Nonnull)APIKey andSecretKey:(NSString * _Nonnull)secretKey;
        [Static]
        [Export("configWithAPIKey:andSecretKey:")]
        [return: NullAllowed]
        KSConfig ConfigWithAPIKey(string APIKey, string secretKey);

        // -(instancetype _Nonnull)enableCrashReporting;
        [Export("enableCrashReporting")]
        KSConfig EnableCrashReporting();

        // -(instancetype _Nonnull)setSessionIdleTimeout:(NSUInteger)timeoutSeconds;
        [Export("setSessionIdleTimeout:")]
        KSConfig SetSessionIdleTimeout(nuint timeoutSeconds);

        // -(instancetype _Nonnull)setRuntimeInfo:(NSDictionary * _Nonnull)info;
        [Export("setRuntimeInfo:")]
        KSConfig SetRuntimeInfo(NSDictionary info);

        // -(instancetype _Nonnull)setSdkInfo:(NSDictionary * _Nonnull)info;
        [Export("setSdkInfo:")]
        KSConfig SetSdkInfo(NSDictionary info);

        // -(instancetype _Nonnull)setTargetType:(KSTargetType)type;
        [Export("setTargetType:")]
        KSConfig SetTargetType(KSTargetType type);
    }

    // @interface Kumulos : NSObject
    [BaseType(typeof(NSObject))]
    interface Kumulos
    {
        // @property (readonly, nonatomic, class) Kumulos * _Nullable shared;
        [Static]
        [NullAllowed, Export("shared")]
        Kumulos Shared { get; }

        // @property NSString * _Nonnull sessionToken;
        [Export("sessionToken")]
        string SessionToken { get; set; }

        // @property KSConfig * _Nonnull config;
        [Export("config", ArgumentSemantic.Assign)]
        KSConfig Config { get; set; }

        // +(NSString * _Nonnull)installId;
        [Static]
        [Export("installId")]
        //[Verify(MethodToProperty)]
        string InstallId { get; }

        // +(instancetype _Nullable)initializeWithConfig:(KSConfig * _Nonnull)config;
        [Static]
        [Export("initializeWithConfig:")]
        [return: NullAllowed]
        Kumulos InitializeWithConfig(KSConfig config);

        // -(instancetype _Nullable)initWithConfig:(KSConfig * _Nonnull)config;
        [Export("initWithConfig:")]
        IntPtr Constructor(KSConfig config);

        // -(instancetype _Nullable)initWithAPIKey:(NSString * _Nonnull)APIKey andSecretKey:(NSString * _Nonnull)secretKey;
        [Export("initWithAPIKey:andSecretKey:")]
        IntPtr Constructor(string APIKey, string secretKey);

        // -(KSAPIOperation * _Nonnull)callMethod:(NSString * _Nonnull)method withSuccess:(KSAPIOperationSuccessBlock)success andFailure:(KSAPIOperationFailureBlock)failure;
        [Export("callMethod:withSuccess:andFailure:")]
        KSAPIOperation CallMethod(string method, [NullAllowed] KSAPIOperationSuccessBlock success, [NullAllowed] KSAPIOperationFailureBlock failure);

        // -(KSAPIOperation * _Nonnull)callMethod:(NSString * _Nonnull)method withParams:(NSDictionary * _Nullable)params success:(KSAPIOperationSuccessBlock)success andFailure:(KSAPIOperationFailureBlock)failure;
        [Export("callMethod:withParams:success:andFailure:")]
        KSAPIOperation CallMethod(string method, [NullAllowed] NSDictionary @params, [NullAllowed] KSAPIOperationSuccessBlock success, [NullAllowed] KSAPIOperationFailureBlock failure);

        // -(KSAPIOperation * _Nonnull)callMethod:(NSString * _Nonnull)method withDelegate:(id<KSAPIOperationDelegate> _Nullable)delegate;
        [Export("callMethod:withDelegate:")]
        KSAPIOperation CallMethod(string method, [NullAllowed] KSAPIOperationDelegate @delegate);

        // -(KSAPIOperation * _Nonnull)callMethod:(NSString * _Nonnull)method withParams:(NSDictionary * _Nullable)params andDelegate:(id<KSAPIOperationDelegate> _Nullable)delegate;
        [Export("callMethod:withParams:andDelegate:")]
        KSAPIOperation CallMethod(string method, [NullAllowed] NSDictionary @params, [NullAllowed] KSAPIOperationDelegate @delegate);
    }

    // @interface Push (Kumulos)
    [Category]
    [BaseType(typeof(Kumulos))]
    interface Kumulos_Push
    {
        // -(void)pushRequestDeviceToken;
        [Export("pushRequestDeviceToken")]
        void PushRequestDeviceToken();

        // -(void)pushRegisterWithDeviceToken:(NSData * _Nonnull)deviceToken;
        [Export("pushRegisterWithDeviceToken:")]
        void PushRegisterWithDeviceToken(NSData deviceToken);

        // -(void)pushTrackOpenFromNotification:(NSDictionary * _Nullable)userInfo;
        [Export("pushTrackOpenFromNotification:")]
        void PushTrackOpenFromNotification([NullAllowed] NSDictionary userInfo);
    }

    // @interface KSPushChannel : NSObject
    [BaseType(typeof(NSObject))]
    interface KSPushChannel
    {
        // @property (readonly, nonatomic) NSString * _Nonnull uuid;
        [Export("uuid")]
        string Uuid { get; }

        // @property (readonly, nonatomic) NSString * _Nullable name;
        [NullAllowed, Export("name")]
        string Name { get; }

        // @property (readonly, nonatomic) BOOL isSubscribed;
        [Export("isSubscribed")]
        bool IsSubscribed { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nullable meta;
        [NullAllowed, Export("meta")]
        NSDictionary Meta { get; }
    }

    // typedef void (^ _Nullable)(NSError * _Nullable) KSPushSubscriptionCompletionBlock;
    delegate void KSPushSubscriptionCompletionBlock([NullAllowed] NSError arg0);

    // typedef void (^ _Nonnull)(NSError * _Nullable, NSArray<KSPushChannel *> * _Nullable) KSPushChannelsSuccessBlock;
    delegate void KSPushChannelsSuccessBlock([NullAllowed] NSError arg0, [NullAllowed] KSPushChannel[] arg1);

    // typedef void (^ _Nonnull)(NSError * _Nullable, KSPushChannel * _Nullable) KSPushChannelSuccessBlock;
    delegate void KSPushChannelSuccessBlock([NullAllowed] NSError arg0, [NullAllowed] KSPushChannel arg1);

    // @interface KumulosPushSubscriptionManager : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface KumulosPushSubscriptionManager
    {
        // -(instancetype _Nonnull)initWithKumulos:(Kumulos * _Nonnull)client;
        [Export("initWithKumulos:")]
        IntPtr Constructor(Kumulos client);

        // -(void)subscribeToChannels:(NSArray<NSString *> * _Nonnull)uuids;
        [Export("subscribeToChannels:")]
        void SubscribeToChannels(string[] uuids);

        // -(void)subscribeToChannels:(NSArray<NSString *> * _Nonnull)uuids onComplete:(KSPushSubscriptionCompletionBlock)complete;
        [Export("subscribeToChannels:onComplete:")]
        void SubscribeToChannels(string[] uuids, [NullAllowed] KSPushSubscriptionCompletionBlock complete);

        // -(void)unsubscribeFromChannels:(NSArray<NSString *> * _Nonnull)uuids;
        [Export("unsubscribeFromChannels:")]
        void UnsubscribeFromChannels(string[] uuids);

        // -(void)unsubscribeFromChannels:(NSArray<NSString *> * _Nonnull)uuids onComplete:(KSPushSubscriptionCompletionBlock)complete;
        [Export("unsubscribeFromChannels:onComplete:")]
        void UnsubscribeFromChannels(string[] uuids, [NullAllowed] KSPushSubscriptionCompletionBlock complete);

        // -(void)setSubscriptions:(NSArray<NSString *> * _Nonnull)uuids;
        [Export("setSubscriptions:")]
        void SetSubscriptions(string[] uuids);

        // -(void)setSubscriptions:(NSArray<NSString *> * _Nonnull)uuids onComplete:(KSPushSubscriptionCompletionBlock)complete;
        [Export("setSubscriptions:onComplete:")]
        void SetSubscriptions(string[] uuids, [NullAllowed] KSPushSubscriptionCompletionBlock complete);

        // -(void)clearSubscriptions;
        [Export("clearSubscriptions")]
        void ClearSubscriptions();

        // -(void)clearSubscriptions:(KSPushSubscriptionCompletionBlock)complete;
        [Export("clearSubscriptions:")]
        void ClearSubscriptions([NullAllowed] KSPushSubscriptionCompletionBlock complete);

        // -(void)listChannels:(KSPushChannelsSuccessBlock)complete;
        [Export("listChannels:")]
        void ListChannels(KSPushChannelsSuccessBlock complete);

        // -(void)createChannelWithUuid:(NSString * _Nonnull)uuid shouldSubscribe:(BOOL)subscribe name:(NSString * _Nullable)name showInPortal:(BOOL)shownInPortal andMeta:(NSDictionary * _Nullable)meta onComplete:(KSPushChannelSuccessBlock)complete;
        [Export("createChannelWithUuid:shouldSubscribe:name:showInPortal:andMeta:onComplete:")]
        void CreateChannelWithUuid(string uuid, bool subscribe, [NullAllowed] string name, bool shownInPortal, [NullAllowed] NSDictionary meta, KSPushChannelSuccessBlock complete);
    }

    // @interface Location (Kumulos)
    [Category]
    [BaseType(typeof(Kumulos))]
    interface Kumulos_Location
    {
        // -(void)sendLocationUpdate:(CLLocation * _Nullable)location;
        [Export("sendLocationUpdate:")]
        void SendLocationUpdate([NullAllowed] CLLocation location);

        // -(void)sendiBeaconProximity:(CLBeacon * _Nullable)beacon;
        [Export("sendiBeaconProximity:")]
        void SendiBeaconProximity([NullAllowed] CLBeacon beacon);
    }

    // @interface Analytics (Kumulos)
    [Category]
    [BaseType(typeof(Kumulos))]
    interface Kumulos_Analytics
    {
        // -(void)trackEvent:(NSString * _Nonnull)eventType withProperties:(NSDictionary * _Nullable)properties;
        [Export("trackEvent:withProperties:")]
        void TrackEvent(string eventType, [NullAllowed] NSDictionary properties);

        // -(void)trackEventImmediately:(NSString * _Nonnull)eventType withProperties:(NSDictionary * _Nullable)properties;
        [Export("trackEventImmediately:withProperties:")]
        void TrackEventImmediately(string eventType, [NullAllowed] NSDictionary properties);

        // -(void)associateUserWithInstall:(NSString * _Nonnull)userIdentifier;
        [Export("associateUserWithInstall:")]
        void AssociateUserWithInstall(string userIdentifier);

        // -(void)associateUserWithInstall:(NSString * _Nonnull)userIdentifier attributes:(NSDictionary * _Nonnull)attributes;
        [Export("associateUserWithInstall:attributes:")]
        void AssociateUserWithInstall(string userIdentifier, NSDictionary attributes);
    }

    // @protocol KSAPIOperationDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface KSAPIOperationDelegate
    {
        // @required -(void)operation:(KSAPIOperation * _Nonnull)operation didCompleteWithResponse:(KSAPIResponse * _Nonnull)response;
        [Abstract]
        [Export("operation:didCompleteWithResponse:")]
        void DidCompleteWithResponse(KSAPIOperation operation, KSAPIResponse response);

        // @optional -(void)operation:(KSAPIOperation * _Nonnull)operation didFailWithError:(NSError * _Nonnull)error;
        [Export("operation:didFailWithError:")]
        void DidFailWithError(KSAPIOperation operation, NSError error);
    }

    // @interface KSAPIOperation : NSOperation
    [BaseType(typeof(NSOperation))]
    interface KSAPIOperation
    {
        // @property (readonly, nonatomic) NSString * _Nonnull method;
        [Export("method")]
        string Method { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nullable params;
        [NullAllowed, Export("params")]
        NSDictionary Params { get; }

        [Wrap("WeakDelegate")]
        [NullAllowed]
        KSAPIOperationDelegate Delegate { get; set; }

        // @property (nonatomic, weak) id<KSAPIOperationDelegate> _Nullable delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; set; }

        // @property (readonly, copy, nonatomic) KSAPIOperationSuccessBlock _Nullable successBlock;
        [NullAllowed, Export("successBlock", ArgumentSemantic.Copy)]
        KSAPIOperationSuccessBlock SuccessBlock { get; }

        // @property (readonly, copy, nonatomic) KSAPIOperationFailureBlock _Nullable failureBlock;
        [NullAllowed, Export("failureBlock", ArgumentSemantic.Copy)]
        KSAPIOperationFailureBlock FailureBlock { get; }

        // -(instancetype _Nonnull)initWithKumulos:(Kumulos * _Nonnull)kumulos method:(NSString * _Nonnull)method params:(NSDictionary * _Nullable)params success:(KSAPIOperationSuccessBlock)successBlock failure:(KSAPIOperationFailureBlock)failureBlock andDelegate:(id<KSAPIOperationDelegate> _Nullable)delegate;
        [Export("initWithKumulos:method:params:success:failure:andDelegate:")]
        IntPtr Constructor(Kumulos kumulos, string method, [NullAllowed] NSDictionary @params, [NullAllowed] KSAPIOperationSuccessBlock successBlock, [NullAllowed] KSAPIOperationFailureBlock failureBlock, [NullAllowed] KSAPIOperationDelegate @delegate);
    }

    // @interface KSAPIResponse : NSObject
    [BaseType(typeof(NSObject))]
    interface KSAPIResponse
    {
        // @property (readonly, nonatomic) id payload;
        [Export("payload")]
        NSObject Payload { get; }

        // @property (readonly, nonatomic) NSNumber * requestProcessingTime;
        [Export("requestProcessingTime")]
        NSNumber RequestProcessingTime { get; }

        // @property (readonly, nonatomic) NSNumber * requestReceivedTime;
        [Export("requestReceivedTime")]
        NSNumber RequestReceivedTime { get; }

        // @property (readonly, nonatomic) NSNumber * responseCode;
        [Export("responseCode")]
        NSNumber ResponseCode { get; }

        // @property (readonly, nonatomic) NSString * responseMessage;
        [Export("responseMessage")]
        string ResponseMessage { get; }

        // @property (readonly, nonatomic) NSNumber * timestamp;
        [Export("timestamp")]
        NSNumber Timestamp { get; }

        // @property (readonly, nonatomic) NSNumber * maxProcessingTime;
        [Export("maxProcessingTime")]
        NSNumber MaxProcessingTime { get; }
    }

    // @interface Crash (Kumulos)
    [Category]
    [BaseType(typeof(Kumulos))]
    interface Kumulos_Crash
    {
        // -(void)logExceptionWithName:(NSString * _Nonnull)name reason:(NSString * _Nonnull)reason language:(NSString * _Nonnull)language lineNumber:(NSString * _Nullable)lineNumber stackTrace:(NSArray<id> * _Nullable)stackTrace loggingAllThreads:(BOOL)loggingAllThreads;
        [Export("logExceptionWithName:reason:language:lineNumber:stackTrace:loggingAllThreads:")]
        void LogExceptionWithName(string name, string reason, string language, [NullAllowed] string lineNumber, [NullAllowed] NSObject[] stackTrace, bool loggingAllThreads);
    }
}