using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Shamai_R4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                var mainwindow = new MainWindow()
                {
                    Title = @"Shamia",
                    TitleCaps = false,
                    ShowTitleBar = true,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    GlowBrush = new SolidColorBrush(Colors.DodgerBlue),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    IsWindowDraggable = true
                };
                mainwindow.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
