using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using GeniusPacman.Core.Sprites;
using GeniusPacman.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Runtime.InteropServices;

using SoftConsept.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.IO.IsolatedStorage;

namespace GeniusPacman.Core
{

	public class XY
	{
		public int x, y;
		public XY(int xx, int yy)
		{
			x = xx;
			y = yy;
		}
	}

	public struct TPillPoint
	{
		public UInt16 x, y;
		public Object data;
		public TPillPoint(int _x, int _y, Object _data)
		{
			x = (UInt16)_x;
			y = (UInt16)_y;
			data = _data;
		}
	}
	public class TXYListInt : List<TPillPoint>
	{
	}
	public class TXYList : TXYListInt
	{
		protected TPillPoint fHead, fTail;
		bool fEmpty;
		public TXYList()
		{
			fEmpty = true;
		}
		protected virtual void intAddHead()
		{
		}
		protected virtual void makeFirst(TPillPoint xy)
		{
		}
		protected virtual void intClear(TPillPoint xy)
		{
		}
		protected virtual void intAdd(TPillPoint xy)
		{
		}
		public virtual void clear()
		{
			fEmpty = true;
			foreach (TPillPoint xy in this)
			{
				intClear(xy);
			}
			intClear(fHead);
			Clear();
		}
		public void deAdd()
		{
			if (fEmpty) return;
			intClear(fHead);
			RemoveAt(Count - 1);
			fEmpty = (Count == 0);
			if (fEmpty) return;
			head = base[Count - 1];
			makeFirst(fHead);
		}
		public void add(TPillPoint xy)
		{
			head = xy;
			if (fEmpty)
			{
				fTail = xy;
			}
			Add(xy);
			intAdd(xy);
			fEmpty = false;
		}
		public virtual void remove()
		{
			RemoveAt(0);
			fEmpty = (Count == 0);
			if (!fEmpty)
			{
				fTail = base[0];
			}
		}
		public bool empty
		{
			get { return fEmpty; }
		}
		public TPillPoint head
		{
			get { return fHead; }
			set
			{
				fHead = value;
				intAddHead();
			}
		}
		public void initHead(TPillPoint value)
		{
			{
				fHead = value;
			}
		}
		public TPillPoint tail
		{
			get { return fTail; }
		}
	};


	public enum GameMode { GAME_OVER_DEATH, READY, PLAY, DEATH, GAME_OVER_ESC };
	// Classe de gestion du jeu

	public class TLives : ObservableCollection<int>
	{
	};

	public class GameRecord
	{
		public int tim;
		public PacmanKey key;
		public TTsk tsk;
		public bool isKey;
		public GameRecord(int _tim, TTsk _tsk)
		{
			tim = _tim;
			tsk = _tsk;
			isKey = false;
		}
		public GameRecord(int _tim, PacmanKey _key)
		{
			tim = _tim;
			key = _key;
			isKey = true;
		}
		public GameRecord()
		{
		}
	}

	public class GameRecords : List<GameRecord>
	{
	}

	public class Game : Notifiable
	{

		TInterval turnInt;

		StoredConfig _cfg;

		public StoredConfig cfg
		{
			get
			{
				return _cfg;
			}
		}

		private const int FANTOME_FIRST = 0;

		#region fields
		// le jeu est t-il en pause ?
		private bool _paused = false;

		// compteur de tour de jeu pour un état donné (game over, ready, ...)
		private int turn = 0, oturn = 0;
		public int lastKeyTurn = 0;

		// état du jeu
		private GameMode _mode = GameMode.GAME_OVER_ESC;

		//TODO use a level manager
		// index of labyrinth in current use
		private int numLaby = 0;

		// Vitesse du jeu
		private int _sleepTime = Constants.INITIAL_SLEEP_TIME;
		double _speedDevisor;
		private int _level = 1;

		// nombre de vies
		private int _lives;

		// objet pacman et fantomes
		private Pacman _Pacman = null;
		private List<Ghost> _Ghosts;
		private Bonus _Bonus = null;

