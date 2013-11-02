using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GeniusPacman.Core.Sprites
{
	public class Pacman : Sprite
	{
		private int past;
		public TXYList xyList;
		Point nextXY;
		public Pacman()
		{
			xyList = new TXYList();
			Init();
		}

		public void Init()
		{
			X = (13 * Constants.GRID_WIDTH) + Constants.GRID_WIDTH_2;
			Y = 23 * Constants.GRID_HEIGHT;
			xyList.initHead(new TPillPoint(13, 23, null));
			nextXY.X = -1;
			setDirection(Direction.Right);
			past = 0;
			xyList.clear();
		}

		private void setDirection(Direction direction)
		{
			this._DesiredDirection = direction;
			CurrentDirection = direction;
		}

		public override int animate()
		{
			int ret = 0;
			int car, nbBouge = 1;
			bool checkPill = false;

			spriteCount = (spriteCount + 1) % 3;
			// Les changements de direction sont autorisé tout le temps
			if (nbBouge > 0)
			{
				if (_DesiredDirection.isOpposite(CurrentDirection))
				{
					CurrentDirection = _DesiredDirection;
				}
				if (X % Constants.GRID_WIDTH == 0 &&
					 Y % Constants.GRID_HEIGHT == 0 &&
					 X >= 0 &&
					 X < Labyrinth.WIDTH * Constants.GRID_WIDTH)
				{
					// pacman inside the block
					xLaby = X / Constants.GRID_WIDTH;
					yLaby = Y / Constants.GRID_HEIGHT;
					
					System.Diagnostics.Debug.WriteLine(string.Format("pos in laby({0},{1})", xLaby, yLaby));

					if (!xyList.empty)
					{
						if (Labyrinth.getCurrent().get(xLaby, xyList.tail.y) != 5)
						{
							if (yLaby > xyList.tail.y) Up();
							else if (yLaby < xyList.tail.y) Down();
						}
						if (Labyrinth.getCurrent().get(xyList.tail.x, yLaby) != 5)
						{
							if (xLaby < xyList.tail.x) Right();
							else if (xLaby > xyList.tail.x) Left();
						}
						if ((xLaby == xyList.tail.x) && (yLaby == xyList.tail.y))
						{
							xyList.remove();
							nbBouge = 0;
							checkPill = true;
						}
					}
					else
					{
						xyList.head = new TPillPoint(xLaby, yLaby, null);
					}
					if (checkPill)
					{
						car = Labyrinth.getCurrent().get(xLaby, yLaby);
						if ((car & 3) > 0)
						{
							// pacman on a pill
							ret = 1;
							if (++past == 4)
							{
								past = 0;
								nbBouge--;
							}
							// clear pill
							Labyrinth.getCurrent().Eat(xLaby, yLaby);
							if (car == 2)
							{
								ret++;
							}
							if (car == 3) ret = 0; //Second "eat
						}
					}
					if (nbBouge > 0)
					{
						if (!_DesiredDirection.Equals(CurrentDirection) &&
							 (car = getWallAt(_DesiredDirection)) != 5)
						{
							// Can pacman change direction?
							if (((CurrentDirection.isLeft() || CurrentDirection.isRight()) &&
								 (_DesiredDirection.isUp() || _DesiredDirection.isDown())) ||
								 ((CurrentDirection.isUp() || CurrentDirection.isDown()) &&
								 (_DesiredDirection.isLeft() || _DesiredDirection.isRight())))
							{
								nbBouge++;
							}
							CurrentDirection = _DesiredDirection;
						}
						if ((getWallAt(CurrentDirection) != 0)&&(xyList.empty))
//						if ((getWallAt(CurrentDirection) == 5))
						{
							nbBouge = 0;
							// Pacman stops - show first sprite
							spriteCount = 1;
						}
					}
				}
			}
			// notify that pacman is about to move or moves again
			InMove = nbBouge > 0;
			System.Diagnostics.Debug.WriteLine("nbbouge : " + CurrentDirection.ToEnum().ToString());
			int currentX = X;
			int currentY = Y;
			while (--nbBouge >= 0)
			{
				switch (CurrentDirection.ToEnum())
				{
					case DirectionEnum.Up:
						currentY -= Constants.GRID_HEIGHT_4;
						break;
					case DirectionEnum.Right:
						currentX += Constants.GRID_WIDTH_4;
						if (currentX >= (Labyrinth.WIDTH * Constants.GRID_WIDTH + Constants.GRID_WIDTH_X2))
						{
							currentX = -Constants.GRID_WIDTH_X2;
						}
						break;
					case DirectionEnum.Down:
						currentY += Constants.GRID_HEIGHT_4;
						break;
					case DirectionEnum.Left:
						currentX -= Constants.GRID_WIDTH_4;
						if (currentX <= -Constants.GRID_WIDTH_X2)
						{
							currentX = Labyrinth.WIDTH * Constants.GRID_WIDTH + Constants.GRID_WIDTH_X2;
						}
						break;
				}
			}
			System.Diagnostics.Debug.WriteLine(string.Format("pacman (x,y) : ({0},{1})", currentX, currentY));
			X = currentX;
			Y = currentY;
			return ret;
		}

		public void Right()
		{
			_DesiredDirection = Direction.Right;
		}

		public void Up()
		{
			_DesiredDirection = Direction.Up;
		}

		public void Left()
		{
			_DesiredDirection = Direction.Left;
		}

		public void Down()
		{
			_DesiredDirection = Direction.Down;
		}
	}
}
