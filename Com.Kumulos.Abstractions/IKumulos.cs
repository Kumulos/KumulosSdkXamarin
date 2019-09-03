using System;
using System.Collections.Generic;

namespace Com.Kumulos.Abstractions
{
    public interface IKumulos
    {
        void Initialize(IKSConfig config);

        string InstallId { get; }
        string UserIdentifier { get; }

        Build Build { get; }
        PushChannels PushChannels { get; }

        void RegisterForRemoteNotifications();
        void UnregisterDeviceToken();

        void TrackEvent(string eventType, Dictionary<string, object> properties);
        void TrackEventImmediately(string eventType, Dictionary<string, object> properties);

        void LogException(Exception e);
        void LogUncaughtException(Exception e);

        void SendLocationUpdate(double lat, double lng);

        void AssociateUserWithInstall(string userIdentifier);
        void AssociateUserWithInstall(string userIdentifier, Dictionary<string, object> attributes);
        void ClearUserAssociation();

        void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, double distanceMetres);
        void TrackiBeaconProximity(object CLBeaconObject);
    }
}
