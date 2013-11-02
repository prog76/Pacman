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
using SoftConsept.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace pacman
{

	public partial class pageMenu : UserControl, INotifyPropertyChanged
	{
		Storyboard _sb;
		GeneralTransform ghostTop, ghostBottom;
		bool _sbStarted = false;

		public event RoutedEventHandler Close{
			add{
				menuClose.Click+=value;
			}
			remove
			{
				menuClose.Click -= value;
			}
		}

		protected void DoPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Double GTop { get { return (ghostTop!=null)?ghostTop.Transform(new Point(0, 0)).Y:21.5; } }
		public Double GBottom { get { return ghostBottom.Transform(new Point(0, 0)).Y; } }
		public bool replayEnabled { get { return game.cfg.playerCfg.record.Count > 0; } }
		
		public int ghost2Speed { get { return game.cfg.ghostCount > 1 ? 1 : 0; } }
		public int ghost3Speed { get { return game.cfg.ghostCount > 2 ? 1 : 0; } }
		public int ghost4Speed { get { return game.cfg.ghostCount > 3 ? 1 : 0; } }

		public pageMenu()
		{
			InitializeComponent();
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			this.Loaded += new RoutedEventHandler(pageMenuLoaded);
		}

		public bool menuVisible
		{
			get{
				return menu.Visibility == Visibility.Visible;
			}
			set
			{
				if (value)
				{
					sets.Visibility = Visibility.Collapsed;
					menu.Visibility = Visibility.Visible;
				}
				else
				{
					menu.Visibility = Visibility.Collapsed;
					sets.Visibility = Visibility.Visible;
				}
			}
		}

		Game game
		{
			get { return gamePresenter.CurrentGame; }
		}

		GamePresenter gamePresenter
		{
			get { return (this.DataContext as GamePresenter); }
		}

		void startSb()
		{
			if (!_sbStarted)
			{
				_sb.Begin();
				_sbStarted = true;
			}
		}

		void pageMenuLoaded(object sender, RoutedEventArgs e)
		{
			_sb = this.LayoutRoot.Resources["sb1"] as Storyboard;
			_sb.Completed += new EventHandler(_sb_Completed);

			startSb();
			if (System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
				return; //HACK BUG VS Designer
			}
			(this.DataContext as GamePresenter).NotifyOn<GamePresenter>("IsGameOver", delegate(GamePresenter presenter)
		  {
			  if (presenter.IsGameOver)
			  {
				  startSb();
				  DoPropertyChanged("replayEnabled");
			  }
		  });

			(this.DataContext as GamePresenter).CurrentGame.cfg.NotifyOn<StoredConfig>("ghostCount", delegate(StoredConfig cfg)
			{
				DoPropertyChanged("ghost2Speed");
				DoPropertyChanged("ghost3Speed");
				DoPropertyChanged("ghost4Speed");
			});

			game.cfg.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Cfg_PropertyChanged);
			init();

			installOffline.Visibility = (Application.Current.InstallState == InstallState.NotInstalled) ? Visibility.Visible : Visibility.Collapsed;								installOffline.Visibility = Visibility.Collapsed;
		}

		void init()
		{
			game.initGame();
		}

		void _sb_Completed(object sender, EventArgs e)
		{
			_sbStarted = false;
			if (this.Visibility == Visibility.Visible)
			{
				Debug.WriteLine("ghost animation re-started");
				startSb();
			}
			else
			{
				Debug.WriteLine("ghost animation is stopped");
			}
		}

		void New_Click(object sender, RoutedEventArgs e)
		{
			menuVisible=false;
		}

		void Continue_Click(object sender, RoutedEventArgs e)
		{
			if (gamePresenter.canPlay())
			{
				game.cont();
				game.KeyPressed(GeniusPacman.Core.PacmanKey.Space);
			}
		}

		void Replay_Click(object sender, RoutedEventArgs e)
		{
			if (gamePresenter.canPlay())
			{
				game.replay();
				game.KeyPressed(GeniusPacman.Core.PacmanKey.Space);
			}
		}

		void root_LayoutUpdated(object sender, EventArgs e)
		{
			ghostTop = upBox.TransformToVisual(canvas);
			ghostBottom = downBox.TransformToVisual(canvas);
			DoPropertyChanged("GBottom");
			DoPropertyChanged("GTop");
		}

		void ExportClick(object sender, RoutedEventArgs e)
		{
			SaveFileDialog sd = new SaveFileDialog();
			sd.DefaultExt = ".xml";
			sd.Filter = "Xml Files|*.xml|All Files|*.*";
			bool? dialogResult = sd.ShowDialog();
			if (dialogResult == true)
			{
				game.saveToStream(sd.OpenFile());
			}
		}

		void ImportClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog od = new OpenFileDialog();
			od.Filter = "Xml Files|*.xml|All Files|*.*";
			bool? dialogResult = od.ShowDialog();
			if (dialogResult == true)
			{
				game.intLoadStream(od.File.OpenRead());
				game.save();
			}
		}

		void DefaultClick(object sender, RoutedEventArgs e)
		{
			game.cfg.init();
			init();
			game.save();
		}

		void NextPlayer(object sender, RoutedEventArgs e)
		{
			game.cfg.playerNext();
		}

		void PrevPlayer(object sender, RoutedEventArgs e)
		{
			game.cfg.playerPrev();
		}

		void Cfg_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "playerName")
			{
				DoPropertyChanged("replayEnabled");
			}
		}

		void pageGameExit(object sender, RoutedEventArgs e)
		{
			menuVisible=true;
		}

		private void Menu_GotFocus(object sender, RoutedEventArgs e)
		{
			startSb();
		}

		private void installOffline_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Running game offline will require registration. Procced with installation?",
	 "Desktop installation", MessageBoxButton.OKCancel);

			if (result == MessageBoxResult.OK)
			{
				Application.Current.Install(); 
			}

		}

	}
}
