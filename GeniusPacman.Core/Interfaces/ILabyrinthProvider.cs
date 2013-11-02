using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core.Interfaces
{
    /// <summary>
    /// Implements this interface, to provide yours labyrinths
    /// 
    /// </summary>
    public interface ILabyrinthProvider
    {
        /// <summary>
        /// You must provide an array of array of byte with these dimensions : byte[31][28]
        /// </summary>
        /// <param name="level">desired level</param>
        /// <returns></returns>
        byte[][] GetLevel(int level);
        /// <summary>
        /// returns the number of level that can be provided
        /// </summary>
        int LevelCount { get; }

        /// <summary>
        /// returns the number of pills for specified level 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        int GetPillsNumber(int level);
    }
}
