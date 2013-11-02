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
using System.Collections.Specialized;
using GeniusPacman.Core.Sprites;
using System.Windows.Data;

namespace pacman
{
    public partial class FlyingScoreUC : UserControl
    {
        Dictionary<FlyingScore, UIElement> _dico = new Dictionary<FlyingScore, UIElement>();
        GamePresenter _presenter;
        public FlyingScoreUC()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(FlyingScoreUC_Loaded);
        }

        void FlyingScoreUC_Loaded(object sender, RoutedEventArgs e)
        {
            if (_presenter == null)
            {
                _presenter = this.DataContext as GamePresenter;
                if (_presenter != null)
                {
                    _presenter.CurrentGame.FlyingScores.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(FlyingScores_CollectionChanged);
                }
            }
        }

        void FlyingScores_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FlyingScore sprite in e.NewItems)
                {
                    TextBlock rect = new TextBlock();

                    rect.Text = sprite.NbPoints.ToString();
                    rect.Foreground = new SolidColorBrush(Colors.White);
                    rect.FontFamily = new FontFamily("digital.ttf#digital");
                    //Rectangle rect = new Rectangle();
                    //rect.Fill = new SolidColorBrush(Colors.Purple);
                    //rect.Width = 10;
                    //rect.Height = 10;
                    this._dico.Add(sprite, rect);
                    rect.DataContext = sprite;
                    Binding b = new Binding("Y");
                    rect.SetBinding(Canvas.TopProperty, b);
                    Canvas.SetLeft(rect, sprite.X);
                    this.LayoutRoot.Children.Add(rect);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (FlyingScore sprite in e.OldItems)
                {
                    UIElement o;
                    if (this._dico.TryGetValue(sprite, out o))
                    {
                        this._dico.Remove(sprite);
                        this.LayoutRoot.Children.Remove(o);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _dico.Clear();
                this.LayoutRoot.Children.Clear();
            }
        }
    }
}
