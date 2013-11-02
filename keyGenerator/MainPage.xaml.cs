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
using pacman;

namespace keyGenerator
{
	public partial class MainPage : UserControl
	{
		myProtect prot;
		public MainPage()
		{
			InitializeComponent();
			prot = new myProtect();
			code.Text = prot.getCode();
			pass.Text = prot.getKey();
		}

		private void getPass_Click(object sender, RoutedEventArgs e)
		{
			pass.Text = prot.getPass(code.Text);
		}

		private void writePass_Click(object sender, RoutedEventArgs e)
		{
			prot.addKey(pass.Text);
		}
	}

	class myProtect : Protect
	{
		public string getKey(){
			return getRegKey().readKey();
		}
		public string getCode()
		{
			return getRegKey().getCode();
		}
		public string getPass(string code)
		{
			return getRegKey().getPass(code);
		}
	}
}
