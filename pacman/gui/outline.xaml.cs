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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Effects;

namespace pacman
{
	[ContentProperty("xy")]
	public class TXY:INotifyPropertyChanged
	{
		public Double fx, fy,fdx,fdy;
		public Double dx
		{
			get
			{
				return fdx;
			}
			set{
				if (fdx == value) return;
				fdx = value;
				OnPropertyChanged("x");
			}
		}
		public Double dy
		{
			get
			{
				return fdy;
			}
			set
			{
				if (fdy == value) return;
				fdy = value;
				OnPropertyChanged("y");
			}
		}
		public Double x
		{
			get
			{
				return fx*fdx;
			}
			set
			{
				if (fx == value) return;
				fx = value;
				OnPropertyChanged("x");
			}
		}
		public Double y
		{
			get
			{
				return fy*fdy;
			}
			set
			{
				if (fy == value) return;
				fy = value;
				OnPropertyChanged("y");
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
		public TXY(Double _x, Double _y)
		{
			fx = _x;
			fy = _y;
		}
		public TXY()
		{
			fdy = 1;
			fdx = 1;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public partial class Outline : UserControl
	{
		Double fontKW, fontKH, fKW, fKH, fOutLineSize;
		List<TXY> outLineMoves = new List<TXY>() 
//		{ new TXY(-1, -1), new TXY(-1, 0), new TXY(-1, 1), new TXY(0, 1), new TXY(1, 1), new TXY(1, 0), new TXY(1, -1), new TXY(0, -1) };
		{ new TXY(-1, 0), new TXY(1, 0), new TXY(0, -1), new TXY(0,1) };

		void adjustSize()
		{
			adjustSize(ActualWidth, ActualHeight);
		}
		void adjustSize(Double w, Double h){
			tb.FontSize = Math.Max(1,Math.Min(w * fontKW*fKW, h * fontKH*fKH));
			Double d;
			d = Math.Max(tb.ActualWidth/Math.Max(1,tb.Text.Length) * fOutLineSize,tb.ActualHeight* fOutLineSize);
			foreach (TXY xy in moves)
			{
				xy.dx = d;
				xy.dy = d;
			}
		}

		public ObservableCollection<TXY> moves{get;set;}

		void TPill_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			adjustSize(e.NewSize.Width,e.NewSize.Height);
		}

		public Double outLineSize
		{
			get
			{
				return fOutLineSize;
			}
			set
			{
				if ((value < 0) | (value > 1))throw new ArgumentException("Value should be in 0-1");
				fOutLineSize = value;
				adjustSize();
			}
		}

		public double kW
		{
			get
			{
				return fKW;
			}
			set
			{
				if (value == fKW) return;
				fKW = value;
				adjustSize();
			}
		}
		public double kH
		{
			get
			{
				return fKH;
			}
			set
			{
				if (value == fKH) return;
				fKH = value;
				adjustSize();
			}
		}

		public String text
		{
			get
			{
				return tb.Text;
			}
			set
			{
				tb.Text = value;
				tb.FontSize = 20;
				tb.Text = value;
				fontKH = 20 / (tb.ActualHeight);
				fontKW = 20 / (tb.ActualWidth);
				adjustSize();
			}
		}

		public Outline()
		{
			moves = new ObservableCollection<TXY>(outLineMoves);
			DataContext = this;
			fOutLineSize = 0.05;
			InitializeComponent();
			SizeChanged += new SizeChangedEventHandler(TPill_SizeChanged);
			text = "0";
			fKW = 1;
			fKH = 1;
		}

	}
}
