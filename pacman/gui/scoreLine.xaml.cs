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
	public partial class scoreLine : UserControl
	{
		public enum TMode{Header, Hilight, Regular};
		TMode fMode;
		public string number
		{
			get { return boxNumber.Text; }
			set { boxNumber.Text = value; }
		}

		public String name
		{
			get { return boxName.Text;}
			set { boxName.Text = value; }
		}

		public string score
		{
			get { return boxScore.Text; }
			set { boxScore.Text = value; }
		}

		public TMode mode
		{
			get
			{
				return fMode;
			}
			set{
				fMode = value;
				switch(value){
					case TMode.Header:
					FontSize=16;
					Foreground = new SolidColorBrush(Colors.Yellow);
					break;
					case TMode.Hilight:
					FontSize=14;
					Foreground = new SolidColorBrush(Colors.Red);
					break;
					case TMode.Regular:
					FontSize=14;
					Foreground = new SolidColorBrush(Colors.White);
					break;
				}
			}
		}

		public scoreLine()
		{
			InitializeComponent();
		}

		private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((scoreLine)d).mode = (TMode)e.NewValue;
		}

		public static readonly DependencyProperty modeProperty =
				DependencyProperty.Register("mode", typeof(TMode), typeof(scoreLine), new PropertyMetadata(OnModeChanged));

		private static void OnNamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((scoreLine)d).name = (string)e.NewValue;
		}

		public static readonly DependencyProperty namProperty =
				DependencyProperty.Register("name", typeof(string), typeof(scoreLine), new PropertyMetadata(OnNamChanged));

		private static void OnNumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((scoreLine)d).number = (string)e.NewValue;
		}

		public static readonly DependencyProperty numProperty =
				DependencyProperty.Register("number", typeof(string), typeof(scoreLine), new PropertyMetadata(OnNumChanged));

		private static void OnScoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((scoreLine)d).score = (string)e.NewValue;
		}

		public static readonly DependencyProperty scoreProperty =
				DependencyProperty.Register("score", typeof(string), typeof(scoreLine), new PropertyMetadata(OnScoreChanged));
	}
}
