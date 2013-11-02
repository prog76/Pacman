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

namespace pacman
{

	public class THolderComparer : IComparer<TTskHolder>
	{
		public int Compare(TTskHolder x, TTskHolder y)
		{
			if (y.selected != x.selected)
			{
				if (x.selected) return -1;
				else return 1;
			}
			if (x.separator) return -1;
			if (y.separator) return 1;
			return (y.mySelCnt.CompareTo(x.mySelCnt));
		}
	}

	public class TTskHolder
	{

		public int mySelCnt;

		String fName;

		static int selCnt = 0;
		bool fSelected, fSeparator;

		public bool separator
		{
			get
			{
				return fSeparator;
			}
		}

		public bool selected
		{
			get
			{
				return fSelected;
			}
			set
			{
				if (value) mySelCnt = selCnt++;
				fSelected = value;
			}
		}

		public string name
		{
			get { return fName; }
		}

		public TTskHolder()
		{
			fSeparator = true;
		}

		public TTskHolder(TTskSet _tskSet)
		{
			fName = _tskSet.name;
			fSeparator = false;
		}
	}
	
	public partial class pageGameSetup : UserControl, INotifyPropertyChanged
	{

		SortedObservableCollection<TTskHolder> fTskHolders;

		Game game
		{
			get { return gamePresenter.CurrentGame; }
		}

		GamePresenter gamePresenter
		{
			get { return (this.DataContext as GamePresenter); }
		}


		protected void DoPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public SortedObservableCollection<TTskHolder> tskSets
		{
			get
			{
				return fTskHolders;
			}
		}
		public bool startEnabled { get { return (fTskHolders.Count > 0) && (fTskHolders[0].selected); } }

		public event RoutedEventHandler Exit
		{
			add
			{
				butCancel.Click += value;
			}
			remove
			{
				butCancel.Click -= value;
			}
		}

		public pageGameSetup()
		{
			fTskHolders = new SortedObservableCollection<TTskHolder>(new THolderComparer());
			InitializeComponent();
			setSource.DataContext = this;
			this.Loaded += new RoutedEventHandler(pageGameSetup_Loaded);
		}

		void pageGameSetup_Loaded(object sender, RoutedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
				return; //HACK BUG VS Designer
			}
			fTskHolders.Clear();
			foreach (TTskSet tsk in game.cfg.tskSets) fTskHolders.Add(new TTskHolder(tsk));
			fTskHolders.Add(new TTskHolder());
		}

		void StartClick(object sender, RoutedEventArgs e)
		{
			List<int> tskIds = new List<int>();
			for (int i = 0; i < fTskHolders.Count; i++)
			{
				if (fTskHolders[i].selected)
				{
					for (int j = 0; j < game.cfg.tskSets.Count; j++)
						if (fTskHolders[i].name.Equals(game.cfg.tskSets[j].name))
							tskIds.Add(j);
				}
				else break;
			}
			if (gamePresenter.canPlay())
			{
				game.newGame(tskIds);
				game.KeyPressed(GeniusPacman.Core.PacmanKey.Space);
			}
		}

		void tskSetClick(object sender, RoutedEventArgs e)
		{
			TTskHolder tsk = ((sender as CheckBox).DataContext as menuCb).DataContext as TTskHolder;
			for (int i = 0; i < fTskHolders.Count; i++)
				if (fTskHolders[i].name == tsk.name)
				{
					fTskHolders.RemoveAt(i);
					break;
				}
			fTskHolders.Add(tsk);
			DoPropertyChanged("startEnabled");
		}

		private void ghostCountBut_Click(object sender, RoutedEventArgs e)
		{
			game.cfg.ghostCount = int.Parse((sender as MenuButton).text);
		}
	}
}
