using System;
using System.Collections.Generic;
using System.Text;
using EscapeWpf.ViewModel;
using EscapeWpf.Model;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using EscapeWpf;
using System.Windows.Input;

namespace EscapeWpf.ViewModel
{
    class EscapeViewModel : ViewModelBase
    {
        private EscapeModel _model;
        private bool isPaused;
        private string pauseStr;
        private int mapSize;

        public ObservableCollection<ButtonField> Fields { get; set; }
        public int MapSize { get { return mapSize; } private set { } }
        public MyICommand NewGameCommand { get; private set; }
        public MyICommand SaveGameCommand { get; private set; }
        public MyICommand LoadGameCommand { get; private set; }
        public MyICommand PauseCommand { get; private set; }
        public string PauseString { get { return pauseStr; } private set { } }
        public MyICommand SizeChanged11 { get; private set; }
        public MyICommand SizeChanged15 { get; private set; }
        public MyICommand SizeChanged21 { get; private set; }


        public EscapeViewModel()
        {
            _model = new EscapeModel();
            _model.GameLost += OnGameLost;
            _model.GameWon += OnGameWon;
            _model._timer.Elapsed += OnElapsed;
            
            
            isPaused = true;
            pauseStr = "Folytatás";
            mapSize = 11;
            
            NewGameCommand = new MyICommand(param => OnNewGame());
            SaveGameCommand = new MyICommand(param => OnSaveGame());
            LoadGameCommand = new MyICommand(param => OnLoadGame());
            PauseCommand = new MyICommand(param => OnPause());
            SizeChanged11 = new MyICommand(param => OnSizeChanged11());
            SizeChanged15 = new MyICommand(param => OnSizeChanged15());
            SizeChanged21 = new MyICommand(param => OnSizeChanged21());
            _model.loadGame(mapSize);
            Fields = new ObservableCollection<ButtonField>();
            generateTable();


            
            
        }
        private void refreshTable()
        {
            for (int i = 0; i < _model._size; i++)
            {
                for (int j = 0; j < _model._size; j++)
                {
                    Fields[i * _model._size + j].FieldColor = _model.getColor(i, j);
                }
            }
        }

        private void OnElapsed(Object source, EventArgs e)
        {
            refreshTable();
        }

        private void OnGameWon(Object source, EventArgs e)
        {
            refreshTable();
            MessageBox.Show("Gj, you made it!", "Win", MessageBoxButton.OK);
        }

        private void OnGameLost(Object source, EventArgs e)
        {
            refreshTable();
            MessageBox.Show("You have been captured.", "Lose", MessageBoxButton.OK);
        }

        private void OnSizeChanged15()
        {
            mapSize = 15;
        }

        private void OnSizeChanged21()
        {
            mapSize = 21;
        }

        private void OnSizeChanged11()
        {
            mapSize = 11;
        }

        private void OnPause()
        {
            
            if (!isPaused)
            {
                pauseGame();
            }
            else
            {
                pauseStr = "Szünet";
                isPaused = false;
                _model.continueGame();
                OnPropertyChanged("PauseString");
                
            }
        }

        private void OnLoadGame()
        {
            mapSize= _model.loadSavedGame();
            OnPropertyChanged("MapSize");
            generateTable();
            refreshTable();
        }

        private void OnSaveGame()
        {
            _model.saveGame();
        }

        private void OnNewGame()
        {
            pauseGame();
            _model.loadGame(mapSize);
            OnPropertyChanged("MapSize");
            generateTable();
            refreshTable();
        }


        private void generateTable()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    Fields.Add(new ButtonField(_model.getColor(i, j), i, j));
                }
            }
        }
        private void pauseGame()
        {
            pauseStr = "Folytatás";
            isPaused = true;
            _model.pauseGame();
            OnPropertyChanged("PauseString");
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!isPaused) {
                if (e.Key == Key.Up) _model.stepPlayer(-1, 0);
                else if (e.Key == Key.Down) _model.stepPlayer(1, 0);
                else if (e.Key == Key.Left) _model.stepPlayer(0, -1);
                else if (e.Key == Key.Right) _model.stepPlayer(0, 1);
                refreshTable();
            }
        }
    }
}
