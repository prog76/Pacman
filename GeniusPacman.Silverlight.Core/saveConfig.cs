using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using SoftConsept.Collections;
using System;

namespace GeniusPacman.Core
{
	public sealed class Config
	{
		public const int MinSpeed = 20;
		public const double FilterSpeedUp = 0.1;
		public const double FilterAvgSpeedUp = 0.1;
		public const double FilterSpeedDown = 0.35;
		public const double FilterAvgSpeedDown = 0.05;

		public const double SpeedPacman = 1.2;
		public const int SafeRadius = 6;

		public const double GhostSpeedEye = 2;
		public const double GhostSpeedOutSafe = 0.2;
		public const double GhostSpeed = 1.6;
		public const double GhostSpeedFlee = 0.5;
		public const double GhostSpeedHunt = 0.7;
		public const double GhostSpeedHuntPulse = 0.5;
		public const int GhostSpeedSwitchTime = 50;

	}

	public struct IntCfg
	{
		public int tskCount, lives;
		public double typeSpeed, minTypeSpeed;
		public List<double[]> freqs;
		public void init()
		{
			typeSpeed = 20;
			minTypeSpeed = 20;
			tskCount = 8;
			lives = Constants.INIT_LIVES;
			freqs = new List<double[]>();
		}
	}

	[DataContract]
	public class PlayerConfig
	{
		[DataMember]
		public IntCfg replay, cont;
		[DataMember]
		public string name;
		[DataMember]
		public GameRecords record;
		[DataMember]
		public List<int> tskSets;
		[DataMember]
		public int ghostCount;

		public int scoreNo;
		void _init()
		{
			replay.init();
			cont.init();
		}
		void _create()
		{
			tskSets = new List<int>();
			record = new GameRecords();
			ghostCount = 4;
		}
		public void init(IEnumerable<int> _tskSets)
		{
			_init();
			record.Clear();
			tskSets.Clear();
			foreach (int i in _tskSets) tskSets.Add(i);
		}
		public PlayerConfig()
		{
			_init();
			_create();
		}
		public PlayerConfig(string _name)
		{
			name = _name;
			_init();
			_create();
		}
	}

	[DataContract]
	public class StoredConfig: INotifyPropertyChanged
	{

		[DataMember]
		public List<PlayerConfig> players;
		[DataMember]
		public int seed;
		[DataMember]
		public List<TTskSet> tskSets;
		[DataMember]
		public TScores scores;
		[DataMember]
		public bool isSoundMuted;
		[DataMember]
		public bool isMusicMuted;

		[OnDeserialized]
		public void subInit(StreamingContext context)
		{
			subSubInit();
		}

		void subSubInit()
		{
			play = new GameRecords();
			tskSetsSpetial = new List<TTskSet>{
				{new TTskSet("Spetial","||●", "", 0, false, Colors.White, 0, 0)},
				{new TTskSet("Delimiter"," " ,"||||||||\n", 3, false, Colors.Yellow, 1, 1)}
			};
		}

		int playerNo;

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}

		#endregion

		public GameRecords play; //Do not write to file
		public IntCfg cfg;
		public List<TTskSet> tskSetsSpetial;
		public int ghostCount
		{
			get
			{
				return Math.Max(1,playerCfg.ghostCount);
			}
			set
			{
				playerCfg.ghostCount = value;
				NotifyPropertyChanged("ghostCount");
			}
		}
		public PlayerConfig playerCfg
		{
			get { return players[playerNo]; }
			set
			{
				players[playerNo] = value;
				NotifyPropertyChanged("playerName");
			}
		}
		public string playerName
		{
			get
			{
				return playerCfg.name;
			}
			set
			{
				playerCfg.name = value;
				NotifyPropertyChanged("playerName");
			}
		}

		public void playerNext()
		{
			playerNo++;
			if (playerNo == players.Count) playerNo = 0;
			NotifyPropertyChanged("playerName");
			NotifyPropertyChanged("ghostCount");
		}
		public void playerPrev()
		{
			playerNo--;
			if (playerNo == -1) playerNo = players.Count - 1;
			NotifyPropertyChanged("playerName");
			NotifyPropertyChanged("ghostCount");
		}
		public void init()
		{
			scores = new TScores();
			scores.add("Pinky", 100);
			scores.add("Blinky", 150);
			scores.add("Inky", 250);
			scores.add("Clyde", 500);
			playerNo = 0;
			
			players = new List<PlayerConfig>();
			players.Add(new PlayerConfig("PLAYER1"));
			players.Add(new PlayerConfig("PLAYER2"));
			players.Add(new PlayerConfig("PLAYER3"));
			tskSets = new List<TTskSet>(){
				{new TTskSet("Русский", "|||ап|выф|ке|уцй|ми|сч|яё|","|||ор|лд|жэ||гн|шщ|зхъ||ти|ьб|ю.|",2, false, Colors.White, 2, 6)},
				{new TTskSet("English","|||fg|dsa|rt|ew|q`||vb|cx|z|", "|||jh|kl|;'||uy|io|p[]||mn|,./",2, false, Colors.White, 2, 6)},
				{new TTskSet("Numbers", "|||56|43|21|","|||78|90|-=|", 2, false, Colors.White, 1, 3)},
				{new TTskSet("NumPad", "","|||4710||582/||693*.-|+", 2, false, Colors.White, 1, 6)}
        };
			seed = Game.rand.NextInt();
		}

		public StoredConfig()
		{
			subSubInit();
			init();
			NotifyPropertyChanged("playerName");
		}
	}
}