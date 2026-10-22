using System;
using System.Collections.Generic;
using System.Text;

namespace Escape.Model
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
