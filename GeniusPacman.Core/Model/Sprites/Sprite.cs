using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GeniusPacman.Core.Sprites
{
    public abstract class Sprite : Notifiable
    {
        public int xLaby;
        public int yLaby;
        private int _X;
        private int _Y;
        private bool _InMove;
        protected int spriteNum;
        protected int spriteCount;
        protected Direction _DesiredDirection;
        private Direction _CurrentDirection;

        public Direction CurrentDirection
        {
            get
            {
                return _CurrentDirection;
            }
            protected set
            {
                if (value != _CurrentDirection)
                {
                    _CurrentDirection = value;
                    DoPropertyChanged("CurrentDirection");
                }
            }
        }

        public int X
        {
            get
            {
                return _X;
            }
            protected set
            {
                if (value != _X)
                {
                    _X = value;
                    DoPropertyChanged("X");
                }
            }
        }

        public int Y
        {
            get
            {
                return _Y;
            }
            protected set
            {
                if (_Y != value)
                {
                    _Y = value;
                    DoPropertyChanged("Y");
                }
            }
        }

        public bool InMove
        {
            get
            {
                return _InMove;
            }
            protected set
            {
                if (value != _InMove)
                {
                    _InMove = value;
                    DoPropertyChanged("InMove");
                }
            }
        }

		 ~Sprite()
        {
            Debug.WriteLine("~Sprite(): " + this.GetType().ToString());
        }

        protected UInt16 getWallAt(Direction direction)
        {
            return Labyrinth.getCurrent().getWallAt(xLaby, yLaby, direction);
        }

        // teste de collision entre 2 sprites
        public bool HasCollision(Sprite other)
        {
            return (Math.Abs(_X - other._X) <= Constants.COLISION_DIST && Math.Abs(_Y - other._Y) <= Constants.COLISION_DIST);
        }

        public abstract int animate();

        public virtual void Dead()
        {
            InMove = false;
        }
    }
}
