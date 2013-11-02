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
using System.Windows.Resources;
using WaveMSS;

namespace pacman
{

	public class MyTextBox : TextBox
	{
		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			String t = e.Text;
		}
	}

	public partial class PacmanUC : UserControl, IAudio
	{
		private bool _animationInProgress;
		private bool _animforever;
		GamePresenter _gamepresenter;
		GeniusPacman.Core.Sprites.Pacman pMan;

		TSoundPlayer pillPlay, soundPlay, musicPlay;

		public PacmanUC()
		{
			InitializeComponent();
			System.Windows.Interop.SilverlightHost host =Application.Current.Host;
			System.Windows.Interop.Settings settings = host.Settings;
			settings.MaxFrameRate = 30;
			// Read/write properties of the Settings object.
#if DEBUG
			settings.EnableCacheVisualization = true;
			settings.EnableFrameRateCounter = true;  
//			settings.EnableRedrawRegions = true;
#endif

			this.Loaded += new RoutedEventHandler(PacmanUC_Loaded);
			tb.KeyDown += new KeyEventHandler(PacmanUC_KeyDown);
			tb.TextInput += new TextCompositionEventHandler(PacmanUC_Input);
		}

		void pacmanSpetialKey(PacmanKey key)
		{

			_gamepresenter.CurrentGame.lastKeyTurn = 0; ;
			CurrentGame.KeyPressed(key);
		}

		void pacmanKey(char key)
		{
			TTsk keyTsk = _gamepresenter.CurrentGame.taskBox.tskProvider.encodeTask(key);
			if (!keyTsk.isPill()) return;

			_gamepresenter.processKey(keyTsk);
		}

		void PacmanUC_Input(object sender, TextCompositionEventArgs e)
		{
			foreach (char key in e.Text) pacmanKey(key);
		}

		void PacmanUC_Loaded(object sender, RoutedEventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
				this.DataContext = new GamePresenter(); //HACK BUG VS Designer
			}
			GamePresenter gp = ((GamePresenter)this.DataContext);
			gp.Pills = Pills;
			//because FindName doesn't works inside viewbox, i should remap local fields
			Canvas cv = (Canvas)this.viewBox.Child;
			pacman.mouseTopPosition = FindName<LineSegment>(cv, "mouseTopPosition");
			pacman.mouseBottomPosition = FindName<ArcSegment>(cv, "mouseBottomPosition");
			this.ghost1 = FindName<GhostUC>(cv, "ghost1");
			this.ghost2 = FindName<GhostUC>(cv, "ghost2");
			this.ghost3 = FindName<GhostUC>(cv, "ghost3");
			this.ghost4 = FindName<GhostUC>(cv, "ghost4");
			gp.PacmanPresenter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PacmanPresenter_PropertyChanged);
			if (this.ghost1 != null)
				((GhostUCPresenter)this.ghost1.DataContext).GhostPresenter = gp.Ghost1;
			if (this.ghost2 != null)
				((GhostUCPresenter)this.ghost2.DataContext).GhostPresenter = gp.Ghost2;
			if (this.ghost3 != null)
				((GhostUCPresenter)this.ghost3.DataContext).GhostPresenter = gp.Ghost3;
			if (this.ghost4 != null)
				((GhostUCPresenter)this.ghost4.DataContext).GhostPresenter = gp.Ghost4;
			gp.CurrentGame.Audio = this;
			_gamepresenter = gp;
			soundPlay = new TSoundPlayer(soundElem);
			musicPlay = new TSoundPlayer(musicElem);
			pillPlay = new TSoundPlayer(pillElem);
			(this as IAudio).MusicMenu();
		}

		#region resolve problems due of viewbox
		private T FindName<T>(FrameworkElement parent, string name)
		{
			Queue<DependencyObject> childs = new Queue<DependencyObject>();
			childs.Enqueue(parent);
			while (childs.Count > 0)
			{
				var ctl = childs.Dequeue();
				FrameworkElement fChild = ctl as FrameworkElement;
				if (fChild != null && fChild.Name == name)
				{
					return (T)(object)fChild;
				}
				if (ctl is Path)
				{
					if (((Path)ctl).Name == "pacman")
					{
						this.pacman.orientationPacman = (RotateTransform)
						((TransformGroup)((PathGeometry)((Path)ctl).Data).Transform).Children[0];
					}
					object result;
					if (FindInGeometry(((Path)ctl).Data, name, out result))
					{
						return (T)result;
					}
				}

				int nbChild = VisualTreeHelper.GetChildrenCount(ctl);
				for (int i = 0; i < nbChild; i++)
				{
					childs.Enqueue(VisualTreeHelper.GetChild(ctl, i));
				}
			}
			return default(T);
		}

		private bool FindInGeometry(Geometry geometry, string name, out object result)
		{
			result = null;
			var pgeo = geometry as PathGeometry;

			if (pgeo != null)
			{
				foreach (var figure in pgeo.Figures)
				{
					var figureName = figure.GetValue(FrameworkElement.NameProperty);
					if (name.Equals(figureName))
					{
						result = figure;
						return true;
					}
					var pFigure = figure as PathFigure;
					if (pFigure != null)
					{
						foreach (var segment in pFigure.Segments)
						{
							figureName = segment.GetValue(FrameworkElement.NameProperty);
							if (name.Equals(figureName))
							{
								result = segment;
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		#endregion

		void PacmanPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Orientation")
			{
				pacman.orientationPacman.Angle = ((PacmanPresenter)sender).Orientation;
			}
			else if (e.PropertyName == "InMove")
			{
				tb.Focus();
				_animforever = CurrentGame.PacMan.InMove;
				if (!_animationInProgress && _animforever)
				{
					System.Diagnostics.Debug.WriteLine("startAnimation");
					_animationInProgress = true;
					StartAnimation();
				}
			}
			else if (e.PropertyName == "Pacman")
			{
				pMan = _gamepresenter.PacmanPresenter.Pacman;
			}
		}

		private void StartAnimation()
		{
			const int CSt_MS = 50;
			PointAnimationUsingKeyFrames ptAnim = new PointAnimationUsingKeyFrames();
			ptAnim.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(14.7, 8.5) });
			ptAnim.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(15, 9.99) });

			Timeline anim = ptAnim;//new PointAnimation();
			//anim.To = new Point(15, 9.99);
			Storyboard sb = new Storyboard();
			sb.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			sb.Children.Add(anim);
			Storyboard.SetTarget(anim, pacman.mouseTopPosition);
			Storyboard.SetTargetProperty(anim, new PropertyPath("Point"));

			PointAnimationUsingKeyFrames ptAnim1 = new PointAnimationUsingKeyFrames();
			ptAnim1.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(14.7, 11.5) });
			ptAnim1.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(15, 10.01) });

			Timeline anim1 = ptAnim1;// new PointAnimation();
			//anim1.To = new Point(15, 10.01);
			sb.Children.Add(anim1);
			Storyboard.SetTarget(anim1, pacman.mouseBottomPosition);
			Storyboard.SetTargetProperty(anim1, new PropertyPath("Point"));

			//DoubleAnimation dblAnim = new DoubleAnimation();
			//sb.Children.Add(dblAnim);
			//dblAnim.To = 7;
			//Storyboard.SetTarget(dblAnim, this);
			//Storyboard.SetTargetProperty(dblAnim, new PropertyPath("PacmanWidth"));

			sb.AutoReverse = true;
			sb.Completed += delegate
			{
				System.Diagnostics.Debug.WriteLine("Completed");
				if (_animforever)
				{
					System.Diagnostics.Debug.WriteLine("reBegin");
					StartAnimation();
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("_animationInProgress = false");
					_animationInProgress = false;
				}
			};
			sb.Begin();
		}

		private Game CurrentGame
		{
			get
			{
				return ((GamePresenter)this.DataContext).CurrentGame;
			}
		}

		void PacmanUC_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Back:
					e.Handled = true;
					pacmanSpetialKey(PacmanKey.Back);
					break;
				case Key.Enter:
					e.Handled = true;
					pacmanKey('\n');
					break;
				case Key.Space:
					e.Handled = true;
					if (CurrentGame.Status != GameMode.PLAY)
					{
						pacmanSpetialKey(PacmanKey.Space);
					}
					else pacmanKey(' ');
					break;
				case Key.Up:
					e.Handled = true;
					pacmanSpetialKey(PacmanKey.Up);
					break;
				case Key.Left:
					e.Handled = true;
					pacmanSpetialKey(PacmanKey.Left);
					break;
				case Key.Right:
					e.Handled = true;
					pacmanSpetialKey(PacmanKey.Right);
					break;
				case Key.Down:
					e.Handled = true;
					pacmanSpetialKey(PacmanKey.Down);
					break;
				case Key.Escape:
					e.Handled = true;
					pacmanSpetialKey(PacmanKey.Escape);
					break;
			}
			if (Keyboard.Modifiers == ModifierKeys.Control)
				switch (e.Key)
				{
					case Key.R:
						e.Handled = true;
						_gamepresenter.CurrentGame.setGhostsState(GhostState.REST);
						break;
					case Key.P:
						e.Handled = true;
						pacmanSpetialKey(PacmanKey.Pause);
						break;
					case Key.F:
						e.Handled = true;
						Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
						break;
					case Key.S:
						e.Handled = true;
						_gamepresenter.isSoundMuted = true;
						_gamepresenter.isMusicMuted = true;
						break;
					case Key.N:
#if DEBUG
						e.Handled = true;
						pacmanSpetialKey(PacmanKey.NextLevel);
#endif
						break;
					default:
						//base.OnPreviewKeyDown(e);
						break;
				}
		}

		#region IAudio Members

		class pacUri : Uri
		{
			public pacUri(string name):base("Music/"+name,UriKind.RelativeOrAbsolute)
			{
			
			}
		}

		void IAudio.PlayGoodPill(double speed)
		{
//			pillPlay.playUri(true, new pacUri("pacman_goodPill.mp3"));
			 StreamResourceInfo sr = Application.GetResourceStream(new pacUri("pacman_goodPill.mp3"));
			 speedPillElem.SetSource(new WaveMediaStreamSource(sr.Stream, speed));
		}

		void IAudio.PlayWrongPill()
		{
			pillPlay.playUri(true, new pacUri("pacman_wrongPill.mp3"));
		}


		void IAudio.PlayBigPill()
		{
			soundPlay.playUri(new pacUri("pacman_ghostScared.mp3"));
		}

		void IAudio.PlayPill()
		{
			soundPlay.playUri(new pacUri("pacman_chomp.mp3"));
		}

		void IAudio.PlayBonus()
		{
			soundPlay.playUri(new pacUri("pacman_eatfruit.mp3"));
		}

		void IAudio.PlayEatGhost()
		{
			soundPlay.playUri(new pacUri("pacman_eatghost.mp3"));
		}

		void IAudio.PlayExtraPac()
		{
			soundPlay.playUri(new pacUri("pacman_extrapac.mp3"));
		}

		void IAudio.PlayKill()
		{
			soundPlay.playUri(true, new pacUri("pacman_kill.mp3"));
		}

		void IAudio.PlayGhostScared()
		{
			soundPlay.playUri(new pacUri("pacman_ghostScared.mp3"));
		}


		void IAudio.MusicMenu()
		{
			musicPlay.playUri(false,true,new pacUri("pacman_menu.mp3"));
		}

		void IAudio.MusicGame()
		{
			musicPlay.playUri(false,true,new pacUri("pacman_game.mp3"));
		}

		void IAudio.PlayIntro()
		{
			musicPlay.playUri(true, new pacUri("pacman_beginning.mp3"));
		}

		void IAudio.PlayDeath()
		{
			musicPlay.playUri(true, new pacUri("pacman_death.mp3"));
		}

		void IAudio.PlayMenuPress()
		{
			pillPlay.playUri(true, new pacUri("pacman_eatfruit.mp3"));
		}

		void IAudio.PlayMenuOver()
		{
			soundPlay.playUri(true, new pacUri("pacman_menuover.mp3"));
		}


		void IAudio.MusicMute(bool mute)
		{
			musicPlay.mute(mute);
		}

		void IAudio.SoundMute(bool mute)
		{
			soundPlay.mute(mute);
		}

		#endregion
	}
}
