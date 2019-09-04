using System;
using CoreLocation;
using Foundation;
using UIKit;

namespace Com.Kumulos.Samples
{
    public class BeaconManager
    {
        CLLocationManager locationManager;
        CLBeaconRegion beaconRegion;

        public BeaconManager()
        {
            SetupBeaconRanging();

            locationManager.StartMonitoring(beaconRegion);
            locationManager.RequestState(beaconRegion);

            locationManager.StartRangingBeacons(beaconRegion);
        }

        private void SetupBeaconRanging()
        {
            locationManager = new CLLocationManager();

            //TODO: Read plist for a beacon uuid region.
            var beaconRegionUuid = new NSUuid("B9407F30-F5F8-466E-AFF9-25556B57FE6D");
            beaconRegion = new CLBeaconRegion(beaconRegionUuid, beaconRegionUuid.ToString());

            beaconRegion.NotifyEntryStateOnDisplay = true;
            beaconRegion.NotifyOnEntry = true;
            beaconRegion.NotifyOnExit = true;

            locationManager.RegionEntered += HandleRegionEntered;
            locationManager.RegionLeft += HandleRegionLeft;
            locationManager.DidDetermineState += HandleDidDetermineState;
            locationManager.DidRangeBeacons += HandleDidRangeBeacons;
        }

        void HandleRegionLeft(object sender, CLRegionEventArgs e)
        {
            if (!e.Region.Identifier.Equals(beaconRegion))
            {
                return;
            }

            locationManager.StopRangingBeacons(beaconRegion);
        }

        void HandleRegionEntered(object sender, CLRegionEventArgs e)
        {
            Console.WriteLine("Region entered: " + e.Region.Identifier);
            if (!e.Region.Identifier.Equals(beaconRegion))
            {
                return;

            }

            locationManager.StartRangingBeacons(beaconRegion);
            Console.WriteLine("Started ranging for beacons in: " + e.Region.Identifier);
        }

        void HandleDidDetermineState(object sender, CLRegionStateDeterminedEventArgs e)
        {
            if (!e.Region.Identifier.Equals(beaconRegion))
            {
                return;

            }

            if (e.State == CLRegionState.Inside)
            {
                Console.WriteLine("Inside beacon region [{0}]", e.Region.Identifier);
                locationManager.StartRangingBeacons(beaconRegion);

            }
            else if (e.State == CLRegionState.Outside)
            {
                Console.WriteLine("Outside beacon region");
                locationManager.StopRangingBeacons(beaconRegion);
            }
        }

        void HandleDidRangeBeacons(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            if (e.Beacons.Length > 0)
            {
                foreach (var b in e.Beacons)
                {

                    if (b.Proximity == CLProximity.Unknown)
                    {
                        continue;
                    }
                    Console.WriteLine("UUID: {0} | Major: {1} | Minor: {2} | Accuracy: {3} | Proximity: {4} | RSSI: {5}", b.ProximityUuid, b.Major, b.Minor, b.Accuracy, b.Proximity, b.Rssi);
                    Kumulos.Current.TrackiBeaconProximity(b);
                }
            }
        }
    }
}
