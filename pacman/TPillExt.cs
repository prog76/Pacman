using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using GeniusPacman.Core;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace pacman
{

	public abstract class TPillTsk : TPill
	{
		protected enum PillMode
		{
			Big, BigFirst, New, First, Old, Hide, None
		};
		TTsk fTsk;
		protected Color fTskColor;
		protected GamePresenter fGp;
		protected TTskProvider fTskProvider;
		protected PillMode fMode;

		protected abstract TTskProvider getProvider();
		public GamePresenter gp
		{
			get
			{
				return fGp;
			}
			set
			{
				fGp = value;
			}
		}
		private static void OnGamePresenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TPillTsk)d).gp = (GamePresenter)e.NewValue;
		}

		public static readonly DependencyProperty gamePresenterProperty =
				DependencyProperty.Register("gp", typeof(Game), typeof(TPillTsk), new PropertyMetadata(OnGamePresenterChanged));

		public TPillTsk()
		{
			InitializeComponent();
			if (System.ComponentModel.DesignerProperties.IsInDesignTool)
			{
				fTskProvider = new TTskProvider(null);
			}
		}

		public ushort utsk
		{
			get { return fTsk; }
			set { tsk = (TTsk)value; }
		}

		public TTsk tsk
		{
			get
			{
				return fTsk;
			}
			set
			{
				if (fTsk != value)
				{
					fMode = PillMode.None;
					fTsk = value;
					TTskRandom task = getProvider().tskSelector[fTsk.tskNo];
					char key = task.charAt(fTsk.chr);
					if (key == ' ') key = '␣';
					else if (key == '\n') key = '↲';
					fTskColor = Constants.fingerColors[task.fingerNums[fTsk.chr]];
					text = key.ToString();
					updatePillMode();
				}
			}
		}

		protected virtual void updatePillMode()
		{
			foreColor = fTskColor;
		}

		private static void OnTskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TPillTsk)d).tsk = (TTsk)e.NewValue;
		}

		public static readonly DependencyProperty valProperty =
				DependencyProperty.Register("tsk", typeof(TTsk), typeof(TPillTsk), new PropertyMetadata(OnTskChanged));
	}

	enum TBitmapHolderState
	{
		Ready, Updating, Clean
	}

	class TBitmapHolder
	{
		public TBitmapHolderState state;
		public WriteableBitmap bitmap;
		public EventHandler onReady;
		public TBitmapHolder()
		{
			state = TBitmapHolderState.Clean;
			bitmap = new WriteableBitmap(0,0);
		}
	}

	public class TPillGame : TPillTsk
	{
		const Double FONT_BIG = 1;
		const Double FONT_SMALL = 0.7;
		int fEatCnt;
		int fPressCnt;
		bool fIsBig;
		public static int RemainingPills = 0;
		public static Color COLOR_BIG = Colors.Blue;
		static Dictionary<char, Dictionary<PillMode, TBitmapHolder>> cachedTb = new Dictionary<char, Dictionary<PillMode, TBitmapHolder>>();
		protected override TTskProvider getProvider()
		{
			return fTskProvider;
		}

		void TPill_MouseDown(object sender, MouseEventArgs e)
		{
			btnPressed = true;
			fGp.processKey(tsk);
		}
		void TPill_MouseUp(object sender, MouseEventArgs e)
		{
			btnPressed = false;
		}
		void TPill_MouseEnter(object sender, MouseEventArgs e)
		{
			if (btnPressed)
			{
				fGp.processKey(tsk);
			}
		}

		void TbChanged(object sender, EventArgs e)
		{
			if (tb.Visibility == Visibility.Collapsed) return;
			OnHolderReady(sender, e);
		}

		void OnHolderReady(object sender, EventArgs e)
		{
			pillMode(fMode, true);
		}

		static int c1 = 0, c2 = 0;

		void drawOutline(PillMode mode)
		{
			c1++;
			tb.Visibility = Visibility.Visible;
			tb.Opacity = 1;
			if (mode == PillMode.Big || mode == PillMode.BigFirst)
			{
				tb.border.Opacity = 1;
				tb.kH = FONT_BIG;
				tb.kW = FONT_BIG;
				if (mode == PillMode.BigFirst)
					foreColor = Colors.White;
				else
					foreColor = COLOR_BIG;
				borderColor = Colors.White;
			}
			else
			{
				tb.kH = FONT_SMALL;
				tb.kW = FONT_SMALL;
				tb.border.Opacity = 0.8;
				if (mode == PillMode.Old)
				{
					foreColor = Colors.LightGray;
				}
				else
				{
					if (mode == PillMode.First) foreColor = Colors.White;
					else foreColor = fTskColor;
				}
				borderColor = foreColor;
			}
		}

		void pillMode(PillMode mode, bool LayoutUpdated = false)
		{
			if ((!LayoutUpdated)&&(mode == fMode)) return;
			fMode = mode;
			if (mode == PillMode.Hide)
			{
				Opacity = 0;
				return;
			}
			Opacity = 1;
			TBitmapHolder holder;
			char key=tb.text[0];
			Dictionary<PillMode, TBitmapHolder> cachedTm;
			if (!cachedTb.TryGetValue(key, out cachedTm))
			{
				cachedTm = new Dictionary<PillMode, TBitmapHolder>();
				cachedTb.Add(key, cachedTm);
			}
			if (!cachedTm.TryGetValue(mode, out holder))
			{
				holder = new TBitmapHolder();
				cachedTm.Add(mode, holder);
			}

			if(tb.Visibility==Visibility.Collapsed){
				if (holder.state == TBitmapHolderState.Updating)
				{
					holder.onReady += new EventHandler(OnHolderReady);
				}else
				if((holder.state==TBitmapHolderState.Clean)||(holder.bitmap.PixelWidth < ActualWidth)){
					holder.state = TBitmapHolderState.Updating;	
					drawOutline(mode);
				}else{
					c2++;
					img.Source = holder.bitmap;
				}
			}else{
				if(LayoutUpdated){
					holder.bitmap = new WriteableBitmap((int)ActualWidth, (int)ActualHeight);
					holder.bitmap.Render(tb, null); 
					holder.bitmap.Invalidate();
					holder.state = TBitmapHolderState.Ready;
					if(holder.onReady!=null)
						holder.onReady.Invoke(null, null);
					holder.onReady = null;
					tb.Visibility = Visibility.Collapsed;
					img.Source = holder.bitmap;
				}
			}
#if DEBUG
			System.Diagnostics.Debugger.Log(0, "", c1.ToString() + "/" + c2.ToString() + "\n");
#endif		
		}

		void subInit()
		{
			fEatCnt = Constants.MAX_EAT_CNT;
			fMode = PillMode.None;
			fIsBig = false;
		}

		public void init()
		{
			subInit();
			ellipse = 0;
			updatePillMode();
		}

		public TPillGame(GamePresenter _gp)
		{
			subInit();
			tb.Visibility = Visibility.Collapsed;
			tb.LayoutUpdated += new EventHandler(TbChanged);
			RemainingPills++;
			fGp = _gp;
			fTskProvider = fGp.CurrentGame.taskBox.tskProvider;
			MouseEnter += new MouseEventHandler(TPill_MouseEnter);
			MouseLeftButtonDown += new MouseButtonEventHandler(TPill_MouseDown);
			MouseLeftButtonUp += new MouseButtonEventHandler(TPill_MouseUp);
		}

		public int eatCnt
		{
			get
			{
				return fEatCnt;
			}
			set
			{
				if (value == fEatCnt) return;
				int oldValue = fEatCnt;
				fEatCnt = value;
				if (oldValue == Constants.MAX_EAT_CNT)
				{
					RemainingPills--;
					fIsBig = false;
				}
				updatePillMode();
			}
		}

		protected override void updatePillMode()
		{
			if (fIsBig && isFirst)
				pillMode(PillMode.BigFirst);
			else if (fIsBig)
				pillMode(PillMode.Big);
			else if (isFirst)
				pillMode(PillMode.First);
			else
			{
				if (fEatCnt == Constants.MAX_EAT_CNT)
				{
					pillMode(PillMode.New);
				}
				else
					if (fEatCnt == 0)
					{
						pillMode(PillMode.Hide);
					}
					else
					{
						pillMode(PillMode.Old);
					}
			}
		}

		public int pressCnt
		{
			get
			{
				return fPressCnt;
			}
			set
			{
				if (value > fPressCnt)
				{
					ellipse = 1;
				}
				if (value < 1)
				{
					ellipse = 0;
				}
				fPressCnt = Math.Max(value, 0);
				updatePillMode();
			}
		}

		public bool isBig
		{
			get
			{
				return fIsBig;
			}
			set
			{
				fIsBig = value;
				updatePillMode();
			}
		}
		public void clrFull()
		{
			isFirst = false;
			pressCnt--;
		}
		public bool isFirst
		{
			get
			{
				return ellipse == 1;
			}
			set
			{
				if (fPressCnt > 0)
				{
					if (value) ellipse = 1;
					else ellipse = 2;
				}
				else ellipse = 0;
				updatePillMode();
			}
		}
	}

	public class TPillErr : TPillTsk
	{
		int fFreq;

		private static void OnFreqChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TPillErr)d).freq = (int)e.NewValue;
		}

		public static readonly DependencyProperty freqProperty =
				DependencyProperty.Register("freq", typeof(int), typeof(TPillErr), new PropertyMetadata(OnFreqChanged));

		private static void OnTskHashChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TPillErr)d).tskHash = (ushort)e.NewValue;
		}

		public static readonly DependencyProperty tskHashProperty =
				DependencyProperty.Register("tskHash", typeof(ushort), typeof(TPillErr), new PropertyMetadata(OnTskHashChanged));

		protected override TTskProvider getProvider()
		{
			if (fTskProvider == null)
				fTskProvider = ((TTskFreqList.TFreqData)this.DataContext).tskProvider;
			return fTskProvider;
		}

		public TPillErr()
		{
			ellipse = 0;
		}

		public ushort tskHash
		{
			get
			{
				return tsk.Data;
			}
			set
			{
				tsk = value;
			}
		}

		public int freq
		{
			get
			{
				return fFreq;
			}
			set
			{
				if (fFreq != value)
				{
					fFreq = value;
					Width = 20 + fFreq;
					Height = Width;
				}
			}
		}
	}
}