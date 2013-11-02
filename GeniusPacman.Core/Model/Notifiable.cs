using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace GeniusPacman.Core
{
    /// <summary>
    /// base class for all "notifiable" classes, implements <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public class Notifiable : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        protected void DoPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
