using System;
using System.Collections.Generic;
using System.Text;
using GeniusPacman.Core.Strategies;

namespace GeniusPacman.Core.Sprites
{
    public class Ghost : Sprite
    {
        private GhostState _State;
        private Strategy strategy;
        private int color; // 0, 1, 2 ou 3 (ROUGE,  VERT,  JAUNE ou CYAN)
        private bool blink;
        private int houseTime;
        private int fleeTime;
        private int randomTime;
        private int xpac;
        private int ypac;
		  public double speedDevider;

        public Ghost(int color)
        {
            this.color = color;
            init();
        }

        public void setPacmanCoor(int xpac, int ypac)
        {
            this.xpac = xpac;
            this.ypac = ypac;
        }

        public void init()
        {
            spriteNum = 0;
            spriteCount = 0;
            _DesiredDirection = Direction.Down;
            CurrentDirection = Game.GetRandom(2) == 0 ? Direction.Left : Direction.Right;
            setState(GhostState.HOUSE);
            blink = false;
            X = Labyrinth.HOUSE_X * Constants.GRID_WIDTH + color * Constants.GRID_WIDTH_2;
            Y = Labyrinth.HOUSE_Y * Constants.GRID_HEIGHT;
        }

        public Direction getOldDirection()
        {
            return _DesiredDirection;
        }
        public void setOldDirection(Direction direction)
        {
            this._DesiredDirection = direction;
        }

        private int getYeuxNum()
        {
            int res = 9;
            switch (CurrentDirection.ToEnum())
            {
                case DirectionEnum.Up:
                    res = 10;
                    break;
                case DirectionEnum.Right:
                    res = 9;
                    break;
                case DirectionEnum.Down:
                    res = 12;
                    break;
                case DirectionEnum.Left:
                    res = 11;
                    break;
            }
            return res;
        }

        int getSpriteNum()
        {
            int n = 13 + color * 2 + spriteCount;
            if (_State.isFlee())
            {
                if (blink && (spriteCount & 1) == 1)
                {
                    n = 13 + color * 2 + spriteCount;
                }
                else
                {
                    n = 21 + spriteCount;
                }
            }
            return n;
        }

        public GhostState State
        {
            get
            {
                return _State;
            }
            private set
            {
                if (_State != value)
                {
                    _State = value;
                    DoPropertyChanged("State");
                }
            }
        }

        public int getColor()
        {
            return color;
        }

        public void setState(GhostState newState)
        {
            if (newState.isEye())
            {
                // état demandé : yeux avec déplacement yeux
                State = GhostState.EYE;
                strategy = Strategy.EYE;
            }
            else if (newState.isFlee())
            {
                // état demandé : fuite
                if (!State.isEye())
                {
                    // fuit seulement si pas en état yeux
                    fleeTime = Constants.TIME_FLEE;
                    blink = false;
                    if (!strategy.isHouse())
                    {
                        strategy = Strategy.FLEE;
                        _DesiredDirection = CurrentDirection;
                        CurrentDirection = CurrentDirection.Opposite;
                    }
                    State = GhostState.FLEE;
                }
            }
				else if (newState.isRest())
				{
					fleeTime = Constants.TIME_REST;
					strategy = Strategy.FLEE;
					
					_DesiredDirection = CurrentDirection;
					CurrentDirection = CurrentDirection.Opposite;
					State = GhostState.REST;
				}
				else if (newState.isHouse())
				{
					// état demandé : chasse avec déplacement maison
					strategy = Strategy.HOUSE;
					houseTime = color * Constants.TIME_HOUSE;
					State = GhostState.HOUSE;
				}
				else if (newState.isHunt())
				{
					// état demandé : chasse avec déplacement chasse
					strategy = Strategy.HUNT;
					State = GhostState.HUNT;
				}
				else if (newState.isRandom())
				{
					// état demandé : chasse avec déplacement aléatoire
					strategy = Strategy.RANDOM;
					randomTime = Constants.TIME_RANDOM_OFFSET + color * Constants.TIME_RANDOM_K;
					State = GhostState.HUNT;
				}
        }

		  public bool OnGrid()
		  {
			  return strategy.OnGrid(X, Y);
		  }

