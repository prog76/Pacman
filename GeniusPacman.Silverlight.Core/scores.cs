using System.Collections.Generic;

namespace GeniusPacman.Core
{

	public class TScore
	{
		public string name;
		public int score;
		public TScore()
		{
		}
		public TScore(string _name, int _score)
		{
			name = _name;
			score = _score;
		}
	}

	public class TScoreComparer : IComparer<TScore>
	{
		public int Compare(TScore x, TScore y)
		{
			int res = y.score.CompareTo(x.score);
			if (res == 0) res = x.name.CompareTo(y.name);
			return res;
		}
	}

	public class TScores : List<TScore>
	{
		public int add(string _name, int _score)
		{
			TScore score = new TScore(_name, _score);
			Add(score);
			Sort(new TScoreComparer());
			while (Count > 10) RemoveAt(9);
			return IndexOf(score);
		}
	}
}