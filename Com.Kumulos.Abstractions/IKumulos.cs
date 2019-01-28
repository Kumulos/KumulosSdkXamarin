using System;
using System.Collections.Generic;

namespace Com.Kumulos.Abstractions
{
    public interface IKumulos
    {
        void Initialize(IKSConfig config);

        string InstallId { get; }

        Build Build { get; }
        PushChannels PushChannels { get; }

        void RegisterForRemoteNotifications();
        void RegisterDeviceToken(object NSDataDeviceToken);
        void TrackNotificationOpen(object NSDictionaryInfo);

        void TrackEvent(string eventType, Dictionary<string, string> properties);
        void TrackEventImmediately(string eventType, Dictionary<string, string> properties);

        void LogException(Exception e);
        void LogUncaughtException(Exception e);

        void SendLocationUpdate(double lat, double lng);

        void AssociateUserWithInstall(string userIdentifier);
        void AssociateUserWithInstall(string userIdentifier, Dictionary<string, string> attributes);

        void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, int? distanceMetres);
        void TrackiBeaconProximity(object CLBeaconObject);
    }
}
