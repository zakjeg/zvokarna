using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace xamlPianoRoll.View
{
    /// <summary>
    /// Interaction logic for popupMenu.xaml
    /// </summary>
    public partial class popupMenu : Window
    {
        public popupMenu()
        {
            InitializeComponent();
        }


        private void AddPiano_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddMarimba_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddDrums_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddGuitar_Click(object sender, RoutedEventArgs e)
        {

        }


        //OSNOVNE FUNKCIJE OKNA
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close(); //zapre okno
                     //Application.Current.Shutdown(); //zapre celotno aplicacijo
        }

    }
}
