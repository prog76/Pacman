using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace pacman
{
	public class OutLineShader : ShaderEffect
	{
		public OutLineShader()
		{
			PixelShader psCustom = new PixelShader();
			psCustom.UriSource = new Uri("gfs/outlineShader.ps", UriKind.Relative);
			PixelShader = psCustom;
		}
	}
}
