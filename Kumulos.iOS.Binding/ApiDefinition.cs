using System;
using CoreLocation;
using Foundation;
using ObjCRuntime;
using UIKit;
using UserNotifications;

namespace Com.Kumulos.iOS
{
    // typedef void (^ _Nullable)(KSInAppButtonPress * _Nonnull) KSInAppDeepLinkHandlerBlock;
    delegate void KSInAppDeepLinkHandlerBlock(KSInAppButtonPress arg0);

    // typedef void (^ _Nullable)(KSPushNotification * _Nonnull) KSPushOpenedHandlerBlock;
    delegate void KSPushOpenedHandlerBlock(KSPushNotification arg0);

    // typedef void (^ _Nullable)(KSDeepLinkResolution, NSUrl * _Nonnull, KSDeepLink * _Nullable) KSDeepLinkHandlerBlock;
    delegate void KSDeepLinkHandlerBlock(KSDeepLinkResolution arg0, NSUrl arg1, [NullAllowed] KSDeepLink arg2);

    // typedef void (^ _Nonnull)(UNNotificationPresentationOptions) KSPushReceivedInForegroundCompletionHandler;
    delegate void KSPushReceivedInForegroundCompletionHandler(UNNotificationPresentationOptions arg0);

    // typedef void (^ _Nullable)(KSPushNotification * _Nonnull) KSPushReceivedInForegroundHandlerBlock;
    delegate void KSPushReceivedInForegroundHandlerBlock(KSPushNotification arg0);

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

        // @property (readonly, nonatomic) UNNotificationPresentationOptions foregroundPushPresentationOptions __attribute__((availability(ios, introduced=10.0))) __attribute__((availability(macos, introduced=10.14)));
        [Export("foregroundPushPresentationOptions")]
        UNNotificationPresentationOptions ForegroundPushPresentationOptions { get; }

        // @property (readonly, nonatomic) KSInAppConsentStrategy inAppConsentStrategy;
        [Export("inAppConsentStrategy")]
        KSInAppConsentStrategy InAppConsentStrategy { get; }

        // @property (readonly, nonatomic) KSInAppDeepLinkHandlerBlock inAppDeepLinkHandler;
        [NullAllowed, Export("inAppDeepLinkHandler")]
        KSInAppDeepLinkHandlerBlock InAppDeepLinkHandler { get; }

        // @property (readonly, nonatomic) KSPushOpenedHandlerBlock pushOpenedHandler;
        [NullAllowed, Export("pushOpenedHandler")]
        KSPushOpenedHandlerBlock PushOpenedHandler { get; }

        // @property (readonly, nonatomic) KSPushReceivedInForegroundHandlerBlock pushReceivedInForegroundHandler __attribute__((availability(ios, introduced=10.0))) __attribute__((availability(macos, introduced=10.14)));
        [NullAllowed, Export("pushReceivedInForegroundHandler")]
        KSPushReceivedInForegroundHandlerBlock PushReceivedInForegroundHandler { get; }

        // @property (readonly, nonatomic) KSDeepLinkHandlerBlock _Nullable deepLinkHandler;
        [NullAllowed, Export("deepLinkHandler")]
        KSDeepLinkHandlerBlock DeepLinkHandler { get; }

        // @property (readonly, nonatomic) NSUrl * _Nullable deepLinkCname;
        [NullAllowed, Export("deepLinkCname")]
        NSUrl DeepLinkCname { get; }

        // +(instancetype _Nullable)configWithAPIKey:(NSString * _Nonnull)APIKey andSecretKey:(NSString * _Nonnull)secretKey;
        [Static]
        [Export("configWithAPIKey:andSecretKey:")]
        [return: NullAllowed]
        KSConfig ConfigWithAPIKey(string APIKey, string secretKey);

        // -(instancetype _Nonnull)enableCrashReporting;
        [Export("enableCrashReporting")]
        KSConfig EnableCrashReporting();

        // -(instancetype _Nonnull)enableInAppMessaging:(KSInAppConsentStrategy)consentStrategy;
        [Export("enableInAppMessaging:")]
        KSConfig EnableInAppMessaging(KSInAppConsentStrategy consentStrategy);

        // -(instancetype _Nonnull)setInAppDeepLinkHandler:(KSInAppDeepLinkHandlerBlock)deepLinkHandler;
        [Export("setInAppDeepLinkHandler:")]
        KSConfig SetInAppDeepLinkHandler([NullAllowed] KSInAppDeepLinkHandlerBlock deepLinkHandler);

