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
	public partial class glowFingerButton : UserControl
	{
		public Color color
		{
			get
			{
				return mainBut.color;
			}
			set
			{
				mainBut.color = value;
				glowBut.color = value;
			}
		}
		public int fingerNo
		{
			get
			{
				return mainBut.fingerNo;
			}
			set
			{
				mainBut.fingerNo = value;
				glowBut.fingerNo = value;
			}
		}
		public bool isGlow
		{
			get
			{
				return glowBut.Opacity > 0.4;
			}
			set
			{
				if (value)
				{
					fade.Begin();
				}
			}
		}
		public bool isEnter
		{
			get
			{
				return mainBut.isEnter;
			}
			set
			{
				mainBut.isEnter = value;
				glowBut.isEnter = value;
			}
		}
		public glowFingerButton()
		{
			InitializeComponent();
			glowBut.blur.Radius = 20;
		}

	}
}
