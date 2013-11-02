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

namespace pacman.Converters
{
    public class DirectionToAngleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DirectionEnum valueEnum = (DirectionEnum)value;
            switch (valueEnum)
            {
                case DirectionEnum.Right:
                    return 0;
                case DirectionEnum.Left:
                    return 180;
                case DirectionEnum.Down:
                    return 90;
                case DirectionEnum.Up:
                    return -90;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