        // -(instancetype _Nonnull)setPushOpenedHandler:(KSPushOpenedHandlerBlock)notificationHandler;
        [Export("setPushOpenedHandler:")]
        KSConfig SetPushOpenedHandler([NullAllowed] KSPushOpenedHandlerBlock notificationHandler);

        // -(instancetype _Nonnull)setPushReceivedInForegroundHandler:(KSPushReceivedInForegroundHandlerBlock)receivedHandler __attribute__((availability(ios, introduced=10.0))) __attribute__((availability(macos, introduced=10.14)));
        [Mac(10, 14), iOS(10, 0)]
        [Export("setPushReceivedInForegroundHandler:")]
        KSConfig SetPushReceivedInForegroundHandler([NullAllowed] KSPushReceivedInForegroundHandlerBlock receivedHandler);

        // -(instancetype _Nonnull)setForegroundPushPresentationOptions:(UNNotificationPresentationOptions)notificationPresentationOptions __attribute__((availability(ios, introduced=10.0))) __attribute__((availability(macos, introduced=10.14)));
        [Mac(10, 14), iOS(10, 0)]
        [Export("setForegroundPushPresentationOptions:")]
        KSConfig SetForegroundPushPresentationOptions(UNNotificationPresentationOptions notificationPresentationOptions);

        // -(instancetype _Nonnull)enableDeepLinking:(NSString * _Nonnull)cname deepLinkHandler:(KSDeepLinkHandlerBlock)deepLinkHandler;
        [Export("enableDeepLinking:deepLinkHandler:")]
        KSConfig EnableDeepLinking(string cname, [NullAllowed] KSDeepLinkHandlerBlock deepLinkHandler);

