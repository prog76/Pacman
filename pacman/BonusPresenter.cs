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
using GeniusPacman.Core;
using GeniusPacman.Core.Sprites;

namespace pacman
{
    public class BonusPresenter : BasePresenter
    {
        Game _currentGame;
        public BonusPresenter(Game g)
        {
            _currentGame = g;
            _currentGame.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_currentGame_PropertyChanged);
        }

        void _currentGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentBonus")
            {
                NotifyPropertyChanged(e.PropertyName);
                NotifyPropertyChanged("Visibility");
                
            }
        }

        public Bonus CurrentBonus
        {
            get
            {
                return _currentGame.CurrentBonus;
            }
        }

        public Visibility Visibility
        {
            get
            {
                return CurrentBonus != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
