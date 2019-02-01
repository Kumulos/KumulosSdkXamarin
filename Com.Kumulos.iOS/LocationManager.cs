using System;
using CoreLocation;
using UIKit;

namespace Com.Kumulos
{
    public class LocationManager
    {
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        protected CLLocationManager locMgr;

        public LocationManager()
        {
            locMgr = new CLLocationManager
            {
                PausesLocationUpdatesAutomatically = false
            };

            // iOS 8 has additional permissions requirements
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization(); // works in background
                                                     //locMgr.RequestWhenInUseAuthorization (); // only in foreground
            }

            locMgr.AllowsBackgroundLocationUpdates |= UIDevice.CurrentDevice.CheckSystemVersion(9, 0);

            LocationUpdated += SendLocationToKumulos;
        }

        public CLLocationManager LocMgr
        {
            get { return locMgr; }
        }

        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                //set the desired accuracy, in meters
                LocMgr.DesiredAccuracy = 1;
                LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                };
                LocMgr.StartUpdatingLocation();
            }
        }

        public void SendLocationToKumulos(object sender, LocationUpdatedEventArgs e)
        {
            CLLocation location = e.Location;

            Kumulos.Current.SendLocationUpdate(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }
    }
}
