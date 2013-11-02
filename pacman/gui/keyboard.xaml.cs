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
using System.Collections.ObjectModel;

namespace pacman
{
	public partial class FingerKeyboard : UserControl
	{
		int fKey;
		public int key
		{
			get
			{
				return fKey;
			}
			set
			{
				fKey = value;
				var elements = from FrameworkElement element in fingers.Children
				where element is glowFingerButton &&
				((glowFingerButton)element).fingerNo==value
				select element as glowFingerButton;
				foreach(glowFingerButton element in elements)element.isGlow = true;
			}
		}
		public FingerKeyboard()
		{
			InitializeComponent();
		}

		private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FingerKeyboard)d).key = (int)e.NewValue;
		}

		public static readonly DependencyProperty valProperty =
				DependencyProperty.Register("key", typeof(int), typeof(FingerKeyboard), new PropertyMetadata(OnKeyChanged));

	}
}