		// liste des points évoluant vers le haut
		private ObservableCollection<FlyingScore> _FlyingScores = null;
		double fUserTypeSpeed, fUserMinTypeSpeed, lastSpeed,lastMomentSpeed;
		int fUserCharCount;
		/// <summary>
		/// current score
		/// </summary>
		private int _Score = 0;
		/// <summary>
		/// high socre
		/// </summary>
		private int _HighScore = 0;
		/// <summary>
		/// score to obtain an another life
		/// </summary>
		private int _LifeScore;
		/// <summary>
		/// points when you eat a ghost
		/// </summary>
		private int _GhostScore;
		/// <summary>
		/// timer to animate the game
		/// </summary>
		private IPacmanTimer _Timer;
		private TTaskBox fTaskBox;

		private TLives fLives;
		private TTskFreqList fFreqList;

		#endregion
		public Dictionary<Point, UIElement> _pillCoord = new Dictionary<Point, UIElement>();

		public Game()
		{
			fLives = new TLives();
			turnInt = new TInterval();
			_FlyingScores = new ObservableCollection<FlyingScore>();
			fFreqList = new TTskFreqList();
			fTaskBox = new TTaskBox(this);
			SetMode(GameMode.GAME_OVER_ESC);

			xyDirs = new TXYDir[Direction.directions.Length];
			for (int i = 0; i < xyDirs.Length; i++)
				xyDirs[i] = new TXYDir(Direction.directions[i]);
			load();
			foreach (TTskSet tsk in cfg.tskSetsSpetial)
			{
				taskBox.tskProvider.tskSelector.tskAdd(tsk);
			}
		}

		#region properties

		public TTaskBox taskBox
		{
			get { return fTaskBox; }
		}

		/// <summary>
		/// You must provide a timer to animate the game
		/// </summary>

		public IPacmanTimer Timer
		{
			get
			{
				return _Timer;
			}
			set
			{
				if (_Timer != value)
				{
					if (_Timer != null)
					{
						_Timer.Tick -= new EventHandler(_Timer_Tick);
					}
					_Timer = value;
					if (_Timer != null)
					{
						_Timer.Tick += new EventHandler(_Timer_Tick);
						_Timer.Elapsed = SleepTime;
					}
					DoPropertyChanged("Timer");
				}
			}
		}

		void _Timer_Tick(object sender, EventArgs e)
		{
			//TODO synchronize with UI thread, user will provide a delegate
			Animate();
		}

		public double userTypeSpeed
		{
			get
			{
				return Math.Round(fUserTypeSpeed);
			}
			set
			{
				double old = userTypeSpeed;
				fUserTypeSpeed = Math.Max(20,value);
				if (old != userTypeSpeed)
				{
					DoPropertyChanged("userTypeSpeed");
				}
			}
		}

		public double userMinTypeSpeed
		{
			get
			{
				return Math.Round(fUserMinTypeSpeed * 0.50 / 10) * 10;
			}
			set
			{
				double old = userMinTypeSpeed;
				fUserMinTypeSpeed = Math.Max(20,value);
				if (old != userMinTypeSpeed)
				{
					_speedDevisor = (1000.0 / SleepTime) / (4.0 * fUserMinTypeSpeed / 60);
					DoPropertyChanged("userMinTypeSpeed");
				} 
			}
		}

		public TTskFreqList freqList
		{
			get
			{
				return fFreqList;
			}
		}

		public int userCharCount
		{
			get { return fUserCharCount; }
			set
			{
				if (value != fUserCharCount)
				{
					DoPropertyChanged("userCharCount");
					fUserCharCount = value;
				}
			}
		}

		/// <summary>
		/// Your score
		/// </summary>
		public int Score
		{
			get
			{
				return _Score;
			}
			private set
			{
				if (_Score != value)
				{
					_Score = value;
					DoPropertyChanged("Score");
				}
			}
		}

		/// <summary>
		/// High score
		/// </summary>
		public int HighScore
		{
			get
			{
				return _HighScore;
			}
			private set
			{
				if (_HighScore != value)
				{
					_HighScore = value;
					DoPropertyChanged("HighScore");
				}
			}
		}

