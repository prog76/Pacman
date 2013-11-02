using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SoftConsept.Collections;


namespace GeniusPacman.Core
{
	public class SortList<T> : List<T>
		where T : IComparable<T>
	{
		public new void Add(T Item)
		{
			if (Count == 0)
			{
				//No list items
				base.Add(Item);
				return;
			}
			if (Item.CompareTo(this[Count - 1]) > 0)
			{
				//Bigger than Max
				base.Add(Item);
				return;
			}
			int min = 0;
			int max = Count - 1;
			while ((max - min) > 1)
			{
				//Find half point
				int half = min + ((max - min) / 2);
				//Compare if it's bigger or smaller than the current item.
				int comp = Item.CompareTo(this[half]);
				if (comp == 0)
				{
					//Item is equal to half point
					Insert(half, Item);
					return;
				}
				else if (comp < 0) max = half;   //Item is smaller
				else min = half;   //Item is bigger
			}
			if (Item.CompareTo(this[min]) <= 0) Insert(min, Item);
			else Insert(min + 1, Item);
		}
	}

	public class TIntSortList : SortList<int>
	{
		public int Max()
		{
			if (Count == 0) return 0;
			return this[Count - 1];
		}
	}

	public class TDoubleSortList : SortList<double>
	{
		public double Max()
		{
			if (Count == 0) return 0;
			return this[Count - 1];
		}
	}

	public class TTsk
	{
		public ushort Data;
		public TTsk()
		{
			Data = 0xFFF;
		}

		public bool isPill()
		{
			return Data != 0xFFF;
		}

		public byte tskNo
		{
			get
			{
				return (byte)(Data & 0xF);
			}
			set
			{
				Data &= 0xFF0;
				Data |= (ushort)(value);
			}
		}
		public byte chr
		{
			get
			{
				return (byte)(Data >> 4);
			}
			set
			{
				Data &= 0x00F;
				Data |= (ushort)(value << 4);
			}
		}
		public static implicit operator TTsk(ushort tskData)
		{
			TTsk tsk = new TTsk();
			tsk.Data = tskData;
			return tsk;
		}
		public static implicit operator ushort(TTsk tsk)
		{
			return tsk.Data;
		}
		public static bool operator ==(TTsk left, TTsk right)
		{
			if (((Object)left == null) && ((Object)right == null)) return true;
			if (((Object)left == null) || ((Object)right == null)) return false;
			return (left.Data == right.Data);
		}
		public static bool operator !=(TTsk left, TTsk right)
		{
			return !(left == right);
		}
		public override bool Equals(object obj)
		{
			if (!(obj is TTsk))
			{
				return false;
			}
			return (((TTsk)obj) == this);
		}
		public override int GetHashCode()
		{
			return Data;
		}
	}

	public class TTskFreqList : INotifyPropertyChanged
	{
		public class TFreqData : INotifyPropertyChanged
		{
			ushort fTskHash;
			double fFreq;
			TTskProvider fTskProvider;
			public TTskProvider tskProvider
			{
				get { return fTskProvider; }
			}
			public ushort tskHash
			{
				get
				{
					return fTskHash;
				}
			}
			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnPropertyChanged(string PropertyName)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
				{
					handler(this, new PropertyChangedEventArgs(PropertyName));
				}
			}

			public double freq
			{
				get
				{
					return fFreq;
				}
				set
				{
					fFreq = Math.Max(value, Constants.MAX_FREQ);
					OnPropertyChanged("freq");
				}
			}
			public TFreqData(ushort _tskHash, double _freq, TTskProvider _tskProvider)
			{
				fTskHash = _tskHash;
				fFreq = _freq;
				fTskProvider = _tskProvider;
			}

		}

		public class TFreqComparer : IComparer<TFreqData>
		{
			public int Compare(TFreqData x, TFreqData y)
			{
				int r = (y.freq.CompareTo(x.freq));
				if (r == 0)
					r = y.tskHash.CompareTo(x.tskHash);
				return r;
			}
		}

