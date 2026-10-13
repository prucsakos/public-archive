using System;
using System.Collections.Generic;
using System.Text;
using EscapeWpf.Model;

namespace EscapeWpf.Persistence
{
    public class GameSaveEventArgs : EventArgs
    {
        public Unit Player { get; private set; }
        public List<Unit> Enemies { get; private set; }
        public List<Unit> Mines { get; private set; }
        public int Size { get; private set; }
        public GameSaveEventArgs(Unit p, List<Unit> e, List<Unit> m, int s)
        {
            Player = p;
            Enemies = e;
            Mines = m;
            Size = s;
        }
    }
}
