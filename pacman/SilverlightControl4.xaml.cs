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
using System.Windows.Markup;

namespace pacman
{
	[ContentProperty("xy")]
	public class TXY
	{
		public Double fx, fy;
		public Double x
		{
			get
			{
				return fx;
			}
			set
			{
				fx = value;
			}
		}
		public Double y
		{
			get
			{
				return fy;
			}
			set
			{
				fy = value;
			}
		}
		public string xy
		{
			get
			{
				return fx.ToString() + '|' + fy.ToString();
			}
			set
			{
				string[] _xy = value.Split('|');
				fx = Double.Parse(_xy[0]);
				fy = Double.Parse(_xy[1]);
			}
		}
		public TXY()
		{
		}
	}

	public partial class SilverlightControl4 : UserControl
	{
		public SilverlightControl4()
		{
			InitializeComponent();
		}
	}
}
