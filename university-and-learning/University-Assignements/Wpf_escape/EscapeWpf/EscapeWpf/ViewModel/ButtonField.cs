using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeWpf.ViewModel
{
    public class ButtonField : ViewModelBase
    {
        public int x;
        public int y;
        public ButtonField(string s, int xC, int yC)
        {
            FColor = s;
            x = xC;
            y = yC;
        }
        public string FColor;
        public string FieldColor { get { return FColor; } set { FColor = value; OnPropertyChanged("FieldColor"); } }
    }
}