		/// <summary>
		/// Score to obtain one more life
		/// </summary>
		public int LifeScore
		{
			get
			{
				return _LifeScore;
			}
			private set
			{
				if (value != _LifeScore)
				{
					_LifeScore = value;
					DoPropertyChanged("LifeScore");
				}
			}
		}

		/// <summary>
		/// how points you win, when you eat a ghost
		/// </summary>
		public int FantScore
		{
			get
			{
				return _GhostScore;
			}
			private set
			{
				if (value != _GhostScore)
				{
					_GhostScore = value;
					DoPropertyChanged("FantScore");
				}
			}
		}

		/// <summary>
		/// your lifes
		/// </summary>
		public int Lives
		{
			get
			{
				return _lives;
			}
			private set
			{
				if (value != _lives)
				{
					_lives = value;
					while (fLives.Count > value) fLives.RemoveAt(0);
					while (fLives.Count < value) fLives.Add(new int());
					DoPropertyChanged("Lives");
				}
			}
		}

		/// <summary>
		/// true when game is paused
		/// </summary>
		public bool Paused
		{
			get
			{
				return _paused;
			}
			private set
			{
				if (value != _paused)
				{
					_paused = value;
					DoPropertyChanged("Paused");
				}
			}
		}

		/// <summary>
		/// level of current game
		/// </summary>
		public int Level
		{
			get
			{
				return _level;
			}
			private set
			{
				if (_level != value)
				{
					_level = value;
					DoPropertyChanged("Level");
				}
			}
		}

		/// <summary>
		/// the elapsed time used to animate the game
		/// </summary>
		public int SleepTime
		{
			get
			{
				return _sleepTime;
			}
			private set
			{
				if (value != _sleepTime)
				{
					_sleepTime = value;
					if (_Timer != null)
						_Timer.Elapsed = value;
					DoPropertyChanged("SleepTime");
				}
			}
		}

		/// <summary>
		/// status of the game (game over, ready, playing...)
		/// </summary>
		public GameMode Status
		{
			get
			{
				return _mode;
			}
			private set
			{
				if (_mode != value)
				{
					_mode = value;
					DoPropertyChanged("Status");
				}
			}
		}

		public Pacman PacMan
		{
			get
			{
				return _Pacman;
			}
			private set
			{
				if (value != _Pacman)
				{
					_Pacman = value;
					DoPropertyChanged("PacMan");
				}
			}
		}

		public List<Ghost> Ghosts
		{
			get
			{
				return _Ghosts;
			}
			private set
			{
				if (_Ghosts != value)
				{
					_Ghosts = value;
					DoPropertyChanged("Ghosts");
				}
			}
		}

		public Bonus CurrentBonus
		{
			get
			{
				return _Bonus;
			}
			private set
			{
				if (_Bonus != value)
				{
					_Bonus = value;
					DoPropertyChanged("CurrentBonus");
				}
			}
		}

		public int Width
		{
			get
			{
				return Constants.GRID_WIDTH * Labyrinth.WIDTH;
			}
		}

		public int Height
		{
			get
			{
				return Constants.GRID_HEIGHT * Labyrinth.HEIGHT;
			}
		}

		public Labyrinth CurrentLabyrinth
		{
			get
			{
				return Labyrinth.getCurrent();
			}
		}

		public TLives pLives
		{
			get
			{
				return fLives;
			}
		}

		public ObservableCollection<FlyingScore> FlyingScores
		{
			get
			{
				return _FlyingScores;
			}
		}
		#endregion

		public static FastRandom rand = new FastRandom(1);

		internal static int GetRandom(int max)
		{
			return (int)(rand.NextDouble() * (double)max);
		}

		#region private methods
		private void DrawReady()
		{
			//draw.drawReady
		}

		// Le jeu est dans l'état GAMEOVER
		private void AnimateGameOver()
		{
			//draw.drawBack();
			//drawScore();
		}

