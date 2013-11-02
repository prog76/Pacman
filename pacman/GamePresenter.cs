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
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Data;

namespace pacman
{
	/// <summary>
	/// this class is the model of view for game 
	/// </summary>
	/// 

	public class TXYPresenter : TXYList
	{
		TPillPoint last;
		public TTaskBox taskBox;

		public TXYPresenter(TTaskBox _taskBox)
		{
			taskBox = _taskBox;
		}

		protected override void intAddHead()
		{
			taskBox.setPoint(fHead.x, fHead.y);
		}
		protected override void intClear(TPillPoint xy)
		{
			if (xy.data != null)
				((TPillGame)xy.data).clrFull();
		}
		protected override void makeFirst(TPillPoint xy)
		{
			last = xy;
			((TPillGame)last.data).isFirst = true;
		}
		protected override void intAdd(TPillPoint xy)
		{
			if (last.data != null)
				((TPillGame)last.data).isFirst = false;
			makeFirst(xy);
			((TPillGame)last.data).pressCnt++;
		}
		public override void clear()
		{
			base.clear();
			intClear(last);
			taskBox.Init();
		}
		public override void remove()
		{
			intClear(tail);
			base.remove();
		}
	}
	
	public class GamePresenter : BasePresenter
	{
		PacmanPresenter _pacman;
		Labyrinth _Labyrinth;
		GhostPresenter[] _ghosts = new GhostPresenter[4];
		BonusPresenter _BonusPresenter;
		int fFingerKey;
		Protect protector;
		DispatcherTimer demoTimer;
		bool fCanPlay;

		public void processKey(TTsk keyTsk){
			fingerKey = CurrentGame.taskBox.tskProvider.tskSelector[keyTsk.tskNo].fingerNums[keyTsk.chr];
			CurrentGame.processKey(keyTsk);
		}

		public GamePresenter()
		{
			_CurrentGame.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_CurrentGame_PropertyChanged);
			_CurrentGame.Timer = new WPFPacmanTimer();
			if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
#if (DEBUG)
#else
				 //HACK BUG VS Designer
				protector = Protect.getInstance();
				protector.Tick += new EventHandler(RegistrationChanged);
#endif
                demoTimer = new DispatcherTimer();
				demoTimer.Interval = TimeSpan.FromMinutes(1);
				demoTimer.Tick += new EventHandler(GameOverTick);
				fCanPlay = true;
				checkRegister();
			}

