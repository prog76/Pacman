using System;

namespace GeniusPacman.Core.Strategies
{
	public class FleeStrategy : Strategy 
	{
        public FleeStrategy(StrategyEnum strategy)
            : base(strategy)
		{
		}

		protected override void moveOnGrid() 
		{
			int dx = xLabyDest - xLabyGhost;
			int dy = yLabyDest - yLabyGhost;
			if (Math.Abs(dx) < Math.Abs(dy)) 
			{
				if (dx > 0) 
				{
					left();
				} 
				else 
				{
					right();
				}
				if (dy > 0) 
				{
					up();
				} 
				else 
				{
					down();
				}
			} 
			else 
			{
				if (dy > 0) 
				{
					up();
				} 
				else 
				{
					down();
				}
				if (dx > 0) 
				{
					left();
				} 
				else 
				{
					right();
				}
			}
		}
		protected override void moveNotOnGrid() 
		{
		}
	}
}