		// Le jeu est dans l'état READY
		private void AnimateReady()
		{
			if (turn == 20)
			{
				// passe en mode de jeu
				//draw.clearReady();
				SetMode(GameMode.PLAY);
			}
			//draw.drawBack();
			DrawScore();
		}
		bool speedCheck(Double k)
		{
			double k1 = _speedDevisor / k;
			return (((int)((turn) / k1) - (int)((turn - 1) / k1)) > 0);
		}
		// Le jeu est dans l'état PLAY
		private void AnimatePlay()
		{
			GameRecord record;
			while (_cfg.play.Count > 0)
			{
				record = _cfg.play[0];
				if (record.tim > oturn) break;
				_cfg.play.RemoveAt(0);
				_cfg.playerCfg.record.Add(record);
				if (record.tim == oturn)
				{
					if (record.isKey)
						switch (record.key)
						{
							case PacmanKey.Back:
								{
									TXYList xyList = _Pacman.xyList;
									if (!xyList.empty) xyList.deAdd();
								}
								break;
							case PacmanKey.Left:
								_Pacman.Left();
								break;
							case PacmanKey.Right:
								_Pacman.Right();
								break;
							case PacmanKey.Down:
								_Pacman.Down();
								break;
							case PacmanKey.Up:
								_Pacman.Up();
								break;
						}
					else
						intProcessKey(record.tsk);
				}
			}

			if (Labyrinth.getCurrent().RemainingPills <= 0)
			{
				// toutes les pastilles ont été mangées, changement de niveau
				// accélère un peu le jeu
				Level++;
				SleepTime -= 1;
				numLaby = (numLaby + 1) % 4;
				SetLabyrinth(numLaby);
				SetMode(GameMode.READY);
			}
			else
			{
				int i;
				int xp, yp;
				GhostState state;

				EraseSprites();
				ErasePoints();

				// testes de collisions
				xp = _Pacman.X;
				yp = _Pacman.Y;
				// collisions avec les fantomes
				foreach (Ghost ghost in _Ghosts)
				{
					state = ghost.State;
					// teste si le Pacman est en collision avec le Fantome i
					if (_Pacman.HasCollision(ghost))
					{
						if (state.isHunt() || state.isRandom())
						{
							// Perdu une vie
							SetMode(GameMode.DEATH);
							return;
						}
						else if (state.isFlee())
						{
							// le fantome est mangé
							ghost.setState(GhostState.EYE);
							AddScore(_GhostScore);
							FantScore *= 2;
							AddPoints(_GhostScore);
							if (Audio != null)
								Audio.PlayEatGhost();
						}
					}
				}
				// collisions avec le bonus
				if (CurrentBonus != null)
				{
					// le bonus existe
					if (_Pacman.HasCollision(CurrentBonus))
					{
						int points = Math.Min(1600, Level * 100);
						AddScore(points);
						AddPoints(points);
						CurrentBonus = null;
						if (Audio != null)
							Audio.PlayBonus();
					}
					if (CurrentBonus != null && CurrentBonus.animate() <= 0)
					{
						// le bonus est en fin de vie
						CurrentBonus = null;
					}
				}
				else
				{
					// pas de bonus
					if (Game.GetRandom(1000) < 5)
					{
						CurrentBonus = new Bonus(Level - 1);
					}
				}

				// animate pacman
				if (speedCheck(Config.SpeedPacman))
				{
					i = _Pacman.animate();
					if (i == 1)
					{
						// une pastille est mangée
						AddScore(10);
						Audio.PlayPill();
					}
					else if (i == 2)
					{
						FantScore = 100;
						// Les fantomes qui chasse deviennent fuyards
						AddScore(50);
						Audio.PlayBigPill();
						setGhostsState(GhostState.FLEE);
					}
				}
				// animate fantomes 
				//      bool siren = false;

				foreach (Ghost ghost in _Ghosts)
				{
					switch (ghost.State.ToEnum())
					{
						case GhostStateEnum.FLEE:
							ghost.speedDevider = Config.GhostSpeedFlee;
							break;
						case GhostStateEnum.EYE:
							ghost.speedDevider = Config.GhostSpeedEye;
							break;
						default:
							if (Math.Sqrt((_Pacman.xLaby - ghost.xLaby) ^ 2 + (_Pacman.yLaby - ghost.yLaby) ^ 2) < Config.SafeRadius)
							{
								if (ghost.State.isHunt())
								{
									if (GetRandom(Config.GhostSpeedSwitchTime) == 2)
									{
										if (GetRandom(1) == 1) ghost.speedDevider = Config.GhostSpeedHunt;
										else ghost.speedDevider = Config.GhostSpeedHuntPulse;
									}
								}
								else ghost.speedDevider = Config.GhostSpeed;
							}
							else ghost.speedDevider = Config.GhostSpeedOutSafe;
							break;
					}

					if (!speedCheck(ghost.speedDevider))
					{
						ghost.timers();
						continue;
					}
					ghost.setPacmanCoor(xp, yp);
					ghost.animate();
					//        if (!siren && ghosts[i].getState().isHunt()) {
					//          siren = true;
					//          Audio.playSiren();
					//        }
				}
 
				// animate points
				foreach (FlyingScore pt in _FlyingScores)
				{
					pt.animate();
				}

				DrawSprites();
				DrawPoints();
				DrawScore();
				DrawLives();
				//if (DEBUG)
				//{
				//    foreach(Ghost ghost in _Ghosts)
				//    {
				//        //draw.drawDebugString(4, 12 + i * 12, 12, ghosts[i].toString());
				//    }
				//}
			}
		}