			_pacman = new PacmanPresenter(CurrentGame);
			_pacman.xyList = new TXYPresenter(CurrentGame.taskBox);
			_BonusPresenter = new BonusPresenter(CurrentGame);
			for (int i = 0; i < 4; i++)
				_ghosts[i] = new GhostPresenter(CurrentGame, i);
		}

		void _CurrentGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentLabyrinth")
			{
				if (_Labyrinth != null)
				{
					_Labyrinth.PillEated -= new PillEatedDelegate(_Labyrinth_PillEated);
					_Labyrinth.PillUpdated -= new PillEatedDelegate(_Labyrinth_PillUpdated);
				}
				_Labyrinth = _CurrentGame.CurrentLabyrinth;
				if (_Labyrinth != null)
				{
					_Labyrinth.PillEated += new PillEatedDelegate(_Labyrinth_PillEated);
					_Labyrinth.PillUpdated += new PillEatedDelegate(_Labyrinth_PillUpdated);
				}
				ConstructWallAndPills(_CurrentGame.CurrentLabyrinth);
			}
			else if (e.PropertyName == "Status")
			{
				NotifyPropertyChanged("IsReady");
				NotifyPropertyChanged("IsGameOver");
			}
			else if (e.PropertyName == "userTypeSpeed")
			{
				if ((_CurrentGame.freqList.freqsSum == 0) && (_CurrentGame.userTypeSpeed > CurrentGame.userMinTypeSpeed))
				{
					_CurrentGame.taskBox.tskProvider.incTskCount();
					_CurrentGame.setGhostsState(GhostState.REST);
					_CurrentGame.Audio.PlayGhostScared();
				}
			}
		}

		void _Labyrinth_PillUpdated(object sender, int x, int y)
		{
			UIElement pill;
			if (CurrentGame._pillCoord.TryGetValue(new Point(x, y), out pill))
			{
				(pill as TPillGame).tsk = this._Labyrinth.chars[x, y];
			}
			else
			{
				pill = null;
			}
		}

		void _Labyrinth_PillEated(object sender, int x, int y)
		{
			UIElement pill;

			if (CurrentGame._pillCoord.TryGetValue(new Point(x, y), out pill))
			{
				this._Labyrinth.laby[y][x] |= 3;
				if ((--(pill as TPillGame).eatCnt) == 0)
					this._Labyrinth.laby[y][x] = 0;
				this._Labyrinth.RemainingPills = TPillGame.RemainingPills;
				NotifyPropertyChanged("Pills");
			}
		}

		private GeometryGroup _Wall = new GeometryGroup();
		/// <summary>
		/// wall geomety of current level
		/// </summary>
		public GeometryGroup Wall
		{
			get { return _Wall; }
			set
			{
				if (value != _Wall)
				{
					_Wall = value;
					NotifyPropertyChanged("Wall");
				}
			}
		}

		private Canvas _Pills = new Canvas();
		/// <summary>
		/// pills of current level
		/// </summary>
		public Canvas Pills
		{
			get { return _Pills; }
			set
			{
				if (value != _Pills)
				{
					_Pills = value;
					NotifyPropertyChanged("Pills");
				}
			}
		}

		/// <summary>
		/// returns true if game is in ready mode
		/// </summary>
		public bool IsReady
		{
			get
			{
				if (CurrentGame.Status == GameMode.PLAY)
				{
					if (PacmanPresenter.Pacman != null)
						PacmanPresenter.Pacman.xyList.clear();
				}
				return CurrentGame.Status == GameMode.READY;
			}
		}

		/// <summary>
		/// returns true if game is in game over mode
		/// </summary>
		public bool IsGameOver
		{
			get { return IsGameOverDeath || (CurrentGame.Status == GameMode.GAME_OVER_ESC); }
		}

		public bool IsGameOverDeath
		{
			get { return CurrentGame.Status == GameMode.GAME_OVER_DEATH; }
		}


		private Game _CurrentGame = new Game();
		/// <summary>
		/// current game, it used for databinding
		/// </summary>
		public Game CurrentGame
		{
			get
			{
				return _CurrentGame;
			}
		}

		/// <summary>
		/// presenter of ghost 1
		/// </summary>
		public GhostPresenter Ghost1
		{
			get
			{
				return _ghosts[0];
			}
		}

		/// <summary>
		/// presenter of ghost 2
		/// </summary>
		public GhostPresenter Ghost2
		{
			get
			{
				return _ghosts[1];
			}
		}

		/// <summary>
		/// presenter of ghost 3
		/// </summary>
		public GhostPresenter Ghost3
		{
			get
			{
				return _ghosts[2];
			}
		}

		/// <summary>
		/// presenter of ghost 4
		/// </summary>
		public GhostPresenter Ghost4
		{
			get
			{
				return _ghosts[3];
			}
		}

		/// <summary>
		/// presenter for pacman
		/// </summary>
		public PacmanPresenter PacmanPresenter
		{
			get
			{
				return _pacman;
			}
		}

		/// <summary>
		/// presenter for bonus
		/// </summary>
		public BonusPresenter BonusPresenter
		{
			get
			{
				return _BonusPresenter;
			}
		}


		public bool isMusicMuted
		{
			get
			{
				return CurrentGame.cfg.isMusicMuted;
			}
			set
			{
				if (isMusicMuted != value)
				{
					CurrentGame.Audio.MusicMute(value);
					CurrentGame.cfg.isMusicMuted = value;
					NotifyPropertyChanged("isMusicMuted");
				}
			}
		}

		public bool isSoundMuted
		{
			get
			{
				return CurrentGame.cfg.isSoundMuted;
			}
			set
			{
				if (isSoundMuted != value)
				{
					CurrentGame.Audio.SoundMute(value);
					CurrentGame.cfg.isSoundMuted = value;
					NotifyPropertyChanged("isSoundMuted");
				}
			}
		}
		public int fingerKey
		{
			get
			{
				return fFingerKey;
			}
			set
			{
				fFingerKey = value;
				NotifyPropertyChanged("fingerKey");
			}
		}

		public bool canPlay()
		{
			if (!fCanPlay)
			{
				openRegisterScreen();
			}
			return fCanPlay;
		}

		private void RegistrationChanged(object sender, EventArgs e)
		{
			checkRegister();
			NotifyPropertyChanged("IsNotRegistered");
		}

		private void GameOverTick(object sender, EventArgs e)
		{
			fCanPlay = false;
			canPlay();
			CurrentGame.KeyPressed(PacmanKey.Escape);
			demoTimer.Stop();
		}

		void checkRegister()
		{
			if (IsNotRegistered){
				if(fCanPlay)demoTimer.Start();
			}else{
				demoTimer.Stop();
				fCanPlay = true;
			}
		}

		public void openRegisterScreen()
		{
			RegScreen.getInstance();
		}

		public bool IsNotRegistered
		{
			get
			{
				return (protector == null) || (!protector.registered);
			}
		}
	

		#region private methods
		/// <summary>
		/// construct geometry of wall and pills for a specific level
		/// </summary>
		/// <param name="newValue"></param>

		private void ConstructWallAndPills(Labyrinth newValue)
		{
			this.Wall.Children.Clear();
			this.Pills.Children.Clear();
			CurrentGame._pillCoord.Clear();
			if (newValue == null)
				return;
			TPillGame.RemainingPills = 0;
			for (int xx = 0; xx < Labyrinth.WIDTH; xx++)
			{
				for (int yy = 0; yy < Labyrinth.HEIGHT; yy++)
				{
					UInt16 b = newValue[xx, yy];
					if (b == 0)
						continue;
					double x, y;
					newValue.GetScreenCoord(xx, yy, out x, out y);
					switch (b)
					{
						case 1://pill
						case 2:
							TPillGame pill = new TPillGame(this);
							this.Pills.Children.Add(pill);
							CurrentGame._pillCoord.Add(new Point(xx, yy), pill);
							pill.Width = Constants.GRID_WIDTH*1.5;
							pill.Height = Constants.GRID_HEIGHT*1.5;
							pill.tsk = newValue.chars[xx, yy];
							pill.center = new Point(x + Constants.GRID_WIDTH_2, y + Constants.GRID_HEIGHT_2);
							pill.init();
							pill.isBig = (b == 2);
							break;
						case 4: //gate of ghost's house
							break;
						case 5://wall
							Geometry current = GetGeometry(xx, yy, (int)x, (int)y, newValue);
							//if (current == null)
							//{
							//    RectangleGeometry r = new RectangleGeometry();
							//    //if (i == 0)
							//    //    r.Rect = new Rect(x + Constants.GRID_WIDTH_2, y, Constants.GRID_WIDTH_2, Constants.GRID_HEIGHT);
							//    //else
							//    r.Rect = new Rect(x, y, Constants.GRID_WIDTH, Constants.GRID_HEIGHT);
							//    current = r;
							//}
							if (current != null)
								this.Wall.Children.Add(current);
							break;
					}
				}
			}

		}

		private Geometry GetGeometry(int i, int j, int xScreen, int yScreen, Labyrinth laby)
		{
			if (i == 0 && j == 0)
			{
				PathGeometry path = new PathGeometry();
				PathFigure pf = new PathFigure() { IsClosed = true, IsFilled = true };
				path.Figures.Add(pf);
				pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT);
				pf.Segments.Add(new ArcSegment()
				{
					Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
					Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT_2),
					SweepDirection = SweepDirection.Clockwise
				});
				pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
				return path;
			}
			else
			{
				RectangleGeometry r;
				PathGeometry corner;
				PathFigure pf;
				int OffsetX;
				int OffsetY;
				WallSide side = GetSide(i, j, laby);
				switch (side)
				{
					case WallSide.InnerTopLeft:
					case WallSide.TopLeft:
						OffsetX = side == WallSide.InnerTopLeft ? Constants.GRID_WIDTH_2 : 0;
						OffsetY = side == WallSide.InnerTopLeft ? Constants.GRID_HEIGHT_2 : 0;
						corner = new PathGeometry();
						pf = new PathFigure() { IsClosed = true, IsFilled = true };
						corner.Figures.Add(pf);
						pf.StartPoint = new Point(xScreen + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY);
						pf.Segments.Add(new ArcSegment()
						{
							Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
							Point = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + OffsetY),
							SweepDirection = SweepDirection.Clockwise
						});
						if (side == WallSide.InnerTopLeft)
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
						}
						else
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT) });
						}
						return corner;
					case WallSide.InnerTopRight:
					case WallSide.TopRight:
						OffsetX = side == WallSide.InnerTopRight ? -Constants.GRID_WIDTH_2 : 0;
						OffsetY = side == WallSide.InnerTopRight ? Constants.GRID_HEIGHT_2 : 0;
						corner = new PathGeometry();
						pf = new PathFigure() { IsClosed = true, IsFilled = true };
						corner.Figures.Add(pf);
						pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + OffsetY);
						pf.Segments.Add(new ArcSegment()
						{
							Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
							Point = new Point(xScreen + Constants.GRID_WIDTH + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY),
							SweepDirection = SweepDirection.Clockwise
						});
						if (side == WallSide.InnerTopRight)
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT) });
						}
						else
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen) });
						}
						return corner;
					case WallSide.InnerBottomRight:
					case WallSide.BottomRight:
						OffsetX = side == WallSide.InnerBottomRight ? -Constants.GRID_WIDTH_2 : 0;
						OffsetY = side == WallSide.InnerBottomRight ? -Constants.GRID_HEIGHT_2 : 0;
						corner = new PathGeometry();
						pf = new PathFigure() { IsClosed = true, IsFilled = true };
						corner.Figures.Add(pf);
						pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY);
						pf.Segments.Add(new ArcSegment()
						{
							Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
							Point = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + Constants.GRID_HEIGHT + OffsetY),
							SweepDirection = SweepDirection.Clockwise
						});
						if (side == WallSide.InnerBottomRight)
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen) });
						}
						else
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen) });
						}
						return corner;
					case WallSide.InnerBottomLeft:
					case WallSide.BottomLeft:
						OffsetX = side == WallSide.InnerBottomLeft ? Constants.GRID_WIDTH_2 : 0;
						OffsetY = side == WallSide.InnerBottomLeft ? -Constants.GRID_HEIGHT_2 : 0;
						corner = new PathGeometry();
						pf = new PathFigure() { IsClosed = true, IsFilled = true };
						corner.Figures.Add(pf);
						pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + Constants.GRID_HEIGHT + OffsetY);
						pf.Segments.Add(new ArcSegment()
						{
							Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
							Point = new Point(xScreen + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY),
							SweepDirection = SweepDirection.Clockwise
						});
						if (side == WallSide.InnerBottomLeft)
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen) });
						}
						else
						{
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT_2) });
							pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
						}
						return corner;
					case WallSide.Left:
						r = new RectangleGeometry();
						r.Rect = new Rect(xScreen, yScreen, Constants.GRID_WIDTH_2, Constants.GRID_HEIGHT);
						return r;
					case WallSide.Right:
						r = new RectangleGeometry();
						r.Rect = new Rect(xScreen + Constants.GRID_WIDTH_2, yScreen, Constants.GRID_WIDTH_2, Constants.GRID_HEIGHT);
						return r;
					case WallSide.Top:
						r = new RectangleGeometry();
						r.Rect = new Rect(xScreen, yScreen, Constants.GRID_WIDTH, Constants.GRID_HEIGHT_2);
						return r;
					case WallSide.Bottom:
						r = new RectangleGeometry();
						r.Rect = new Rect(xScreen, yScreen + Constants.GRID_HEIGHT_2, Constants.GRID_WIDTH, Constants.GRID_HEIGHT_2);
						return r;
					default:
						return null;
				}
			}
		}

		enum WallSide { None, TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, InnerTopLeft, InnerTopRight, InnerBottomRight, InnerBottomLeft };


		private WallSide GetSide(int i, int j, Labyrinth laby)
		{
			//first column
			if (i == 0)
			{
				if (j == 0)
					return WallSide.InnerTopLeft;
				if (j == Labyrinth.HEIGHT - 1)
					return WallSide.InnerBottomLeft;
				if (laby[i + 1, j] == 5 && laby[i, j - 1] == 5 && laby[i + 1, j - 1] != 5)
					return WallSide.InnerBottomLeft;
				if (laby[i + 1, j] == 5 && laby[i, j + 1] == 5 && laby[i + 1, j + 1] != 5)
					return WallSide.InnerTopLeft;
				if (laby[i, j - 1] == 5 && laby[i, j + 1] == 5 && (laby[i + 1, j] == 5 || laby[i + 1, j] == 4 || laby[i + 1, j] == 0))
					return WallSide.None;
				return WallSide.Right;
			}
			//last column
			else if (i == Labyrinth.WIDTH - 1)
			{
				if (j == 0)
					return WallSide.InnerTopRight;
				if (j == Labyrinth.HEIGHT - 1)
					return WallSide.InnerBottomRight;
				if (laby[i - 1, j] == 5 && laby[i, j - 1] == 5 && laby[i - 1, j - 1] != 5)
					return WallSide.InnerBottomRight;
				if (laby[i - 1, j] == 5 && laby[i, j + 1] == 5 && laby[i - 1, j + 1] != 5)
					return WallSide.InnerTopRight;

				return WallSide.Left;
			}
			//first row
			else if (j == 0)
			{
				if (laby[i + 1, j + 1] != 5 && laby[i, j + 1] == 5)
					return WallSide.InnerTopLeft;
				if (laby[i - 1, j + 1] != 5 && laby[i, j + 1] == 5)
					return WallSide.InnerTopRight;
				if (laby[i - 1, j] == 5 && laby[i + 1, j] == 5 && (laby[i, j + 1] == 5 || laby[i, j + 1] == 0))
					return WallSide.None;

				return WallSide.Bottom;
			}
			//last row
			else if (j == Labyrinth.HEIGHT - 1)
			{
				if (laby[i - 1, j - 1] != 5 && laby[i, j - 1] == 5)
					return WallSide.InnerBottomRight;
				if (laby[i + 1, j - 1] != 5 && laby[i, j - 1] == 5)
					return WallSide.InnerBottomLeft;
				if (laby[i - 1, j] == 5 && laby[i + 1, j] == 5 && (laby[i, j - 1] == 5 || laby[i, j - 1] == 0))
					return WallSide.None;
				return WallSide.Top;
			}
			else if (j > 0 && j < Labyrinth.HEIGHT - 1 && i > 0 && i < Labyrinth.WIDTH - 1)
			{
				//Corners
				if (laby[i - 1, j - 1] != 5 && laby[i - 1, j] != 5 && laby[i, j - 1] != 5)
					return WallSide.TopLeft;
				if (laby[i + 1, j - 1] != 5 && laby[i + 1, j] != 5 && laby[i, j - 1] != 5)
					return WallSide.TopRight;
				if (laby[i + 1, j + 1] != 5 && laby[i + 1, j] != 5 && laby[i, j + 1] != 5)
					return WallSide.BottomRight;
				if (laby[i - 1, j + 1] != 5 && laby[i - 1, j] != 5 && laby[i, j + 1] != 5)
					return WallSide.BottomLeft;
				//Top and Bottom
				if (laby[i, j - 1] != 5 && laby[i, j - 1] != 4)
					return WallSide.Top;
				if (laby[i, j + 1] != 5)
					return WallSide.Bottom;
				//Left and Right
				if (laby[i - 1, j] != 5 && laby[i - 1, j] != 4)
					return WallSide.Left;
				if (laby[i + 1, j] != 5)
					return WallSide.Right;
				//inner corners
				if (laby[i - 1, j - 1] != 5)
					return WallSide.InnerBottomRight;
				if (laby[i + 1, j + 1] != 5)
					return WallSide.InnerTopLeft;

				if (laby[i - 1, j + 1] != 5)
					return WallSide.InnerTopRight;

				if (laby[i + 1, j - 1] != 5)
					return WallSide.InnerBottomLeft;

			}
			return WallSide.None;
		}

		#endregion
	}
}
