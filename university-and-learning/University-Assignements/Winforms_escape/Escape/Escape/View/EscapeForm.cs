using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Escape.Model;
using System.Timers;

namespace Escape.View
{
    public partial class EscapeForm : Form
    {
        private Button[,] _buttonGrid;
        private Int32 size;

        private EscapeModel _model;
        public EscapeForm()
        {
            InitializeComponent();
            _model = new EscapeModel();
            _model._timer.Elapsed += OnTimerElapsed;
            _model.GameLost += OnLose;
            _model.GameWon += OnWin;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            size = 10;
            KeyPreview = true;
            KeyDown += new KeyEventHandler(OnKeyDown);

        }
        
        private void loadGrid()
        {
            if(_buttonGrid != null)
            {
                foreach(Button b in _buttonGrid)
                {
                    tableLayoutPanel1.Controls.Remove(b);
                }
            }
            _buttonGrid = new Button[size,size];
            tableLayoutPanel1.RowCount = size;
            tableLayoutPanel1.ColumnCount = size;
            for(Int32 i = 0; i < size; i++)
            {                
                for(Int32 j = 0; j < size; j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Size = new Size(30,30);
                    _buttonGrid[i, j].Dock = DockStyle.Fill;
                    _buttonGrid[i, j].Enabled = false;
                    
                    tableLayoutPanel1.Controls.Add(_buttonGrid[i, j], j, i);
                }
            }
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();

            for (int i = 0; i < size; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 1 / size));
                
            }
            for (int j = 0; j < size; j++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1 / size));
            }
        }
        private void refreshTable()
        {
            
            bool[,] m = _model.GetMap;
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    
                    if(m[i,j] == false)
                    {
                        _buttonGrid[i, j].BackColor = Color.White;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackColor = Color.Red;
                    }                    

                }
                
            }
            Unit p = _model._player;

            _buttonGrid[p.X, p.Y].BackColor = Color.Blue;
            List<Unit> lu = _model._enemies;
            foreach (Unit u in lu)
            {
                if (u.IsActive == false) continue;
                _buttonGrid[u.X, u.Y].BackColor = Color.Black;
            }
        }

        private void OnTimerElapsed(Object source, ElapsedEventArgs e)
        {
            refreshTable();
        }
        private void OnWin(Object source, EventArgs e)
        {
            refreshTable();
            MessageBox.Show("Gj, you made it!", "Win", MessageBoxButtons.OK);
        }
        private void OnLose(Object source, EventArgs e)
        {
            refreshTable();
            MessageBox.Show("You have been captured.", "Lose", MessageBoxButtons.OK);
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            size = 11;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            size = 15;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            size = 21;
        }

        private void MenuSLLoadItem_Click(object sender, EventArgs e)
        {
            
            int x = _model.loadSavedGame();
            if(x > 0)
            {
                size = x;
                loadGrid();
                tableLayoutPanel1.Select();
                refreshTable();
            }
        }

        private void MenuSLSaveItem_Click(object sender, EventArgs e)
        {
            _model.saveGame();
            
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text != "Pause")
            {
                buttonPause.Text = "Pause";
                
                MenuSLLoadItem.Enabled = false;
                MenuSLSaveItem.Enabled = false;
            }
            _model.pauseGame();
            loadGrid();
            _model.loadGame(size);
            tableLayoutPanel1.Select();
            _model.continueGame();
            
            
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text == "Pause")
            {
                buttonPause.Text = "Continue";
                _model.pauseGame();
                MenuSLLoadItem.Enabled = true;
                MenuSLSaveItem.Enabled = true;
            } else
            {
                buttonPause.Text = "Pause";
                _model.continueGame();
                MenuSLLoadItem.Enabled = false;
                MenuSLSaveItem.Enabled = false;
            }
            tableLayoutPanel1.Select();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) _model.stepPlayer(-1, 0);
            else if (e.KeyCode == Keys.Down) _model.stepPlayer(1, 0);
            else if (e.KeyCode == Keys.Left) _model.stepPlayer(0, -1);
            else if (e.KeyCode == Keys.Right) _model.stepPlayer(0, 1);
            refreshTable();
        }
    }
}