		private void SetLabyrinth(int numLaby)
		{
			Labyrinth.SetCurrent(numLaby);
			taskBox.lab = Labyrinth.getCurrent();
			DoPropertyChanged("CurrentLabyrinth");
		}

		// Le jeu est dans l'état DEATH
		private void AnimateDeath()
		{
			if (turn == 70)
			{
				if (Lives == 0)
				{
					SetMode(GameMode.GAME_OVER_DEATH);
				}
				else
				{
					SetMode(GameMode.PLAY);
				}
			}
		}


		private void DrawSprites()
		{
			//if (_Bonus != null)
			//{
			//    _Bonus.draw();
			//}
			//// Affiche le Pacman
			//_Pacman.draw();
			//// Affiche les fantomes
			//foreach (Ghost ghost in _Ghosts)
			//{
			//    ghost.draw();
			//}
		}

		private void EraseSprites()
		{
			//if (_Bonus != null)
			//{
			//    _Bonus.restore();
			//}
			//_Pacman.restore();
			//foreach (Ghost ghost in _Ghosts)
			//{
			//    ghost.restore();
			//}
		}

		private void AddPoints(int points)
		{
			this._FlyingScores.Add(new FlyingScore(_Pacman.X, _Pacman.Y, (points / 100) - 1));
		}

		private void DrawScore()
		{
			if (Score > HighScore)
			{
				HighScore = Score;
			}
			//draw.drawScore(score, meilleurScore);
		}

		private void DrawPoints()
		{

			// Parcours la liste des Points et affiche
			for (int i = 0; i < _FlyingScores.Count; i++)
			{
				FlyingScore point = _FlyingScores[i];
				if (point.count() > 0)
				{
					//point.draw();
				}
				else
				{
					_FlyingScores.RemoveAt(i);
					i--;
				}
			}
		}

		private void ErasePoints()
		{
			//// Parcours la liste des Points et restaure le fond
			//foreach (FlyingScore pt in FlyingScores)
			//{
			//    pt.restore();
			//}
		}

		private void DrawLives()
		{
			//draw.drawLives(lives, numLaby);
		}

		private void AddScore(int num)
		{
			Score += num;
			if (Score > LifeScore)
			{
				Lives++;
				Audio.PlayExtraPac();
				LifeScore += 2 * LifeScore;
			}
		}