		public class TFreqs : SortedObservableCollection<TFreqData>
		{
			public TFreqs(IComparer<TFreqData> comparer) : base(comparer) { }
		};
		public class THashes : Dictionary<ushort, TFreqData>
		{
		};
		TFreqs fFreqs;
		THashes fHashes;
		public TTskProvider fTskProvider;
		int fFreqsSum;

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string PropertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(PropertyName));
			}
		}

		public TFreqs freqs
		{
			get
			{
				return fFreqs;
			}
		}

		public int freqsSum
		{
			get
			{
				return fFreqsSum;
			}
		}
		public void init()
		{
			fFreqs.Clear();
			fHashes.Clear();
			fFreqsSum = 0;
		}
		public TTskFreqList()
		{
			fFreqs = new TFreqs(new TFreqComparer());
			fHashes = new THashes();
			init();
		}
		public double this[ushort tskHash]
		{
			set
			{
				TFreqData data;
				int oldFreqsSum = fFreqsSum;
				if (fHashes.TryGetValue(tskHash, out data))
				{
					oldFreqsSum -= (int)data.freq;
					fFreqs.Remove(data);
					fHashes.Remove(data.tskHash);
				}
				if (value > 0)
				{
					data = new TFreqData(tskHash, value, fTskProvider);
					fFreqs.Add(data);
					fHashes.Add(data.tskHash, data);
					oldFreqsSum += (int)data.freq;
				}
				if (oldFreqsSum != fFreqsSum)
				{
					fFreqsSum = oldFreqsSum;
					OnPropertyChanged("freqsSum");
				}
			}
		}
	}

	public class TRandom
	{
		public class TFreq
		{
			double[] fFreq;
			public TTskFreqList parentCharFreqList;
			TDoubleSortList freqList;
			int tskNo;
			public TFreq(TTskFreqList _parentCharFreqList, int _tskNo)
			{
				parentCharFreqList = _parentCharFreqList;
				tskNo = _tskNo;
				freqList = new TDoubleSortList();
			}
			public double max
			{
				get
				{
					return freqList.Max();
				}
			}
			public int count
			{
				get
				{
					return fFreq.Length;
				}
				set
				{
					fFreq = new double[value];
				}
			}

			public Double[] freq
			{
				get { return fFreq; }
				set{
					for (int i = 0; i < fFreq.Length; i++)
						this[i] = value[i];
				}
			}

			public double this[int index]
			{
				get
				{
					return fFreq[index];
				}
				set
				{
					if (value < 0) value = 0;
					freqList.Remove(fFreq[index]);
					fFreq[index] = value;
					freqList.Add(value);
					if (parentCharFreqList != null)
					{
						TTsk tsk = new TTsk();
						tsk.tskNo = (byte)tskNo;
						tsk.chr = (byte)index;
						parentCharFreqList[tsk.Data] = fFreq[index];
					}
				}
			}
		}
		public class TSkip
		{
			int[] fSkip;
			int flag;
			int skipCnt, skipCnt2;
			public TSkip()
			{
				init();
			}

			public int count
			{
				get
				{
					return fSkip.Length;
				}
				set
				{
					fSkip = new int[value];
				}
			}
			public bool isAllSkipped()
			{
				return (skipCnt + skipCnt2) >= count;
			}
			public void init()
			{
				flag = 1;
				skipCnt = 0;
				skipCnt2 = 0;
			}
			public void clear()
			{
				flag++;
				skipCnt2 = 0;
			}
			public bool this[int index]{
				get{
					return (fSkip[index]==-1)||(fSkip[index]==flag);
				}
				set{
					if (value)
					{
						if (fSkip[index] != flag)
						{
							fSkip[index] = flag;
							skipCnt2++;
						}
					}
					else
					{
						if (fSkip[index] == -1)
						{
							skipCnt--;
						}
						else if(fSkip[index]==flag) skipCnt2--;
						fSkip[index] = 0;
					}
				}
			}
			public void skipAlways(int index){
				fSkip[index]=-1;
				skipCnt++;
			}
		}
		String fChars;
		TFreq fFreqs;
		double lastErr;
		int lastX;
		public TSkip skip;
		public HashSet<int> only;

		public TFreq freq
		{
			get
			{
				return fFreqs;
			}
		}
		public TRandom(TTskFreqList _parentCharFreqList, int _tskNo)
		{
			fFreqs = new TFreq(_parentCharFreqList, _tskNo);
			only = new HashSet<int>();
			fChars = "";
			skip = new TSkip();
		}

		public TRandom(string value)
			: this(value, null, 0)
		{
		}
		public TRandom(string value, TTskFreqList _parentCharFreqList, int _tskNo)
			: this(_parentCharFreqList, _tskNo)
		{
			chars = value;
		}
		int fc;
		public int count{
			get{return fc;}
			set{fc=value;}
		}

		protected virtual void setMaxCount(int cnt)
		{
			fFreqs.count = cnt;
			skip.count = cnt;
		}

		public int charsCount
		{
			get
			{
				return chars.Length;
			}
		}

		public int charIndex(char chr)
		{
			return chars.IndexOf(chr);
		}

		public char charAt(int ind)
		{
			return chars[ind];
		}

		protected String chars
		{
			get
			{
				return fChars;
			}
			set
			{
				fChars = value;
				count = fChars.Length;
				setMaxCount(count);
			}
		}
		private void addVal(int x)
		{
			if (skip[x]) return;
			double curErr = freq[x];
			if (curErr < lastErr) return;
			if (curErr > lastErr) lastErr = curErr;
			lastX = x;
		}
		public Char getVal()
		{
			return fChars[getValNo()];
		}
		public int getValNo()
		{
			do
			{
				lastErr = 0;
				Byte[] rands = new Byte[(int)fFreqs.max + 1];
				Game.rand.NextBytes(rands);
				for (int j = 0; j < rands.Length; j++)
				{
					if (only.Count > 0) addVal(only.ElementAt(rands[j] % only.Count));
					else addVal(rands[j] % count);
				}
			} while (skip[lastX]);
			return lastX;
		}
	}

	public class TTskRandom : TRandom
	{
		public bool isAlone;
		public int minLen, maxLen;
		public Color col;	//character color
		public Byte[] fingerNums;
		class TCharFinger{
			public char Key;
			public byte Value;
			public TCharFinger(char _ch,byte _finger){
				Key=_ch;
				Value=_finger;
			}
		};
		class TCharFingers: Queue<TCharFinger>{};
		void scanStr(string str,byte firstFinger,out TCharFingers res){
			bool sameFinger = false;
			byte finger = 0;
			res = new TCharFingers();
			bool oldWasDelim;
			foreach (char ch in str)
			{
				oldWasDelim = false;
				if (finger > 4) finger = 1;
				if (ch == '|')
				{
					oldWasDelim = true;
					sameFinger = !sameFinger;
					if((!sameFinger)&&(oldWasDelim)) finger++; //|||||A|
				}else{
					res.Enqueue(new TCharFinger(ch, (byte)(finger + firstFinger)));
					if(!sameFinger)finger++;
				}
			}
		}
		public TTskRandom(TTskFreqList _parentCharFreqList, int _tskNo, string right, string left, bool _isAlone, Color _col, int _minLen, int _maxLen):base(_parentCharFreqList, _tskNo) //maxLen==0 - spetial chars
		{
			TCharFingers leftData,rightData,tempData;
			scanStr(left, 0,out leftData);
			scanStr(right, 5,out rightData);
			fingerNums=new Byte[leftData.Count+rightData.Count];
			if(leftData.Count>rightData.Count){
				tempData=rightData;
				rightData=leftData;
				leftData=tempData;
			}
			string allChars="";
			TCharFingers pairs = new TCharFingers();
			foreach(TCharFinger pair in rightData){
				pairs.Clear();
				pairs.Enqueue(pair);
				if(leftData.Count>0)
					pairs.Enqueue(leftData.Dequeue());
				foreach(TCharFinger intPair in pairs){
					fingerNums[allChars.Length] = intPair.Value;
					allChars += intPair.Key;
				}
			}
			chars = allChars;
			col = _col;
			isAlone = _isAlone;
			minLen = _minLen;
			maxLen = _maxLen;
			count=0;
		}

		protected override void setMaxCount(int cnt)
		{
			base.setMaxCount(cnt);
		}

		public void freqBad(int tsk)
		{
			freq[tsk] += 2;
		}
	}

	public class TTskSelector : TRandom
	{
		public List<TTskRandom> tskList;
		int fTskCount;
		TTskFreqList parentCharFreqList;
		public TTskSelector(TTskFreqList _parentCharFreqList, int _tskNo)
			: base(null, _tskNo)
		{
			parentCharFreqList = _parentCharFreqList;
			tskList = new List<TTskRandom>();
			fTskCount = 0;
		}
		public TTskRandom this[int index]
		{
			get
			{
				return tskList[index];
			}
		}
		public void tskClear()
		{
			while (tskList.Count > 2) tskList.RemoveAt(2);
			chars = "01";
			fTskCount = tskList[0].count + tskList[1].count;
		}
		public void tskAdd(TTskSet tsk)
		{
			TTskRandom tskRandom = new TTskRandom(parentCharFreqList, tskList.Count, tsk.keysRight, tsk.keysLeft, tsk.isAlone, tsk.color, tsk.minLen, tsk.maxLen);
			int ind = charsCount;
			chars += ind.ToString();
			freq[ind] = tsk.freq;
			tskList.Add(tskRandom);
		}
		public int tskCount()
		{
			return fTskCount;
		}
		public void tskCountSet(int maxTskCnt)
		{
			int first = 0, last = tskList.Count, sign = 1;
			if (fTskCount > maxTskCnt)
			{
				int temp = last;
				last = first;
				first = temp;
				sign = -sign;
			}
			int delta = maxTskCnt - fTskCount;
			for (int i = first+sign; i != last; i += sign)
			{
				int subTskCntOld = tskList[i].count;
				int subDelta=0;
				if (delta > 0)
				{
					subDelta = Math.Min(delta, tskList[i].charsCount - subTskCntOld);
					if (subDelta > 0)
					{
						int subTskCntNew = subTskCntOld + subDelta;
						for (int j = subTskCntOld; j < subTskCntNew; j++)
							tskList[i].freq[j]+=4;
						tskList[i].count = subTskCntNew;
					}
				}
				else if (delta < 0)
				{
					subDelta = Math.Max(delta, -subTskCntOld);
					tskList[i].count += subDelta;
				}
				fTskCount += subDelta;
				delta -= subDelta;

				if(delta ==0){
					if (tskList[i].count > 0) count = i + 1;
					else count = i - 1;
					return;
				}

				if (subDelta > 0)
				{
					count = i + 1;
				}
			}
		}
	}

	public class TTskProvider
	{
		public TTskSelector tskSelector;
		List<XY> pointDirList;
		bool pointNew;
		public Labyrinth lab;
		Game game;
		public TTskProvider(Game _game)
		{
			game = _game;
			if (_game != null)
			{
				game.freqList.fTskProvider = this;
				tskSelector = new TTskSelector(game.freqList, 0);
			}
			else tskSelector = new TTskSelector(null, 0);
			pointDirList = new List<XY>();
			pointNew = false;
		}

		public void tskGood(TTsk tsk, bool speedOk)
		{
			if (speedOk) tskSelector[tsk.tskNo].freq[tsk.chr]--;
			else tskSelector[tsk.tskNo].freq[tsk.chr] += 0.25;
		}
		public void tskBad(TTsk tsk)
		{
			tskSelector[tsk.tskNo].freq[tsk.chr] += 2;
		}

		public void incTskCount()
		{
			TTskRandom tsk=tskSelector.tskList[tskSelector.count-1];
			int dTsk=Math.Min(2,tsk.charsCount-tsk.count);
			if (dTsk==0)dTsk=2;
			tskCount += dTsk;
		}

		public int tskCount
		{
			get
			{
				return tskSelector.tskCount();
			}
			set
			{
				tskSelector.tskCountSet(value);
				if(game!=null)game.userCharCount = tskCount;
			}
		}

		public void pointAddSide(int dx, int dy)
		{
			if (pointNew) pointDirList.Clear();
			pointNew = false;
			pointDirList.Add(new XY(dx, dy));
		}
		public void pointSet(int x, int y)
		{
			tskSelector.only.Clear();
			tskSelector.skip.clear();
			for (int i = 1; i < tskSelector.count; i++)
				tskSelector[i].skip.clear();

			foreach (XY xy in pointDirList)
			{
				int x1 = x, y1 = y, len = 0, newTsk = 0, tskNo = 0;
				for (; ; )
				{
					x1 += xy.x;
					y1 += xy.y;
					if (lab.chars[x1,y1].isPill())
					{
						TTsk ch = lab.chars[x1, y1];
						newTsk = ch.tskNo;
						if (newTsk == 0) break;
						if (len != 0)
						{
							if (tskNo != newTsk) break;
						}
						else tskNo = newTsk;
						len++;
					}
					else break;
				}
				if (tskNo > 0)
				{
					TTskRandom task = tskSelector[tskNo];
					if ((task.isAlone) || (len > task.maxLen))
					{
						tskSelector.skip[tskNo] = true;
						tskSelector.only.Remove(tskNo);
					}
					else
					{
						if ((len < task.minLen) && (task.count > 4) && (!tskSelector.skip[tskNo]))
						{
							tskSelector.only.Add(tskNo);
						}
					}
				}
			}
#if DEBUG		
			for(int i=1;i<tskSelector.count;i++)
				if(!tskSelector.skip[i] && tskSelector[i].isAlone)
					if (check(x - 1, y, i) || check(x - 1, y, i) || check(x - 1, y, i) || check(x - 1, y, i))
			{
				throw new Exception("pointSet");
			}
#endif
		}

#if DEBUG
		bool check(int xx, int yy, int tsk)
		{
			return ((lab.chars[xx, yy] >> 8) == tsk);
		}
#endif

		public TTsk pointGetVal()
		{
			TTsk tsk = new TTsk();
			pointNew = true;
			for (; ; )
			{
				if (tskSelector.skip.isAllSkipped())
				{
					tsk.tskNo = 0; //not found
					tsk.chr = 0;
					return tsk;
				}
				tsk.tskNo = (byte)tskSelector.getValNo();
				if (tskSelector[tsk.tskNo].skip.isAllSkipped())
					tskSelector.skip[tsk.tskNo] = true;
				else
				{
					tsk.chr=(byte)tskSelector[tsk.tskNo].getValNo();
					return tsk;
				}
			}
		}
		public TTsk encodeTask(char key)
		{
			TTsk res = new TTsk();
			int keyInd;
			for (ushort i = 1; i < tskSelector.count; i++)
			{
				if ((keyInd = tskSelector[i].charIndex(key)) >= 0)
				{
					if (keyInd >= tskSelector[i].count)
					{
						break; //New that is wrong for now
					}
					res.tskNo = (byte)i;
					res.chr = (byte)keyInd;
					break;
				}
			}
			return res;
		}
	}

}

