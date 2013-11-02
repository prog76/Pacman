using System;

namespace GeniusPacman.Core.Strategies
{
	public class HouseStrategy : Strategy 
	{
        public HouseStrategy(StrategyEnum strategy)
            : base(strategy)
		{
		}

		protected override void moveOnGrid() 
		{
			ghost.setOldDirection(Direction.Down);
			if(ghost.CurrentDirection.isLeft()) 
			{
				left();
				right();
			} 
			else 
			{
				right();
				left();
			}
		}
		protected override void moveNotOnGrid() 
		{
		}

	}
}
