using System;
using GeniusPacman.Core;

namespace GeniusPacman.Core
{
	public class TTaskBox
	{
		const int BOX_SIZE = 3;
		Labyrinth fLab;
		int x, y;
		public TTskProvider tskProvider;
		public TTaskBox(Game _game)
		{
			tskProvider = new TTskProvider(_game);
		}
		public Labyrinth lab
		{
			set
			{
				fLab = value;
				tskProvider.lab = fLab;
			}
		}

		void getTask(int xx, int yy, bool isX, int dxy)
		{
			bool notFound;
			TTsk cInd;
			int x1, y1, x2, y2, dx, dy;
			if (!fLab.chars[xx, yy].isPill()) return;
			if (isX)
			{
				y1 = yy - 2;
				x1 = xx;
				dy = 1;
				dx = -dxy;
			}
			else
			{
				x1 = xx - 2;
				y1 = yy;
				dx = 1;
				dy = -dxy;
			}
			tskProvider.pointSet(xx, yy);
			do
			{
				cInd = tskProvider.pointGetVal();
				if ((cInd.tskNo == 0) && (cInd.chr == 0))
				{
					break; //not found 
				}
				notFound = false;
				x2 = x1;
				y2 = y1;
				if (fLab.chars[xx + dx, yy + dy] == cInd)
					notFound = true;
				else
					for (int j = 0; j < 3; j++)
					{
						notFound |= ((!fLab.isWallOrOut(xx, y2)) ||(!fLab.isWallOrOut(x2, yy))) && (fLab.chars[x2, y2] == cInd);
						if (notFound) break;
						x2 += dx;
						y2 += dy;
					}
				if (!notFound && (fLab.chars[xx - 2, yy] == cInd))
				{
					y2 = 1;
				}
				if (notFound)
				{
					tskProvider.tskSelector[cInd.tskNo].skip[cInd.chr]=true;
				}
			} while (notFound);
			fLab.chars[xx, yy] = cInd;
		}
		void clearChar(int xx, int yy)
		{
			TTsk tsk = new TTsk();
			tsk.tskNo = 0;
			tsk.chr = 0;
			if (fLab.chars[xx,yy].isPill()) fLab.chars[xx, yy] = tsk;
		}

		void fillBox(int dxy, bool isX)
		{
			bool clear = true;
			int signA = Math.Abs(dxy);
			int sign = Math.Sign(dxy);
			if (signA > BOX_SIZE)
			{
				signA = BOX_SIZE * 2 + 1;
				sign = 1;
				clear = false;
			}
			if (isX)
			{
				tskProvider.pointAddSide(-sign, 0);
				tskProvider.pointAddSide(0, -1);
			}
			else
			{
				tskProvider.pointAddSide(0, -sign);
				tskProvider.pointAddSide(-1, 0);
			}
			for (int i = -BOX_SIZE; i <= BOX_SIZE; i++)
				for (int j = signA; j > 0; j--)
				{
					if (isX)
					{
						if (clear) clearChar(x - sign * (BOX_SIZE + j), y + i);
						getTask(x + sign * (BOX_SIZE + 1 - j), y + i, isX, sign);
					}
					else
					{
						if (clear) clearChar(x + i, y - sign * (BOX_SIZE + j));
						getTask(x + i, y + sign * (BOX_SIZE + 1 - j), isX, sign);
					}
				}
		}

		public void Init()
		{
			x = 0;
			y = 0;
		}
		public void setPoint(int xx, int yy)
		{
			int dx = xx - x;
			int dy = yy - y;
			x = xx;
			if ((dx != 0) && (dx <= BOX_SIZE)) fillBox(dx, true);
			y = yy;
			if (dy != 0) fillBox(dy, false);
		}
	}
}