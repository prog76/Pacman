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
    /// <summary>
    /// view model of one ghost
    /// </summary>
    public class GhostPresenter : BasePresenter
    {
        Game _game;
        int _index;
        Ghost _current;
        public GhostPresenter(Game game, int i)
        {
            _game = game;
            _index = i;
            _current = null;
            _game.NotifyOn<Game>("Ghosts", delegate
            {
                if (_current != null)
                    _current.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_current_PropertyChanged);
					 if (_index < _game.Ghosts.Count)
					 {
						 _current = _game.Ghosts[_index];
					 }
					 else _current = null;
                if (_current != null)
                    _current.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_current_PropertyChanged);
                NotifyPropertyChanged("Ghost");
            });
        }

        void _current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// returns the ghost model
        /// </summary>
        public Ghost Ghost
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// return the game model
        /// </summary>
        public Game Game
        {
            get
            {
                return _game;
            }
        }
    }
}
