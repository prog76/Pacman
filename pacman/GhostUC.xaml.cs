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
using System.Windows.Data;

namespace pacman
{
    public partial class GhostUC : UserControl
    {
        GhostUCPresenter Presenter;

        public GhostUC()
        {
            Presenter = new GhostUCPresenter(this);
            this.DataContext = Presenter;
            //RadialGradientBrush br = new RadialGradientBrush();
            //br.Center = new Point(0.5, 0.5);
            //br.GradientStops.Add(new GradientStop() { Offset = 0, Color = Colors.White });
            //GradientStop gs = new GradientStop() { Offset = 1, Color = Colors.Orange };
            //br.GradientStops.Add(gs);

            //this.GhostColor = br;
            InitializeComponent();
        }
        
        private static void OnGhostColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GhostUC)d).Presenter.GhostColor = (Brush)e.NewValue;
        }

        public Brush GhostColor
        {
            get { return (Brush)GetValue(GhostColorProperty); }
            set { SetValue(GhostColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GhostColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GhostColorProperty =
            DependencyProperty.Register("GhostColor", typeof(Brush), typeof(GhostUC), new PropertyMetadata(OnGhostColorChanged));


    }
}
