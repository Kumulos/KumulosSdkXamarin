using System;
namespace Com.Kumulos.Abstractions
{
    public class InAppInboxItem
    {
        public InAppInboxItem()
        {
        }

        public int Id { get; }
                
        public string Title { get; }

        public string Subtitle { get; }

        public DateTime AvailableFrom { get; }

        public DateTime AvailableTo { get; }

        public DateTime DismissedAt { get; }
    }
}
