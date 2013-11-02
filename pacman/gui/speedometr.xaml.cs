using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace pacman
{
	public partial class speedometr : UserControl
	{
		public double max
		{
			get
			{
				return gauge.MaxValue;
			}
			set
			{
				gauge.MaxValue = value;
			}
		}
		public double val
		{
			get
			{
				return gauge.CurrentValue;
			}
			set
			{
				gauge.CurrentValue = value;
				double lowRange = max - value;
				if (lowRange < max / 5)
					max = Math.Round((value * 1.6) / 10) * 10;
			}
		}

		public double opBegin
		{
			get
			{
				return gauge.OptimalRangeStartValue;
			}
			set
			{
				gauge.OptimalRangeStartValue = value;
				double lowRange = max-value;
				if ((lowRange < max / 2) || (lowRange > max * 3 / 4))
					max = Math.Round((value * 1.6) / 10) * 10;
				opEnd = value + max / 5;
			}
		}

		public double opEnd
		{
			get
			{
				return gauge.OptimalRangeEndValue;
			}
			set
			{
				gauge.OptimalRangeEndValue = value;
			}
		}

		private static void OnValChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((speedometr)d).val = (Double)e.NewValue;
		}

		public static readonly DependencyProperty valProperty =
				DependencyProperty.Register("val", typeof(Double), typeof(speedometr), new PropertyMetadata(OnValChanged));

		private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((speedometr)d).max = (Double)e.NewValue;
		}

		public static readonly DependencyProperty maxProperty =
				DependencyProperty.Register("max", typeof(Double), typeof(speedometr), new PropertyMetadata(OnMaxChanged));

		private static void OnOpBeginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((speedometr)d).opBegin = (Double)e.NewValue;
		}

		public static readonly DependencyProperty opBeginProperty =
				DependencyProperty.Register("opBegin", typeof(Double), typeof(speedometr), new PropertyMetadata(OnOpBeginChanged));

		public static readonly DependencyProperty opEndProperty =
				DependencyProperty.Register("opEnd", typeof(Double), typeof(speedometr), new PropertyMetadata(OnOpEndChanged));

		private static void OnOpEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((speedometr)d).opEnd = (Double)e.NewValue;
		}

		public speedometr()
		{
			InitializeComponent();
		}
	}
}
