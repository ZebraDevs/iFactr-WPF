using System;
using iFactr.Integrations;

namespace iFactr.Wpf
{
    public class Compass : ICompass
    {
        public event EventHandler<HeadingEventArgs> HeadingUpdated;

        public bool IsActive { get; private set; }

        public void Start()
        {
            IsActive = true;

            var handler = HeadingUpdated;
            if (handler != null)
            {
                handler(this, new HeadingEventArgs(new HeadingData()));
            }
        }

        public void Stop()
        {
            IsActive = false;
        }
    }
}
