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
using GeniusPacman.Core;
using System.Windows.Data;
using System.Globalization;

namespace pacman
{
	public partial class TPill : UserControl
	{
		public static bool btnPressed = false;
		Point fCenter;

		void TPill_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			center=fCenter;
		}

		public Point center
		{
			get
			{
				return fCenter;
			}
			set
			{
				fCenter = value;
				SetValue(Canvas.LeftProperty, fCenter.X - ActualWidth / 2);
				SetValue(Canvas.TopProperty, fCenter.Y - ActualHeight / 2);
			}
		}

		public TPill()
		{
			InitializeComponent();
			tb.DataContext = this;
			foreColor = Colors.White;
			SizeChanged += new SizeChangedEventHandler(TPill_SizeChanged);
		}

		public string text
		{
			get
			{
				return tb.text;
			}
			set
			{
				tb.text = value;
			}
		}

		public Color borderColor
		{
			get
			{
				return ((SolidColorBrush)BorderBrush).Color;
			}
			set
			{
				if(((SolidColorBrush)BorderBrush).Color!=value)
					((SolidColorBrush)BorderBrush).Color = value;
			}
		}

		public Color foreColor
		{
			get
			{
				return ((SolidColorBrush)Foreground).Color;
			}
			set
			{
				((SolidColorBrush)Foreground).Color=value;
			}
		}

		public int ellipse
		{
			get
			{
				if (ellipseRed.Opacity != 0) return 1;
				if (ellipseGreen.Opacity != 0) return 2;
				return 0;
			}
			set
			{
				ellipseRed.Opacity = 0;
				ellipseGreen.Opacity = 0;
				if (value == 2) ellipseGreen.Opacity = 1;
				if (value == 1) ellipseRed.Opacity = 1;
			}
		}
	}
}
