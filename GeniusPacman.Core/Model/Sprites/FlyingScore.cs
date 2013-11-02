using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core.Sprites
{
    public class FlyingScore : Sprite
    {
        private int Fcount;

        public FlyingScore(int xecran, int yecran, int nsprite)
        {
            this.X = xecran;
            this.Y = yecran;
            this.Fcount = yecran;
            this.spriteNum = nsprite;
        }

        public void up()
        {
            Y--;
        }

        public void down()
        {
        }

        public void left()
        {
        }

        public void right()
        {
        }

        public override int animate()
        {
            up();
            return 0;
        }

        public int count()
        {
            Fcount--;
            return Fcount;
        }

        public int NbPoints
        {
            get
            {
                return (spriteNum + 1) * 100;
            }
        }

    }
}
