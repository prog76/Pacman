using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GeniusPacman.Core.Interfaces;
using System.Windows.Threading;

namespace pacman
{
    /// <summary>
    /// implemention of <see cref="IPacmanTimer"/> for WPF and Silverlight
    /// </summary>
    public class WPFPacmanTimer : IPacmanTimer
    {
        DispatcherTimer timer;

        public WPFPacmanTimer()
        {
			  
            timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (Tick != null)
                Tick(this, EventArgs.Empty);
        }

        #region IPacmanTimer Members

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public bool IsStarted
        {
            get { return timer.IsEnabled; }
        }

        public int Elapsed
        {
            get
            {
                return timer.Interval.Milliseconds;
            }
            set
            {
                timer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }

        public event EventHandler Tick;

        #endregion
    }
}
