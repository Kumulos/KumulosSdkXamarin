using System;
using Newtonsoft.Json.Linq;

namespace Com.Kumulos.Abstractions
{
    public class InAppInboxItem
    {
        public InAppInboxItem(int id, bool isRead, string title, string subtitle, DateTime? sentAt, DateTime? availableFrom, DateTime? availableTo, DateTime? dismissedAt, string imageUrl, JObject data)
        {
            Id = id;
            IsRead = isRead;
            Title = title;
            Subtitle = subtitle;
            SentAt = sentAt;
            AvailableFrom = availableFrom;
            AvailableTo = availableTo;
            DismissedAt = dismissedAt;
            Data = data;
            ImageUrl = imageUrl;
        }

        public int Id { get; }

        public bool IsRead { get; }
                
        public string Title { get; }

        public string Subtitle { get; }

        public DateTime? SentAt { get; }

        public DateTime? AvailableFrom { get; }

        public DateTime? AvailableTo { get; }

        public DateTime? DismissedAt { get; }

        public JObject Data { get; }

        public string ImageUrl { get; }
    }
}
