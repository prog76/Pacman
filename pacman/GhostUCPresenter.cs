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

namespace pacman
{
    public class GhostUCPresenter : BasePresenter
    {
        GhostUC _view;
        public GhostUCPresenter(GhostUC view)
        {
            _view = view;
        }

        public Brush GhostColor
        {
            get { return _view.GhostColor; }
            set
            {
                NotifyPropertyChanged("GhostColor");
            }
        }


        private GhostPresenter _GhostPresenter;
        public GhostPresenter GhostPresenter
        {
            get { return _GhostPresenter; }
            set
            {
                if (value != _GhostPresenter)
                {
                    _GhostPresenter = value;
                    NotifyPropertyChanged("GhostPresenter");
                }
            }
        }
    }
}
