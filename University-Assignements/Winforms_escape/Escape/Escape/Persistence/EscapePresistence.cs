using System;
using System.Collections.Generic;
using System.Text;
using Escape.Model;
using System.IO;
using System.Windows.Forms;

namespace Escape.Persistence
{
    public class EscapePresistence
    {
        /* size
         * p.x
         * p.y
         * e[0].x
         * e[0].y
         * e[1].x
         * e[1].y
         * m[0].x
         * m[0].y
         * m[1].x
         * m[1].y
         */
        public void saveGame(Unit p, List<Unit> e, List<Unit> m, int s)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Game";
            sfd.DefaultExt = ".txt";
            sfd.RestoreDirectory = true;
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(sfd.FileName);
                StringBuilder sb = new StringBuilder();
                sb.Append(s.ToString());
                sb.AppendLine();
                sb.Append(p.X.ToString());
                sb.AppendLine();
                sb.Append(p.Y.ToString());
                sb.AppendLine();
                foreach(Unit u in e)
                {
                    sb.Append(u.X.ToString());
                    sb.AppendLine();
                    sb.Append(u.Y.ToString());
                    sb.AppendLine();
                }
                foreach(Unit u in m)
                {
                    sb.Append(u.X.ToString());
                    sb.AppendLine();
                    sb.Append(u.Y.ToString());
                    sb.AppendLine();
                }
                writer.Write(sb.ToString());
                writer.Close();
            }
        }
        public int loadGame()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select saving";
            ofd.Filter = "txt files(*.txt)| *.txt";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                
                    StreamReader reader = new StreamReader(ofd.FileName);
                    int size = int.Parse(reader.ReadLine());
                    Unit p = new Unit(int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
                    Unit e1 = new Unit(int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
                    Unit e2 = new Unit(int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
                    Unit m1 = new Unit(int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
                    Unit m2 = new Unit(int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
                    List<Unit> Enemies = new List<Unit>();
                    Enemies.Add(e1);
                    Enemies.Add(e2);
                    List<Unit> Mines = new List<Unit>();
                    Mines.Add(m1);
                    Mines.Add(m2);
                    GameSaveEventArgs args = new GameSaveEventArgs(p, Enemies, Mines, size);
                    GameLoad(this, args);
                    return size;
   
            }
            return -1;
        }
        public event EventHandler<GameSaveEventArgs> GameLoad;
    }
}
