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

namespace keyboard
{
	public class MyTextBox : TextBox
	{
		TextCompositionEventArgs ee;

		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			String t = e.Text;
			OnComplete(t);
		}
		public delegate void OnCompleteText(string keys);
		public event OnCompleteText OnComplete;
	}
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();
			keys.OnComplete += new MyTextBox.OnCompleteText(keys_OnComplete);
		}

		void keys_OnComplete(string keys)
		{
			txt.Text += keys;
		}
	}
}
