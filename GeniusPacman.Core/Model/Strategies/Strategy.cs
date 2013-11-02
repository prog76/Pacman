using System;
using GeniusPacman.Core.Sprites;

namespace GeniusPacman.Core.Strategies
{
	public enum StrategyEnum { EYE, FLEE, HUNT, RANDOM, HOUSE };
	public abstract class Strategy
	{
		public static Strategy EYE = new EyeStrategy(StrategyEnum.EYE);
		public static Strategy FLEE = new FleeStrategy(StrategyEnum.FLEE);
		public static Strategy HUNT = new HuntStrategy(StrategyEnum.HUNT);
		public static Strategy RANDOM = new RandomStrategy(StrategyEnum.RANDOM);
		public static Strategy HOUSE = new HouseStrategy(StrategyEnum.HOUSE);
		private StrategyEnum _Strategy;
		protected Ghost ghost;
		protected int xEcranGhost;
		protected int yEcranGhost;
		protected int xLabyGhost;
		protected int yLabyGhost;
		protected int xLabyDest;
		protected int yLabyDest;
		protected Direction newDirection;
		protected bool moved;

		protected Strategy(StrategyEnum strategy)
		{
			this._Strategy = strategy;
		}

		public bool OnGrid(int xEcranGhost, int yEcranGhost)
		{
			return (xEcranGhost % Constants.GRID_HEIGHT == 0) && (yEcranGhost % Constants.GRID_HEIGHT == 0) &&	(xEcranGhost > 0) &&	(xEcranGhost < Labyrinth.WIDTH * Constants.GRID_HEIGHT);
		}

		public Direction move(Ghost ghost, int xEcranGhost, int yEcranGhost, int xPacEcran, int yPacEcran)
		{
			this.ghost = ghost;
			this.xEcranGhost = xEcranGhost;
			this.yEcranGhost = yEcranGhost;
			xLabyGhost = xEcranGhost / Constants.GRID_WIDTH;
			yLabyGhost = yEcranGhost / Constants.GRID_HEIGHT;
			xLabyDest = xPacEcran / Constants.GRID_WIDTH;
			yLabyDest = yPacEcran / Constants.GRID_HEIGHT;
			newDirection = ghost.CurrentDirection;

			moved = false;
			if (OnGrid(xEcranGhost,yEcranGhost))
			{
				moveOnGrid();
				movePriority();
			}
			else
			{
				moveNotOnGrid();
			}
			return newDirection;
		}

		// appelé quand le fantome est sur une case de la grille
		protected abstract void moveOnGrid();
		// appelé quand le fantome n'est pas une case de la grille
		protected abstract void moveNotOnGrid();

		private void movePriority()
		{
			int i = 0;
			while (!moved && i < 4)
			{
				int dir = ((i + ghost.getColor()) % 4);
				switch (dir)
				{
					case 0:
						up();
						break;
					case 2:
						down();
						break;
					case 1:
						right();
						break;
					case 3:
						left();
						break;
				}
				i++;
			}
		}


		protected void moveX()
		{
			if (!moved)
			{
				if (xLabyGhost < xLabyDest)
				{
					right();
				}
				else
				{
					left();
				}
			}
		}

		protected void moveY()
		{
			if (!moved)
			{
				if (yLabyGhost < yLabyDest)
				{
					down();
				}
				else
				{
					up();
				}
			}
		}

		protected void right()
		{
			int bInf;
			int car;
			// on essai d'aller a DROITE
			if (moved || (ghost.getOldDirection().isLeft() && !ghost.State.isHouse()))
			{
				return;
			}
			if (xLabyGhost >= 27)
			{
				car = 0;
			}
			else
			{
				car = Labyrinth.getCurrent().get(xLabyGhost + 1, yLabyGhost);
			}
			bInf = 4 + ((ghost.State.isEye() || ghost.State.isHouse()) ? 1 : 0);
			if (car < bInf)
			{
				newDirection = Direction.Right;
				moved = true;
			}
		}

		protected void up()
		{
			int bInf;
			int car;
			// on essai d'aller en dHaut
			if (moved || (ghost.getOldDirection().isDown() && !ghost.State.isHouse()))
			{
				return;
			}
			car = Labyrinth.getCurrent().get(xLabyGhost, yLabyGhost - 1);
			bInf = 4 + ((ghost.State.isEye() || ghost.State.isHouse()) ? 1 : 0);
			if (car < bInf)
			{
				newDirection = Direction.Up;
				moved = true;
			}
		}

		protected void left()
		{
			int bInf;
			int car;
			// on essai d'aller a dGauche
			if (moved || (ghost.getOldDirection().isRight() && !ghost.State.isHouse()))
			{
				return;
			}
			if (xLabyGhost < 0)
			{
				car = 0;
			}
			else
			{
				car = Labyrinth.getCurrent().get(xLabyGhost - 1, yLabyGhost);
			}
			bInf = 4 + ((ghost.State.isEye() || ghost.State.isHouse()) ? 1 : 0);
			if (car < bInf)
			{
				newDirection = Direction.Left;
				moved = true;
			}
		}

		protected void down()
		{
			int bInf;
			int car;
			// on essai d'aller en dBas
			if (moved || (ghost.getOldDirection().isUp() && !ghost.State.isHouse()))
			{
				return;
			}
			car = Labyrinth.getCurrent().get(xLabyGhost, yLabyGhost + 1);
			bInf = 4 + ((ghost.State.isEye() || ghost.State.isHouse()) ? 1 : 0);
			if (car < bInf)
			{
				newDirection = Direction.Down;
				moved = true;
			}
		}

		public bool isEye()
		{
			return _Strategy == StrategyEnum.EYE;
		}
		public bool isFlee()
		{
			return _Strategy == StrategyEnum.FLEE;
		}
		public bool isHunt()
		{
			return _Strategy == StrategyEnum.HUNT;
		}
		public bool isRandom()
		{
			return _Strategy == StrategyEnum.RANDOM;
		}
		public bool isHouse()
		{
			return _Strategy == StrategyEnum.HOUSE;
		}

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is Strategy) && ((Strategy)obj)._Strategy == _Strategy;
		}

		public override int GetHashCode()
		{
			return _Strategy.GetHashCode();
		}
	}
}
