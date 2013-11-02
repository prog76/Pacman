using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core.Interfaces
{
    public interface IPacmanTimer
    {
        void Start();
        void Stop();
        
        bool IsStarted {get;}
        int Elapsed { get; set; }

        event EventHandler Tick;
    }
}
