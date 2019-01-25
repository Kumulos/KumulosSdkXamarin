using System;
namespace Com.Kumulos.Abstractions
{
    public interface IKumulos
    {
        void Initialize(IKSConfig config);

        string GetInstallId();

        Build Build { get; }
        PushChannels PushChannels {get;}

        void RegisterForRemoteNotifications();
        void RegisterDeviceToken(object NSDataDeviceToken);
        void TrackNotificationOpen(object NSDictionaryInfo);

        void TrackEvent(string eventType, object properties);
        void TrackEventImmediately(string eventType, object properties);
        void LogException(Exception e);
        void LogUncaughtException(Exception e);
        void SendLocationUpdate(decimal lat, decimal lng);
        void AssociateUserWithInstall(string userIdentifier, object attributes);
        void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, int? distanceMetres);
        void TrackiBeaconProximity(string uuid, int major, int minor, int proximity);
    }
}
