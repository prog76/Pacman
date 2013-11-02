using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core
{
    public enum DirectionEnum {Up, Right, Down, Left};

    public class Direction
    {
        public static Direction Up = new Direction(DirectionEnum.Up);
        public static Direction Right = new Direction(DirectionEnum.Right);
        public static Direction Down = new Direction(DirectionEnum.Down);
        public static Direction Left = new Direction(DirectionEnum.Left);

        public static Direction[] directions = { Up, Right, Down, Left };

        private DirectionEnum _Direction;

        private Direction(DirectionEnum direction)
        {
            this._Direction = direction;
        }

        public bool isOpposite(Direction other)
        {
            return Math.Abs(_Direction - other._Direction) == 2;
        }

        public Direction Opposite
        {
            get
            {
                return directions[(((int)_Direction + 2) % 4)];
            }
        }
        public bool isUp()
        {
            return _Direction == DirectionEnum.Up;
        }

        public bool isRight()
        {
            return _Direction == DirectionEnum.Right;
        }

        public bool isDown()
        {
            return _Direction == DirectionEnum.Down;
        }

        public bool isLeft()
        {
            return _Direction == DirectionEnum.Left;
        }

        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is Direction) && ((Direction)obj)._Direction == _Direction;
        }

        public override int GetHashCode()
        {
            return _Direction.GetHashCode();
        }

        public override string ToString()
        {
            switch (_Direction)
            {
                case DirectionEnum.Up: return "up";
                case DirectionEnum.Down: return "down";
                case DirectionEnum.Left: return "left";
                case DirectionEnum.Right: return "right";
            }
            return "?";
        }

        public DirectionEnum ToEnum()
        {
            return _Direction;
        }
    }
}