        public override int animate()
        {
               spriteCount = 1 - spriteCount;

                // sauvegarde la direction courante avant toute modification
                _DesiredDirection = CurrentDirection;
                // détermine la nouvelle direction en fonction de la stratégie courante
                CurrentDirection = strategy.move(this, X, Y, xpac, ypac);

                // gestion des timers
                timers();
                // déplacement à l'écran
                moveGhost();

            return 0;
        }



        public void timers()
        {
            if (strategy.isHouse())
            {
                houseTime--;
                if (houseTime <= 0 && X == (Labyrinth.HOUSE_X * Constants.GRID_WIDTH) - Constants.GRID_WIDTH_2)
                {
                    // en position pour sortir de la maison
                    if (Y == (Labyrinth.HOUSE_Y - 3) * Constants.GRID_HEIGHT)
                    {
                        // ok la sortie est atteinte
                        if (!_State.isFlee())
                        {
                            setState(GhostState.RANDOM);
                        }
                        // en sortant de la maison certains partent à doite, d'autres à gauche
                        if (color <= 1)
                        {
                            CurrentDirection = Direction.Right;
                        }
                        else
                        {
                            CurrentDirection = Direction.Left;
                        }
                    }
                    else if (Y == Labyrinth.HOUSE_Y * Constants.GRID_HEIGHT)
                    {
                        // dans la maison, en face de la sortie
                        CurrentDirection = Direction.Up;
                    }
                }
            }
            if (strategy.isRandom())
            {
                randomTime--;
                if (randomTime <= 0)
                {
                    setState(GhostState.HUNT);
                }
            }
				if (_State.isRest())
				{
					fleeTime--;
					if (fleeTime <= 0)
					{
						setState(GhostState.RANDOM);
					}
				}
            if (_State.isFlee())
            {
                fleeTime--;
                if (fleeTime <= (Constants.TIME_FLEE/2))
                {
                    blink = true;
                    if (fleeTime <= 0)
                    {
                        setState(GhostState.HUNT);
                        blink = false;
                    }
                }
            }
        }

        private void moveGhost()
        {
            // on deplace le fantome
            switch (CurrentDirection.ToEnum())
            {
                case DirectionEnum.Up:
						Y -= Constants.GRID_HEIGHT_4;
                    break;
                case DirectionEnum.Right:
						  X += Constants.GRID_WIDTH_4;
                    if (X >= (Labyrinth.WIDTH * Constants.GRID_HEIGHT + Constants.GRID_WIDTH_X2))
                    {
                        X = -Constants.GRID_WIDTH_X2;
                    }
                    break;
                case DirectionEnum.Down:
						  Y += Constants.GRID_HEIGHT_4;
                    break;
                case DirectionEnum.Left:
						  X -= Constants.GRID_WIDTH_4;
                    if (X <= -Constants.GRID_WIDTH_X2)
                    {
                        X = (Labyrinth.WIDTH * Constants.GRID_WIDTH + Constants.GRID_WIDTH_X2);
                    }
                    break;
            }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(color);
            sb.Append(", state=");
            if (_State.isEye())
            {
                sb.Append("e");
            }
            else if (_State.isFlee())
            {
                sb.Append("f");
            }
            else if (_State.isHouse())
            {
                sb.Append("ho");
            }
            else if (_State.isHunt())
            {
                sb.Append("hu");
            }
            else if (_State.isRandom())
            {
                sb.Append("r");
            }
            sb.Append(", strategy=");
            if (strategy.isEye())
            {
                sb.Append("e");
            }
            else if (strategy.isFlee())
            {
                sb.Append("f");
            }
            else if (strategy.isHouse())
            {
                sb.Append("ho");
            }
            else if (strategy.isHunt())
            {
                sb.Append("hu");
            }
            else if (strategy.isRandom())
            {
                sb.Append("r");
            }
            sb.Append(", x=");
            sb.Append(xLaby);
            sb.Append(", y=");
            sb.Append(yLaby);
            sb.Append(", d=");
            sb.Append(CurrentDirection);
            sb.Append(", od=");
            sb.Append(_DesiredDirection);
            return sb.ToString();
        }
    }
}
