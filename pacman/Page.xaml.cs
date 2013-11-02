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
using pacman;

namespace pacman
{
    public partial class Page : UserControl
    {
        GamePresenter Presenter = new GamePresenter();

		  public GamePresenter present
		  {
			  get
			  {
				  return Presenter;
			  }
		  }

        public Page()
        {
            this.DataContext = Presenter;
            InitializeComponent();
	
            this.KeyDown += new KeyEventHandler(Page_KeyDown);
        }

        void Page_KeyDown(object sender, KeyEventArgs e)
        {
            pacmanUC.Foreground = null;
        }

		  private void MusicoFFClick(object sender, MouseButtonEventArgs e)
		  {
			  Presenter.isMusicMuted = !Presenter.isMusicMuted;
		  }

		  private void SoundOffClick(object sender, MouseButtonEventArgs e)
		  {
			  Presenter.isSoundMuted = !Presenter.isSoundMuted;
		  }

		  private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		  {
			  Presenter.openRegisterScreen();
		  }
    }
}
