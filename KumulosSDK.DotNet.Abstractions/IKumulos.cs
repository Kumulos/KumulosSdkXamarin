﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KumulosSDK.DotNet.Abstractions
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

        void UpdateInAppConsentForUser(bool consentGiven);

        InAppInboxItem[] InboxItems { get; }
        InAppMessagePresentationResult PresentInboxMessage(InAppInboxItem item);
        bool DeleteMessageFromInbox(InAppInboxItem item);
        Task<InAppInboxSummary> GetInboxSummary();
        bool MarkInboxItemAsRead(InAppInboxItem item);
        bool MarkAllInboxItemsAsRead();

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

        void SetInboxUpdatedHandler(IInboxUpdatedHandler inboxUpdatedHandler);
        void ClearInboxUpdatedHandler();
    }
}
