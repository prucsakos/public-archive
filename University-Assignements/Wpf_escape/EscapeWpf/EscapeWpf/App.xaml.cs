using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EscapeWpf.ViewModel;

namespace EscapeWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _mwindow;
        private EscapeViewModel _vmodel;
        App()
        {
            Startup += App_Startup;
        }
        void App_Startup(object sender, StartupEventArgs eventArgs)
        {
            _mwindow = new MainWindow();
            _vmodel = new EscapeViewModel();
            
            _mwindow.KeyDown += _vmodel.OnKeyDown;
            _mwindow.DataContext = _vmodel;
            _mwindow.Show();
        }
    }
}
