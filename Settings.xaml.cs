using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CheckNotiz_Pro
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window {
        private void GarbageCollector() { 
            if (Properties.Settings.Default.useGarbageCollector == true) {
                GC.Collect(); 
                GC.WaitForPendingFinalizers();
            }
        }

        public Settings() {
            Properties.Settings.Default.resetApplication = false;
            Properties.Settings.Default.Save();
            InitializeComponent();
            LoadSettings();
            GarbageCollector();
        }

        private void LoadSettings() {
            if (Properties.Settings.Default.useOwnServer == true) {
                useOwnServer.IsChecked = true; }
            if (Properties.Settings.Default.hashPasswords == true) {
                hashPasswords.IsChecked = true; }
            if (Properties.Settings.Default.blurPasswdBoxes == true) {
                blurPasswdBoxes.IsChecked = true; }
            if (Properties.Settings.Default.useIDsWithoutPWD == true) {
                useIDsWithoutPWD.IsChecked = true; }
            if (Properties.Settings.Default.useGarbageCollector == true) {
                useGarbageCollector.IsChecked = true; }
            if (Properties.Settings.Default.useBackdropBlur == true) {
                useBackdropBlur.IsChecked = true; }
            GarbageCollector();
        }
        private void titlebar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) { window.DragMove(); } }
        private void closeWIndow_MouseUp(object sender, MouseButtonEventArgs e) {
            Close(); }

        private void useOwnServer_Checked(object sender, RoutedEventArgs e) {
            btn_configureServer.IsEnabled = true;
            GarbageCollector();
        }

        private void useOwnServer_Unchecked(object sender, RoutedEventArgs e) {
            btn_configureServer.IsEnabled = false;
            Properties.Settings.Default.useOwnServer = false;
            Properties.Settings.Default.ip = "sql11.freemysqlhosting.net";
            Properties.Settings.Default.database = "sql11503728";
            Properties.Settings.Default.table = "shared";
            Properties.Settings.Default.col1 = "id";
            Properties.Settings.Default.col2 = "note";
            Properties.Settings.Default.col3 = "passwrd";
            Properties.Settings.Default.user = "sql11503728";
            Properties.Settings.Default.passwrd = "Bi6RysUDIU";
            Properties.Settings.Default.useOwnServer = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }


        private void hashPasswords_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.hashPasswords = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void hashPasswords_Unchecked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.hashPasswords = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void blurPasswdBoxes_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.blurPasswdBoxes = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void blurPasswdBoxes_Unchecked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.blurPasswdBoxes = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useIDsWithoutPWD_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.useIDsWithoutPWD = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useIDsWithoutPWD_Unchecked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.useIDsWithoutPWD = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useGarbageCollector_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.useGarbageCollector = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useGarbageCollector_Unchecked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.useIDsWithoutPWD = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useBackdropBlur_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.useBackdropBlur = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useBackdropBlur_Unchecked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.useBackdropBlur = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void btn_configureServer_Click(object sender, RoutedEventArgs e)
        {
            ServerConnection serverconfig = new();
            serverconfig.ShowDialog();
        }

        private void btn_resetApplication_Click(object sender, RoutedEventArgs e)
        {
            ResetApplication reset = new();
            reset.ShowDialog();
        }

        private void btn_convert_Click(object sender, RoutedEventArgs e)
        {
            ConvertAssistant convert = new();
            convert.Show();
        }

        private void window_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.resetApplication == true)
            {
                GarbageCollector();
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/checknotiz";
                Directory.Delete(dir, true);
                Properties.Settings.Default.Reset();
                MessageBox.Show("Das Programm wurde zurückgesetzt. Die Anwendung wird nun geschlossen.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
