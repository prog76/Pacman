using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core.Sprites
{
    public class Bonus : Sprite
    {
        private int time;

        public Bonus(int level)
        {
            X = (13 * Constants.GRID_HEIGHT) + 8;
            Y = 17 * Constants.GRID_HEIGHT;
            spriteNum = level % 5;
            time = Math.Max(40, 500 - level * 10);
        }

        public override int animate()
        {
            time--;
            return time;
        }
    }
}
