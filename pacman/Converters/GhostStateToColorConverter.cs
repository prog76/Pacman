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
using System.Windows.Data;
using GeniusPacman.Core;

namespace pacman
{
    public class GhostStateToColorConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GhostState state = value as GhostState;
            if (state != null)
            {

                if (state.isFlee())
                {
                    return Application.Current.Resources["ghostFuite"];
                }
                if (state.isEye())
                {
                    return Application.Current.Resources["ghostEye"];
                }
                return parameter;
            }
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
