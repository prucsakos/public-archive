using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Escape.Persistence;

namespace Escape.Model
{
    public class EscapeModel
    {
        public EscapePresistence _saveAndLoad;
        public Unit _player;
        private bool[,] _map;  // false: üres mező, true: akna
        public List<Unit> _enemies;
        public System.Timers.Timer _timer;
        private int _enemyCount;
        private int _size;
        public List<Unit> _mines;
        

        public EscapeModel()
        {
            _timer = new System.Timers.Timer(300);
            _timer.Elapsed += OnElapsed;
            _saveAndLoad = new EscapePresistence();
            _saveAndLoad.GameLoad += OnLoadGame;
        }
        List<Unit> Mines { get { return _mines; } }
        public bool[,] GetMap { get { return _map; } }
        Unit Player { get { return _player; } }
        List<Unit> Enemies { get { return _enemies; } }
        public void saveGame()
        {
            _saveAndLoad.saveGame(Player, Enemies, Mines, _size);
            
        }
        public int loadSavedGame()
        {
            try
            {
                return _saveAndLoad.loadGame();
            } catch
            {
                MessageBox.Show("Extrem fatal brutal error: Failed to load game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
        public void loadGame(int size)
        {
            
            _enemyCount = 2;
            _size = size;
            _player = new Unit(0, _size / 2);
            _mines = new List<Unit>();
            _enemies = new List<Unit>();
            _enemies.Add( new Unit(_size - 1, 0) );
            _enemies.Add( new Unit(_size - 1, _size - 1) );

            _map = new bool[_size, _size];
            
            for(int i = 0; i < _size; i++)
            {                
                for(int j = 0; j < _size; j++)
                {
                    _map[i,j] = false;
                }
            }
            Random rnd = new Random();
            for(int i = 0; i < 2; i++)
            {

                int tmpx = rnd.Next(0, size - 1);
                int tmpy = rnd.Next(0, size - 1);
                _mines.Add(new Unit(tmpx, tmpy));
                _map[tmpx,tmpy] = true;
            }
            

        }
        void loadGame(int size, Unit player, Unit e1, Unit e2, Unit min1, Unit min2)
        {
            
            _enemyCount = 2;
            _size = size;
            _player = new Unit(player.X, player.Y);
            _enemies = new List<Unit>();
            _enemies.Add(new Unit(e1.X, e1.Y));
            _enemies.Add(new Unit(e2.X, e2.Y));
            _mines = new List<Unit>();
            _mines.Add(min1);
            _mines.Add(min2);

            _map = new bool[size, size];
            for (int i = 0; i < size; i++)
            {
                
                for (int j = 0; j < size; j++)
                {
                    _map[i,j] = false;
                }
            }
            _map[min1.X, min1.Y] = true;
            _map[min2.X, min2.Y] = true;
            
        }
        private void OnElapsed(Object source, ElapsedEventArgs e)
        {
            stepUnits();
        }
        private void OnLoadGame(Object s, GameSaveEventArgs e)
        {
            loadGame(e.Size, e.Player, e.Enemies[0], e.Enemies[1], e.Mines[0], e.Mines[1]);
        }

        public void stepUnits()
        {
            foreach(Unit u in _enemies)
            {
                if(u.IsActive == false)
                {
                    continue;
                }
                int distX = Math.Abs(u.X - _player.X);
                int distY = Math.Abs(u.Y - _player.Y);
                if(distX <= distY)
                {
                    if( u.Y <= _player.Y)
                    {
                        u.Y += 1;
                    }else
                    {
                        u.Y -= 1;
                    }
                }
                else
                {
                    if( u.X <= _player.X)
                    {
                        u.X += 1;
                    }
                    else
                    {
                        u.X -= 1;
                    }

                }
                if(u.X == _player.X && u.Y == _player.Y)
                {
                    //// LOSE EVENT
                    _timer.Stop();
                    onGameLost();
                    break;
                    
                }
                if(_map[u.X,u.Y] == true)
                {
                    u.IsActive = false;
                    _enemyCount -= 1;
                }
                if (isWin())
                {
                    //// WIN EVENT
                    _timer.Stop();
                    onGameWon();
                    break;
                    
                }
            }
        }
        private bool isWin()
        {
            return ( _enemyCount <= 0 );
        }

        public void stepPlayer(int x, int y)
        {
            int nextX = _player.X + x;
            int nextY = _player.Y + y;
            if(nextX >= 0 && nextX < _size && nextY >= 0 && nextY < _size)
            {
                _player.X = nextX;
                _player.Y = nextY;
            }
        }


        public event EventHandler GameWon;
        public event EventHandler GameLost;

        private void onGameWon()
        {
            if(GameWon != null)
            {
                GameWon(this, EventArgs.Empty);
            }
        }
        private void onGameLost()
        {
            if (GameLost != null)
            {
                GameLost(this, EventArgs.Empty);
                
            }
        }

        public void pauseGame()
        {
            _timer.Stop();
        }
        public void continueGame()
        {
            _timer.Start();
        }

    }
}
