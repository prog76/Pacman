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
using System.Threading;

namespace pacman
{
	public partial class MenuButton : UserControl
	{
		public string text
		{
			get
			{
				return tb.Text;
			}
			set
			{
				tb.Text = value;
			}
		}
		public MenuButton()
		{
			InitializeComponent();
		}

		private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
		{
			mEnter.Begin();
		}

		private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
		{
			mLeave.Begin();
		}

		private void tb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			mDown.Begin();
		}

		private void tb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			mDown.Stop();
			mUp.Begin();
			if (Click == null) return;
			Focus();
			Dispatcher.BeginInvoke(doClick);
		}

		void doClick()
		{
			foreach (RoutedEventHandler h in Click.GetInvocationList())
			{
				try
				{
					h(this, null);
				}
				catch (Exception)
				{
					Console.WriteLine("A listener threw an exception in its handler");
				}
			}
		}

		public event RoutedEventHandler Click;
	}
}