		private void Animate()
		{
			if (!Paused)
			{

				long delta = turnInt.getMsFilt(0.1);

				turn++;
				oturn++;

				switch (Status)
				{
					case GameMode.GAME_OVER_DEATH:
						AnimateGameOver();
						break;
					case GameMode.READY:
						AnimateReady();
						break;
					case GameMode.PLAY:
						AnimatePlay();
						break;
					case GameMode.DEATH:
						AnimateDeath();
						break;
				}
			}
		}
		/// <summary>
		/// changes the game's state
		/// </summary>
		/// <param name="newMode"></param>
		private void SetMode(GameMode newMode)
		{
			oturn = oturn + 10000;
			turn = 0;
			GameMode oldStatus = Status;
			switch (newMode)
			{
				case GameMode.GAME_OVER_DEATH:
				case GameMode.GAME_OVER_ESC:
					if (newMode == GameMode.GAME_OVER_DEATH)
						cfg.playerCfg.scoreNo = cfg.scores.add(cfg.playerName, Score)+1;
					if (Audio != null)
					{
						if (newMode == GameMode.GAME_OVER_DEATH)
							Audio.PlayDeath();
						Audio.MusicMenu();
					}
					numLaby = 0;
					SetLabyrinth(numLaby);
					if ((oldStatus == GameMode.PLAY) || (oldStatus == GameMode.DEATH))
					{
						save();
					}
					SleepTime = Constants.INITIAL_SLEEP_TIME;
					if (Timer != null && Timer.IsStarted)Timer.Stop();
					break;
				case GameMode.READY:
					if (Audio != null)
					{
						Audio.PlayIntro();
						Audio.MusicGame();
					}
					// copie le niveau dans le labyrinthe courant
					DrawReady();
					// init du laby courant
					SetLabyrinth(numLaby);
					DrawLives();
					if (Timer == null)
						throw new Exception("You must provide a timer, fill 'Timer' property");
					if (!Timer.IsStarted)
						Timer.Start();
					break;
				case GameMode.PLAY:
					//draw.drawBack();
					if (PacMan == null)
						PacMan = new Pacman();
					else
						PacMan.Init();
					//					taskBox.tskProvider.checkSpeed(-1);//init with default speed value
					List<Ghost> tmpList = new List<Ghost>();
					CurrentBonus = null;
					for (int i = 0; i < cfg.ghostCount; i++)
					{
						tmpList.Add(new Ghost(FANTOME_FIRST + i));
					}
					Ghosts = tmpList;
					break;
				case GameMode.DEATH:
					Lives--;
					Audio.PlayKill();
					_FlyingScores.Clear();
					if (PacMan != null)
						PacMan.Dead();
					break;
			}
			Status = newMode;
		}
		public void setGhostsState(GhostState state)
		{
			foreach (Ghost ghost in _Ghosts)
			{
				while (!ghost.OnGrid()) ghost.animate();
				ghost.setState(state);
			}
		}
		public IAudio Audio { get; set; }
		#endregion

		#region public methods

		public void initGame()
		{
			numLaby = 0;
			Level = 1;
			SetLabyrinth(numLaby);
			Lives = Constants.INIT_LIVES;
			LifeScore = 5000;
			Score = 0;
		}
		void applyConfig(StoredConfig cfg)
		{
			taskBox.tskProvider.tskSelector.tskClear();
			freqList.init();

			foreach (int tskNo in cfg.playerCfg.tskSets)
			{
				taskBox.tskProvider.tskSelector.tskAdd(cfg.tskSets[tskNo]);
			}

			taskBox.tskProvider.tskSelector.skip.init();
			taskBox.tskProvider.tskSelector.skip.skipAlways(0);

			rand.Reinitialise(cfg.seed);
			taskBox.tskProvider.tskCount = cfg.cfg.tskCount;
			Lives = cfg.cfg.lives;

			for (int i = 0; i < cfg.cfg.freqs.Count; i++)
				taskBox.tskProvider.tskSelector.tskList[i].freq.freq = (double[])cfg.cfg.freqs[i].Clone();

			userMinTypeSpeed = cfg.cfg.minTypeSpeed;
			userTypeSpeed = cfg.cfg.typeSpeed;
			cfg.playerCfg.replay = cfg.cfg;
			cfg.playerCfg.replay.freqs.Clear();
			for (int i = 0; i < taskBox.tskProvider.tskSelector.tskList.Count; i++)
				cfg.playerCfg.replay.freqs.Add((double[])taskBox.tskProvider.tskSelector.tskList[i].freq.freq.Clone());
		}