        // -(instancetype _Nonnull)enableDeepLinking:(KSDeepLinkHandlerBlock)deepLinkHandler;
        [Export("enableDeepLinking:")]
        KSConfig EnableDeepLinking([NullAllowed] KSDeepLinkHandlerBlock deepLinkHandler);

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
    }

    // @interface DeepLinking (Kumulos)
    [Category]
    [BaseType(typeof(Kumulos))]
    interface Kumulos_DeepLinking
    {
        // +(void)scene:(UIScene * _Nonnull)scene continueUserActivity:(NSUserActivity * _Nonnull)userActivity __attribute__((availability(ios, introduced=13.0)));
        [iOS(13, 0)]
        [Static]
        [Export("scene:continueUserActivity:")]
        void Scene(UIScene scene, NSUserActivity userActivity);

        // +(BOOL)application:(UIApplication * _Nonnull)application continueUserActivity:(NSUserActivity * _Nonnull)userActivity restorationHandler:(void (^ _Nonnull)(NSArray<id<UIUserActivityRestoring>> * _Nonnull))restorationHandler;
        [Static]
        [Export("application:continueUserActivity:restorationHandler:")]
        bool Application(UIApplication application, NSUserActivity userActivity, Action<NSArray<IUIUserActivityRestoring>> restorationHandler);
    }

    // @interface KSDeepLinkContent : NSObject
    [BaseType(typeof(NSObject))]
    interface KSDeepLinkContent
    {
        // @property (nonatomic) NSString * _Nullable title;
        [NullAllowed, Export("title")]
        string Title { get; set; }

        // @property (nonatomic) NSString * _Nullable description;
        [NullAllowed, Export("description")]
        string Description { get; set; }
    }

    // @interface KSDeepLink : NSObject
    [BaseType(typeof(NSObject))]
    interface KSDeepLink
    {
        // @property (nonatomic) NSUrl * _Nonnull url;
        [Export("url", ArgumentSemantic.Assign)]
        NSUrl Url { get; set; }

        // @property (nonatomic) KSDeepLinkContent * _Nonnull content;
        [Export("content", ArgumentSemantic.Assign)]
        KSDeepLinkContent Content { get; set; }

        // @property (nonatomic) NSDictionary * _Nonnull data;
        [Export("data", ArgumentSemantic.Assign)]
        NSDictionary Data { get; set; }

        // -(instancetype _Nullable)init:(NSUrl * _Nonnull)url from:(NSDictionary * _Nullable)jsonData;
        [Export("init:from:")]
        IntPtr Constructor(NSUrl url, [NullAllowed] NSDictionary jsonData);
    }

    // typedef void (^ _Nullable)(UNAuthorizationStatus, NSError * _Nullable) KSUNAuthorizationCheckedHandler;
    delegate void KSUNAuthorizationCheckedHandler(UNAuthorizationStatus arg0, [NullAllowed] NSError arg1);

    // @interface KSPushNotification : NSObject
    [BaseType(typeof(NSObject))]
    interface KSPushNotification
    {
        // +(instancetype _Nullable)fromUserInfo:(NSDictionary * _Nullable)userInfo;
        [Static]
        [Export("fromUserInfo:")]
        [return: NullAllowed]
        KSPushNotification FromUserInfo([NullAllowed] NSDictionary userInfo);

        // +(instancetype _Nullable)fromUserInfo:(NSDictionary * _Nonnull)userInfo withNotificationResponse:(UNNotificationResponse * _Nonnull)response __attribute__((availability(ios, introduced=10.0)));
        [iOS(10, 0)]
        [Static]
        [Export("fromUserInfo:withNotificationResponse:")]
        [return: NullAllowed]
        KSPushNotification FromUserInfo(NSDictionary userInfo, UNNotificationResponse response);

        // @property (readonly, nonatomic) NSNumber * _Nonnull id;
        [Export("id")]
        NSNumber Id { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nonnull aps;
        [Export("aps")]
        NSDictionary Aps { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nonnull data;
        [Export("data")]
        NSDictionary Data { get; }

        // @property (readonly, nonatomic) NSUrl * _Nullable url;
        [NullAllowed, Export("url")]
        NSUrl Url { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nullable inAppDeepLink;
        [NullAllowed, Export("inAppDeepLink")]
        NSDictionary InAppDeepLink { get; }

        // @property (readonly, nonatomic) NSString * _Nullable actionIdentifier;
        [NullAllowed, Export("actionIdentifier")]
        string ActionIdentifier { get; }
    }

    // @interface Push (Kumulos)
    [Category]
    [BaseType(typeof(Kumulos))]
    interface Kumulos_Push
    {
        // -(void)pushRequestDeviceToken;
        [Export("pushRequestDeviceToken")]
        void PushRequestDeviceToken();

        // -(void)pushRequestDeviceToken:(KSUNAuthorizationCheckedHandler)onAuthorizationStatus __attribute__((availability(ios, introduced=10.0)));
        [iOS(10, 0)]
        [Export("pushRequestDeviceToken:")]
        void PushRequestDeviceToken([NullAllowed] KSUNAuthorizationCheckedHandler onAuthorizationStatus);

        // -(void)pushRegisterWithDeviceToken:(NSData * _Nonnull)deviceToken;
        [Export("pushRegisterWithDeviceToken:")]
        void PushRegisterWithDeviceToken(NSData deviceToken);

        // -(void)pushUnregister;
        [Export("pushUnregister")]
        void PushUnregister();

        // -(void)pushTrackOpenFromNotification:(KSPushNotification * _Nullable)notification;
        [Export("pushTrackOpenFromNotification:")]
        void PushTrackOpenFromNotification([NullAllowed] KSPushNotification notification);
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

        // -(void)clearUserAssociation;
        [Export("clearUserAssociation")]
        void ClearUserAssociation();

        // +(NSString * _Nonnull)currentUserIdentifier;
        [Static]
        [Export("currentUserIdentifier")]
        string CurrentUserIdentifier { get; }
    }

    // @interface KSInAppInboxItem : NSObject
    [BaseType(typeof(NSObject))]
    interface KSInAppInboxItem
    {
        // @property (readonly, nonatomic) NSNumber * _Nonnull id;
        [Export("id")]
        NSNumber Id { get; }

        // @property (readonly, nonatomic) NSString * _Nonnull title;
        [Export("title")]
        string Title { get; }

        // @property (readonly, nonatomic) NSString * _Nonnull subtitle;
        [Export("subtitle")]
        string Subtitle { get; }

        // @property (readonly, nonatomic) NSDate * _Nullable availableFrom;
        [NullAllowed, Export("availableFrom")]
        NSDate AvailableFrom { get; }

        // @property (readonly, nonatomic) NSDate * _Nullable availableTo;
        [NullAllowed, Export("availableTo")]
        NSDate AvailableTo { get; }

        // @property (readonly, nonatomic) NSDate * _Nullable dismissedAt;
        [NullAllowed, Export("dismissedAt")]
        NSDate DismissedAt { get; }

        // @property (readonly, nonatomic) NSDate * _Nonnull sentAt;
        [Export("sentAt")]
        NSDate SentAt { get; }

        // @property (readonly, nonatomic) NSDictionary * _Nullable data;
        [NullAllowed, Export("data")]
        NSDictionary Data { get; }

        // -(BOOL)isRead;
        [Export("isRead")]

        bool IsRead { get; }

        // -(NSURL * _Nullable)getImageUrl;
        [Export("getImageUrl")]
        [return: NullAllowed]
        NSUrl GetImageUrl();

        // -(NSURL * _Nullable)getImageUrl:(int)width;
        [Export("getImageUrl:")]
        [return: NullAllowed]
        NSUrl GetImageUrl(int width);
    }

    // @interface InAppInboxSummary : NSObject
    [BaseType(typeof(NSObject))]
    interface InAppInboxSummary
    {
        // @property (readonly, nonatomic) int totalCount;
        [Export("totalCount")]
        int TotalCount { get; }

        // @property (readonly, nonatomic) int unreadCount;
        [Export("unreadCount")]
        int UnreadCount { get; }

        // +(instancetype _Nonnull)init:(int)totalCount unreadCount:(int)unreadCount;
        [Static]
        [Export("init:unreadCount:")]
        InAppInboxSummary Init(int totalCount, int unreadCount);
    }

    // typedef void (^ _Nullable)(void) InboxUpdatedHandlerBlock;
    delegate void InboxUpdatedHandlerBlock();

    // typedef void (^ _Nullable)(InAppInboxSummary * _Nullable) InboxSummaryBlock;
    delegate void InboxSummaryBlock([NullAllowed] InAppInboxSummary arg0);

    // @interface KSInAppButtonPress : NSObject
    [BaseType(typeof(NSObject))]
    interface KSInAppButtonPress
    {
        // @property (readonly, nonatomic, strong) NSDictionary * _Nonnull deepLinkData;
        [Export("deepLinkData", ArgumentSemantic.Strong)]
        NSDictionary DeepLinkData { get; }

        // @property (readonly, nonatomic, strong) NSNumber * _Nonnull messageId;
        [Export("messageId", ArgumentSemantic.Strong)]
        NSNumber MessageId { get; }

        // @property (readonly, nonatomic, strong) NSDictionary * _Nullable messageData;
        [NullAllowed, Export("messageData", ArgumentSemantic.Strong)]
        NSDictionary MessageData { get; }
    }

    // @interface KumulosInApp : NSObject
    [BaseType(typeof(NSObject))]
    interface KumulosInApp
    {
        // +(void)updateConsentForUser:(BOOL)consentGiven;
        [Static]
        [Export("updateConsentForUser:")]
        void UpdateConsentForUser(bool consentGiven);

        // +(NSArray<KSInAppInboxItem *> * _Nonnull)getInboxItems;
        [Static]
        [Export("getInboxItems")]
        KSInAppInboxItem[] InboxItems { get; }

        // +(KSInAppMessagePresentationResult)presentInboxMessage:(KSInAppInboxItem * _Nonnull)item;
        [Static]
        [Export("presentInboxMessage:")]
        KSInAppMessagePresentationResult PresentInboxMessage(KSInAppInboxItem item);

        // +(BOOL)deleteMessageFromInbox:(KSInAppInboxItem * _Nonnull)item;
        [Static]
        [Export("deleteMessageFromInbox:")]
        bool DeleteMessageFromInbox(KSInAppInboxItem item);

        // +(BOOL)markAsRead:(KSInAppInboxItem * _Nonnull)item;
        [Static]
        [Export("markAsRead:")]
        bool MarkAsRead(KSInAppInboxItem item);

        // +(BOOL)markAllInboxItemsAsRead;
        [Static]
        [Export("markAllInboxItemsAsRead")]
        bool MarkAllInboxItemsAsRead { get; }

        // +(void)setOnInboxUpdated:(InboxUpdatedHandlerBlock)inboxUpdatedHandlerBlock;
        [Static]
        [Export("setOnInboxUpdated:")]
        void SetOnInboxUpdated([NullAllowed] InboxUpdatedHandlerBlock inboxUpdatedHandlerBlock);

        // +(void)getInboxSummaryAsync:(InboxSummaryBlock)inboxSummaryBlock;
        [Static]
        [Export("getInboxSummaryAsync:")]
        void GetInboxSummaryAsync([NullAllowed] InboxSummaryBlock inboxSummaryBlock);
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