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

namespace pacman
{
	public partial class FingerButton : UserControl
	{
		int fFingerNo;
		public Color color
		{
			get
			{
				return Constants.fingerColors[fingerNo];
			}
			set
			{
				Constants.fingerColors[fingerNo] = value;
				fingerNo = fingerNo;
			}
		}
		public int fingerNo
		{
			get
			{
				return fFingerNo;
			}
			set
			{
				fFingerNo = value;
				foreColor.Color = Constants.fingerColors[fingerNo];
			}
		}
		
		public bool isEnter
		{
			get
			{
				return (layout.RowDefinitions[0].Height.Value != 0);
			}
			set
			{
				if (value)
				{
					right.Visibility = System.Windows.Visibility.Visible;
					layout.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
				}
				else
				{
					right.Visibility = System.Windows.Visibility.Collapsed;
					layout.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
				}
			}
		}
		public FingerButton()
		{
			InitializeComponent();
		}
	}
}