		public void newGame(IEnumerable<int> tskSets)
		{
			oturn = 0;
			_cfg.playerCfg.init(tskSets);
			_cfg.play = new GameRecords();
			_cfg.cfg.init();
			applyConfig(_cfg);
		}

		public void intLoadStream(Stream stream)
		{
			_cfg = null;
			if (stream.Length > 0)
			{
				try
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(StoredConfig));
					_cfg = serializer.ReadObject(stream) as StoredConfig;
				}
				catch
				{
					_cfg = null;
				}
			}
			if (_cfg == null)
			{
				_cfg = new StoredConfig();
			}
		}

		public void load()
		{
			String fileName = Constants.CFG_FILE_NAME;
			try
			{
				using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
				using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(fileName, System.IO.FileMode.OpenOrCreate, storageFile))
				{
					intLoadStream(storageFileStream);
				}
			}
			catch
			{
				_cfg = new StoredConfig();
			}
		}

		public void cont()
		{
			oturn = 0;
			_cfg.cfg = _cfg.playerCfg.cont;
			_cfg.seed = rand.NextInt();
			applyConfig(_cfg);
		}

		public void replay()
		{
			oturn = 0;
			_cfg.cfg = _cfg.playerCfg.replay;
			_cfg.play = _cfg.playerCfg.record;
			_cfg.playerCfg.record = new GameRecords();
			applyConfig(_cfg);
		}

		public void saveToStream(Stream stream)
		{
			_cfg.playerCfg.cont.tskCount = taskBox.tskProvider.tskCount;
			_cfg.playerCfg.cont.minTypeSpeed = userMinTypeSpeed;
			_cfg.playerCfg.cont.typeSpeed = userTypeSpeed;
			_cfg.playerCfg.cont.lives = Lives;
			_cfg.playerCfg.cont.freqs = new List<double[]>();
			for (int i = 0; i < taskBox.tskProvider.tskSelector.tskList.Count; i++)
				_cfg.playerCfg.cont.freqs.Add(taskBox.tskProvider.tskSelector.tskList[i].freq.freq);
			DataContractSerializer serializer = new DataContractSerializer(typeof(StoredConfig));
			serializer.WriteObject(stream, _cfg);
		}

		public void save()
		{
			String fileName = Constants.CFG_FILE_NAME;
			using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
			using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(fileName, System.IO.FileMode.Create, storageFile))
			{
				saveToStream(storageFileStream);
			}
		}


		/// <summary>
		/// press space to start or pause, Escape to switch in game over mode
		/// </summary>
		/// <param name="key"></param>
		public void KeyPressed(PacmanKey key)
		{
			// Gestion des flèches pour le déplacement du pacman

			if (key == PacmanKey.Pause)
			{
				// touche Pause ou P
				Paused = !Paused;
			}
			else if (key == PacmanKey.Space && ((GameMode.GAME_OVER_DEATH == Status) || (GameMode.GAME_OVER_ESC == Status)))
			{
				SetMode(GameMode.READY);
			}
			else if (key == PacmanKey.Escape)
			{
				SetMode(GameMode.GAME_OVER_ESC);
			}
			else if (key == PacmanKey.NextLevel)
			{
				Labyrinth.getCurrent().RemainingPills = 0;
			}
			else
				if (_Pacman != null)
					_cfg.play.Add(new GameRecord(oturn+ 1, key));
		}
		#endregion
		private bool chkKey(TTsk keyTsk, bool speedOk, int x, int y)
		{
			TTsk labTsk = CurrentLabyrinth.chars[x, y];
			if (labTsk == keyTsk)
			{
				UIElement pill;
				_pillCoord.TryGetValue(new Point(x, y), out pill);
				_Pacman.xyList.add(new TPillPoint(x, y, pill));
				taskBox.tskProvider.tskGood(keyTsk, speedOk);
				return true;
			}
			return false;
		}

		class TXYDir
		{
			public int x, y;
			public bool skip;
			public Direction dir;
			public TXYDir(Direction _dir)
			{
				dir = _dir;
			}
		};
		TXYDir[] xyDirs;

		public void processKey(TTsk keyTsk)
		{
			_cfg.play.Add(new GameRecord(oturn+1, keyTsk));
		}

		bool intProcessKey(TTsk keyTsk)
		{
			bool speedOk = true;
			if (lastKeyTurn > 0)
			{
				Double speed;
				if (lastKeyTurn == turn) speed = lastSpeed+lastMomentSpeed;
				else
				{
					speed = 1000.0 * 60 / SleepTime / (turn - lastKeyTurn);
					lastMomentSpeed = speed;
				}
				lastSpeed = speed;
				if (speed > fUserTypeSpeed)
				{
					userTypeSpeed = Config.FilterSpeedUp * speed + (1 - Config.FilterSpeedUp) * fUserTypeSpeed;
					userMinTypeSpeed = Config.FilterAvgSpeedUp * speed + (1 - Config.FilterAvgSpeedUp) * fUserMinTypeSpeed;
				}
				else
				{
					userTypeSpeed = Config.FilterSpeedDown * speed + (1 - Config.FilterSpeedDown) * fUserTypeSpeed;
					userMinTypeSpeed = Config.FilterAvgSpeedDown * speed + (1 - Config.FilterAvgSpeedDown) * fUserMinTypeSpeed;
				}
				speedOk = (speed > userMinTypeSpeed);
			}
			lastKeyTurn = turn;

			UInt16 x, y;
			if (keyTsk.tskNo < 1) return false;
			if (_Pacman == null) return false;
			TPillPoint xy = _Pacman.xyList.head;

			x = xy.x;
			y = xy.y;

			foreach (TXYDir dir in xyDirs)
			{
				dir.x = x;
				dir.y = y;
				dir.skip = false;
			}
			int dirCnt = xyDirs.Length;
			do
			{
				foreach (TXYDir xyDir in xyDirs)
				{
					if (xyDir.skip) continue;
					Labyrinth.translate(ref xyDir.x, ref xyDir.y, xyDir.dir);
					if (!Labyrinth.outBound(xyDir.x, xyDir.y))
					{
						if (CurrentLabyrinth.laby[xyDir.y][xyDir.x] == 0)
						{
							foreach (Direction dir in Direction.directions)
							{
								if (dir.Opposite == xyDir.dir) continue;
								if (dir == xyDir.dir) continue;

								int xx = xyDir.x;
								int yy = xyDir.y;
								Labyrinth.translate(ref xx, ref yy, dir);
								if (chkKey(keyTsk, speedOk, xx, yy)) return true;
							}
							continue;
						}
						else
						{
							if (chkKey(keyTsk, speedOk, xyDir.x, xyDir.y))
							{
								double halfSpeed = userMinTypeSpeed / 2;
								Audio.PlayGoodPill(Math.Max(Math.Min((userTypeSpeed - halfSpeed) / halfSpeed, 1), 0));
								return true;
							}
						}
					}
					dirCnt--;
					xyDir.skip = true;
				}
			} while (dirCnt > 0);
			taskBox.tskProvider.tskBad(keyTsk);
			if(Audio!=null)
				Audio.PlayWrongPill();
			return false;
		}
	}

	public class TInterval
	{
		long before;
		double msOld;
		public TInterval()
		{
			get();
		}
		public long getMsFilt(double k)
		{
			msOld = msOld * (1 - k) + get().TotalMilliseconds * k;
			return (long)msOld;
		}
		public long getMs()
		{
			return (long)(get().TotalMilliseconds);
		}
		public TimeSpan get()
		{
			long now = DateTime.Now.Ticks;
			TimeSpan elapsedTime = new TimeSpan(now - before);
			before = now;
			return elapsedTime;
		}
	}
}
