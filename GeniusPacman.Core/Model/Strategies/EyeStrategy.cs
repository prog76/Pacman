using System;

namespace GeniusPacman.Core.Strategies
{
	public class EyeStrategy : Strategy 
	{
		public EyeStrategy(StrategyEnum strategy) : base(strategy)
		{
		}

		protected override void moveOnGrid() 
		{
			int dx = xLabyGhost - Labyrinth.HOUSE_X;
			int dy = yLabyGhost - Labyrinth.HOUSE_Y;
			if (dx < 0) 
			{
				right();
			} 
			else 
			{
				left();
			}
			if (dy < 0) 
			{
				down();
			} 
			else 
			{
				up();
			}
			right();
			left();
			down();
			up();
		}

		protected override void moveNotOnGrid() 
		{
			// les yeux sont ils en face de l'entrée de la maison ?
            if (xEcranGhost == (Labyrinth.HOUSE_X * Constants.GRID_WIDTH) - Constants.GRID_WIDTH_2) 
			{
                if (yEcranGhost == (Labyrinth.HOUSE_Y - 3) * Constants.GRID_HEIGHT) 
				{
					// oui, on entre
					newDirection = Direction.Down;
				}
                else if (yEcranGhost == Labyrinth.HOUSE_X * Constants.GRID_HEIGHT) 
				{
					ghost.setState(GhostState.HOUSE);
					newDirection = Direction.Left;
				}
			}

		}

	}
}
