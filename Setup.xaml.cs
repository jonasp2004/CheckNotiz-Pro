using CheckNotiz_Pro.SetupSections;
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

namespace CheckNotiz_Pro
{
    /// <summary>
    /// Interaktionslogik für Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {

        public Setup()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                window.DragMove();
            }
        }

        private void btn_viewOnlineConnectPopup_Click(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }

        private void btn_continue_Click(object sender, RoutedEventArgs e) {
            Aktivierung act = new();
            act.ShowDialog();
            Close();
        }
    }
}
