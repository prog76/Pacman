using System;

namespace GeniusPacman.Core.Strategies
{
	public class HuntStrategy : Strategy 
	{
        public HuntStrategy(StrategyEnum strategy)
            : base(strategy)
		{
		}
		
		protected override void moveOnGrid() 
		{
			// si le fantome chasse il va de preference en X ou en Y
			// suivant son numero
			if(ghost.getColor() > 1) 
			{ // les fantomes 2 et 3 seront prioritaires en Y
				moveY();
				moveX();
			} 
			else 
			{ // les fantomes 0 et 1 seront prioritaires en X
				moveX();
				moveY();
			}
		}
		protected override void moveNotOnGrid() 
		{
			if(Game.GetRandom(1000) < 5) 
			{
				// Aléatoirement, les fantomes font demi-tour
				newDirection = ghost.CurrentDirection.Opposite;
			}
		}

	}
}
