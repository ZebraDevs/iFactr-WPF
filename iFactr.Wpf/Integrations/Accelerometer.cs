using System;
using iFactr.Integrations;

namespace iFactr.Wpf
{
    public class Accelerometer : IAccelerometer
    {
        public event EventHandler<AccelerometerEventArgs> ValuesUpdated;

        public bool IsActive { get; private set; }

        public void Start()
        {
            IsActive = true;

            var handler = ValuesUpdated;
            if (handler != null)
            {
                handler(this, new AccelerometerEventArgs(new AccelerometerData()));
            }
        }

        public void Stop()
        {
            IsActive = false;
        }
    }
}
