using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core
{
    public enum GhostStateEnum {NONE, HUNT, RANDOM, FLEE, EYE, HOUSE, REST };
    public class GhostState
    {
        public static GhostState NONE = new GhostState(GhostStateEnum.NONE);
        public static GhostState HUNT = new GhostState(GhostStateEnum.HUNT);
        public static GhostState RANDOM = new GhostState(GhostStateEnum.RANDOM);
        public static GhostState FLEE = new GhostState(GhostStateEnum.FLEE);
        public static GhostState EYE = new GhostState(GhostStateEnum.EYE);
        public static GhostState HOUSE = new GhostState(GhostStateEnum.HOUSE);
		  public static GhostState REST = new GhostState(GhostStateEnum.REST);

        private GhostStateEnum _State;

        private GhostState(GhostStateEnum state)
        {
            this._State = state;
        }

        public bool isHunt()
        {
            return _State == GhostStateEnum.HUNT;
        }
        public bool isRandom()
        {
            return _State == GhostStateEnum.RANDOM;
        }
        public bool isFlee()
        {
            return _State == GhostStateEnum.FLEE;
        }
		  public bool isRest()
		  {
			  return _State == GhostStateEnum.REST;
		  }
        public bool isEye()
        {
            return _State == GhostStateEnum.EYE;
        }
        public bool isHouse()
        {
            return _State == GhostStateEnum.HOUSE;
        }

        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is GhostState) && ((GhostState)obj)._State == _State;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public GhostStateEnum ToEnum()
        {
            return _State;
        }
    }
}
