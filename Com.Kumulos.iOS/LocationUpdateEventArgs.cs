using System;
using CoreLocation;

namespace Com.Kumulos
{
    public class LocationUpdatedEventArgs : EventArgs
    {
        public LocationUpdatedEventArgs(CLLocation location)
        {
            this.Location = location;
        }

        public CLLocation Location { get; }
    }
}
