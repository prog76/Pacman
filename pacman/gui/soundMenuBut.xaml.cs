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
	public partial class soundMenuBut : UserControl
	{
		IAudio Audio;
		public string text
		{
			get
			{
				return but.text;
			}
			set
			{
				but.text = value;
			}
		}
		public event RoutedEventHandler Click
		{
			add
			{
				but.Click += value;
			}
			remove
			{
				but.Click -= value;
			}
		}
		public soundMenuBut()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(soundMenuBut_Loaded);
			Click += new RoutedEventHandler(soundMenuBut_Click);
			but.MouseEnter += new MouseEventHandler(but_MouseEnter);
		}

		void but_MouseEnter(object sender, MouseEventArgs e)
		{
			Audio.PlayMenuOver();
		}

		void soundMenuBut_Click(object sender, RoutedEventArgs e)
		{
			Audio.PlayMenuPress();
		}

		void soundMenuBut_Loaded(object sender, RoutedEventArgs e)
		{
			if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
				Audio = ((GamePresenter)this.DataContext).CurrentGame.Audio;
			}
		}
	}
}
