using System;
using System.Windows.Threading;
using iFactr.UI;

namespace iFactr.Wpf
{
    public class Timer : DispatcherTimer, ITimer
    {
        public event EventHandler Elapsed;

        public new bool IsEnabled
        {
            get
            {
                return base.IsEnabled;
            }
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        public new double Interval
        {
            get
            {
                return base.Interval.TotalMilliseconds;
            }
            set
            {
                base.Interval = TimeSpan.FromMilliseconds(value);
            }
        }

        public Timer()
        {
            base.Tick += (o, e) =>
            {
                if (IsEnabled)
                {
                    Stop();
                    var handler = Elapsed;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            };
        }

        public void Dispose()
        {
            Stop();
            Elapsed = null;
        }
    }
}
