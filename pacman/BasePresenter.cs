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
using System.ComponentModel;

namespace pacman
{
    public class BasePresenter : INotifyPropertyChanged
    {
        public BasePresenter()
        {
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }

    public static class BasePresenterExtension
    {
        public static void NotifyOn<T>(this INotifyPropertyChanged sender, string propName, Action<T> onNotify)
        {
            sender.PropertyChanged += delegate(object sender1, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == propName)
                {
                    onNotify((T)sender1);
                }
            };
        }
    }
}
