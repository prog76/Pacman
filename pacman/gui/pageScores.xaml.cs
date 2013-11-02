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
using SoftConsept.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GeniusPacman.Core;


namespace pacman
{

	public class TScorePresenter
	{
		String fName, fScore, fNumber;
		scoreLine.TMode fMode;
		public String name { get { return fName; } }
		public String score { get { return fScore; } }
		public String number { get { return fNumber; } }
		public scoreLine.TMode mode { get { return fMode; } }
		public TScorePresenter()
		{
		}
		public TScorePresenter(string _number, string _name, string _score, scoreLine.TMode _mode)
		{
			fName = _name;
			fScore = _score;
			fNumber = _number;
			fMode = _mode;
		}
	}

	public partial class pageScores : UserControl, INotifyPropertyChanged
	{
		ObservableCollection<TScorePresenter> fScores;

		protected void DoPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<TScorePresenter> scores
		{
			get { return fScores; }
		}
		public event RoutedEventHandler Close
		{
			add
			{
				menuClose.Click += value;
			}
			remove
			{
				menuClose.Click -= value;
			}
		}

		public pageScores()
		{
			fScores = new ObservableCollection<TScorePresenter>();
			InitializeComponent();
			scoreSet.DataContext = this;
			this.Loaded += new RoutedEventHandler(pageScores_Loaded);
		}

		public void show(){
			Visibility=Visibility.Visible;
			fScores.Clear();
			int i=0;
			foreach(TScore _score in (DataContext as GamePresenter).CurrentGame.cfg.scores){
				i++;
				fScores.Add(new TScorePresenter(i.ToString(), _score.name, _score.score.ToString(), ((DataContext as GamePresenter).CurrentGame.cfg.playerCfg.scoreNo != i)?scoreLine.TMode.Regular:scoreLine.TMode.Hilight));
			}
		}

		void pageScores_Loaded(object sender, RoutedEventArgs e)
		{

		}
	}
}
