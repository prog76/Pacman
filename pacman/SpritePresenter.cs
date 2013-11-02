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

namespace pacman
{
    public class SpritePresenter : BasePresenter
    {
        DirectionEnum _Direction;
        public DirectionEnum Direction
        {
            get
            {
                return _Direction;
            }
            protected set
            {
                if (value != _Direction)
                {
                    _Direction = value;
                    NotifyPropertyChanged("Direction");
                }
            }
        }

    }
}
