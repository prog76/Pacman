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
	public partial class Progress : UserControl
	{
		double fMax, fVal;
		public double max
		{ 
			get
			{
				return fMax;
			}
			set
			{
				fMax = value;
				val = val;
			}
		}
		public double val 
		{
			get
			{
				return fVal;
			}
			set
			{
				fVal = value;
				if ((fVal >= fMax)||(fMax==0)) grid1.Height = 100;
				else grid1.Height = fVal / fMax * 100;
			}
		}

		private static void OnValChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Progress)d).val = (Double)e.NewValue;
		}

		public static readonly DependencyProperty valProperty =
				DependencyProperty.Register("val", typeof(Double), typeof(Progress), new PropertyMetadata(OnValChanged));

		private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Progress)d).max = (Double)e.NewValue;
		}

		public static readonly DependencyProperty maxProperty =
				DependencyProperty.Register("max", typeof(Double), typeof(Progress), new PropertyMetadata(OnMaxChanged));

    
		public Progress()
		{
			fMax = 100;
			InitializeComponent();
			val = 50;
		}
	}
}
