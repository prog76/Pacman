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
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SoftConsept.Collections;
using GeniusPacman.Core;

namespace pacman
{
	public partial class pageHolder: UserControl
	{
		public pageHolder()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(pageHolderLoaded);
		}
		void pageHolderLoaded(object sender, RoutedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
				this.DataContext = new GamePresenter(); //HACK BUG VS Designer
			}
			(this.DataContext as GamePresenter).NotifyOn<GamePresenter>("IsGameOver", delegate(GamePresenter presenter)
			{
				if (presenter.IsGameOverDeath)
				{
					showScores = true;
				}
			});			
		}

		public bool showScores
		{
			get
			{
				return (Scores.Visibility == Visibility.Visible);
			}
			set
			{
				if (value)
				{
					Menu.Visibility = Visibility.Collapsed;
					Scores.show();
				}
				else
				{
					Scores.Visibility = Visibility.Collapsed;
					Menu.Visibility = Visibility.Visible;
					Menu.menuVisible = true;
					Menu.Focus();
				}
			}
		}

		private void subPageClose(object sender, RoutedEventArgs e)
		{
			showScores = !showScores;
		}
	}
}
