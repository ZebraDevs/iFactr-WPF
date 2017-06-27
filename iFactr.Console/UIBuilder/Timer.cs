using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class Timer : System.Timers.Timer, ITimer
    {
        public bool IsEnabled
        {
            get { return base.Enabled; }

            set { base.Enabled = value; }
        }

        public event EventHandler Elapsed;

        public Timer()
        {
            base.Elapsed += (o, e) =>
            {
                Elapsed?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
