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
	public partial class menuCb : UserControl
	{
		bool fSeparator;
		public bool separator
		{
			get
			{
				return fSeparator;
			}
			set
			{
				fSeparator = value;
				if (fSeparator)
				{
					cb.Visibility = Visibility.Collapsed;
					text = "-----------";
				}
				else
				{
					cb.Visibility = Visibility.Visible;
				}

			}
		}

		public string text
		{
			get
			{
				return txt.text;
			}
			set
			{
				txt.text = value;
			}
		}
		public bool isChecked
		{
			get
			{
				return (cb.IsChecked==true);
			}
			set
			{
				if(value!=cb.IsChecked)
					cb.IsChecked = value;
			}
		}

		public event RoutedEventHandler Click
		{
 		   add { cb.Click += value; }  
			remove { cb.Click -= value; }  
		}

		public menuCb()
		{
			InitializeComponent();
			cb.DataContext = this;
			isChecked = false;
		}

		private static void OnSepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((menuCb)d).separator = (bool)e.NewValue;
		}

		public static readonly DependencyProperty sepProperty =
				DependencyProperty.Register("separator", typeof(bool), typeof(menuCb), new PropertyMetadata(OnSepChanged));


		private static void OnTxtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((menuCb)d).text = (string)e.NewValue;
		}

		public static readonly DependencyProperty txtProperty =
				DependencyProperty.Register("text", typeof(string), typeof(menuCb), new PropertyMetadata(OnTxtChanged));

		private static void OnChkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((menuCb)d).isChecked = (bool)e.NewValue;
		}

		public static readonly DependencyProperty chkProperty =
				DependencyProperty.Register("isChecked", typeof(bool), typeof(menuCb), new PropertyMetadata(OnChkChanged));
	}
}
