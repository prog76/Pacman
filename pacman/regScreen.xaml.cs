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
	public partial class RegScreen : ChildWindow
	{
		static RegScreen regScreen;
		public RegScreen()
		{
			InitializeComponent();
			regCode.Text = Protect.getInstance().getCode();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Protect.getInstance().addKey(regKey.Text);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		public static RegScreen getInstance(){
			if (regScreen == null) regScreen = new RegScreen();
			regScreen.Show();
			return regScreen;
		}
	}
}

