using System;

namespace GeniusPacman.Core.Strategies
{
	public class RandomStrategy : Strategy 
	{
        public RandomStrategy(StrategyEnum strategy)
            : base(strategy)
		{
		}

		protected override void moveOnGrid() 
		{
			while(!moved) 
			{
				switch(Game.GetRandom(4)) 
				{
					case 0:
						up();
						break;
					case 1:
						right();
						break;
					case 2:
						down();
						break;
					case 3:
						left();
						break;
				}
			}
		}
		protected override void moveNotOnGrid() 
		{
		}

	}
}
