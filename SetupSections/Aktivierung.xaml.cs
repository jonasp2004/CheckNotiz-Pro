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

namespace CheckNotiz_Pro.SetupSections
{
    /// <summary>
    /// Interaktionslogik für Aktivierung.xaml
    /// </summary>
    public partial class Aktivierung : Window {
        private void GarbageCollector() {
                GC.Collect();
                GC.WaitForPendingFinalizers();
        }

        public Aktivierung() {
            InitializeComponent();
            GarbageCollector();
        }

        private void btn_continue_Click(object sender, RoutedEventArgs e) {
            Setup setup = new();
            setup.ShowDialog();
            Close();
            GarbageCollector();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            window.DragMove();
            GarbageCollector();
        }

        private void close_Click(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e) {
            GarbageCollector();
            Setup setup = new();
            setup.ShowDialog();
        }
    }
}
