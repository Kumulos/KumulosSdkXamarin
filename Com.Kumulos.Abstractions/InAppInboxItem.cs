using System;
namespace Com.Kumulos.Abstractions
{
    public class InAppInboxItem
    {
        public InAppInboxItem(int id, string title, string subtitle, DateTime? availableFrom, DateTime? availableTo, DateTime? dismissedAt)
        {
            Id = id;
            Title = title;
            Subtitle = subtitle;
            AvailableFrom = availableFrom;
            AvailableTo = availableTo;
            DismissedAt = dismissedAt;
        }

        public int Id { get; }
                
        public string Title { get; }

        public string Subtitle { get; }

        public DateTime? AvailableFrom { get; }

        public DateTime? AvailableTo { get; }

        public DateTime? DismissedAt { get; }
    }
}
