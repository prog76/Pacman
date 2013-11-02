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
using GeniusPacman.Core;

namespace test
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			Game game;
			game = new Game();
			TTskProvider tprov = new TTskProvider(game);
			tprov.tskBad(tprov.encodeTask('f'));
			tprov.tskBad(tprov.encodeTask('g'));
			tprov.tskBad(tprov.encodeTask('g'));
			tprov.tskBad(tprov.encodeTask('g'));
			tprov.tskBad(tprov.encodeTask('h'));

			tprov.tskGood(tprov.encodeTask('h'),true);

			TRandom rand = new TRandom("0123456789", null,0);
			Dictionary<Char, int> dic = new Dictionary<Char, int>();
			rand.count = 6;
			rand.only.Add(2);
			rand.only.Add(1);
			rand.only.Add(5);

			rand.freq[2] = 1;
			rand.freq[4] = 2;
			Char c;
			String res = "";
			string res1 = "";
			for (int i = 0; i < rand.count; i++) dic.Add(rand.charAt(i), 0);
			for (int i = 0; i < 800; i++) { res += (c = rand.getVal()); dic[c]++; }
			for (int i = 0; i < dic.Count; i++)res1 += " " + dic.ElementAt(i).Key + '='+dic.ElementAt(i).Value.ToString() + ' '; 
			InitializeComponent();
		}
	}
}
