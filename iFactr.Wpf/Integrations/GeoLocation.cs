using System;
using System.Device.Location;
using iFactr.Integrations;

namespace iFactr.Wpf
{
    public class GeoLocation : IGeoLocation
    {
        public event EventHandler<GeoLocationEventArgs> LocationUpdated;

        public bool IsActive { get; private set; }

        private readonly GeoCoordinateWatcher watcher;

        public GeoLocation()
        {
            watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += (o, e) =>
            {
                var handler = LocationUpdated;
                if (handler != null)
                {
                    handler(this, new GeoLocationEventArgs(new GeoLocationData(e.Position.Location.Latitude, e.Position.Location.Longitude)));
                }
            };
        }

        public void Start()
        {
            watcher.Start();
            IsActive = true;
        }

        public void Stop()
        {
            watcher.Stop();
            IsActive = false;
        }
    }
}
