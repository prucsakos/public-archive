using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeWpf.Model
{
    public class Unit
    {
        public int X;
        public int Y;
        public bool IsActive;
        public Unit(int x, int y)
        {
            X = x;
            Y = y;
            IsActive = true;
        }
        
    }
}
