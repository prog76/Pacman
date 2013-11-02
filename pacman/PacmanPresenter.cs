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
using GeniusPacman.Core.Sprites;
using GeniusPacman.Core;
using System.Diagnostics;

namespace pacman
{
    public class PacmanPresenter : SpritePresenter
    {
        Game _game;
		  public TXYPresenter xyList;
        public PacmanPresenter(Game game)
        {
            _game = game;
            _game.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_game_PropertyChanged);
        }

        void _game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PacMan")
            {
                if (_Pacman != null)
                    _Pacman.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_Pacman_PropertyChanged);

                _Pacman = _game.PacMan;
					 if (_Pacman != null)
					 {
						 _Pacman.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_Pacman_PropertyChanged);
						 Pacman.xyList = xyList;
					 }

                NotifyPropertyChanged("Pacman");
            }
        }

        Pacman _Pacman;
        public Pacman Pacman 
        {
            get
            {
                return _game.PacMan;
            }
        }

        public int Orientation
        {
            get
            {
                if (_Pacman != null)
                {
                    switch(_Pacman.CurrentDirection.ToEnum())
                    {
                        case DirectionEnum.Right :
                            return 0;
                        case DirectionEnum.Up:
                            return 270;
                        case DirectionEnum.Left:
                            return 180;
                        case DirectionEnum.Down:
                            return 90;
                    }
                }
                return 0;
            }
        }

        void _Pacman_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentDirection":
                    NotifyPropertyChanged("Orientation");
                    break;
                default :
                    NotifyPropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
