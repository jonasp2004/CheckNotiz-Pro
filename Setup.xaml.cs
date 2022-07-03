using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CheckNotiz_Pro {
    /// <summary>
    /// Interaktionslogik für Setup.xaml
    /// </summary>
    public partial class Setup : Window {

        public string cs = @"server=sql11.freemysqlhosting.net;userid=sql11503728;password=Bi6RysUDIU;database=sql11503728";
        private void GarbageCollector() {
            if (Properties.Settings.Default.useGarbageCollector == true) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public Setup()
        {
            InitializeComponent();
            if (Properties.Settings.Default.useOwnServer == true) {
                useOwnServer.IsChecked = true;
            }
            if (Properties.Settings.Default.blurPasswdBoxes == true) {
                blurPasswdBoxes.IsChecked = true;
            }
            if (Properties.Settings.Default.useGarbageCollector == true) {
                useGarbageCollector.IsChecked = true;
            }
            GarbageCollector();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                window.DragMove();
            }
            GarbageCollector();
        }

        private void btn_viewOnlineConnectPopup_Click(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown();
            GarbageCollector();
        }

        private void btn_goBackToWelcome_Click(object sender, RoutedEventArgs e) {
            welcomeSection.Visibility = Visibility.Visible;
            activationSection.Visibility = Visibility.Collapsed;
            GarbageCollector();
        }

        private void btn_continue_Click(object sender, RoutedEventArgs e) {
            welcomeSection.Visibility = Visibility.Collapsed;
            activationSection.Visibility = Visibility.Visible;
            GarbageCollector();
        }

        private void productkey_TextChanged(object sender, TextChangedEventArgs e) {
            if (productkey.Text == "JonasStinkt") {
                MessageBoxResult eastereeg = MessageBox.Show("Willst du die Produktschlüsseleingabe überspringen?", "Hey, kleiner Entwickler...", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (eastereeg) {
                    case MessageBoxResult.Yes:
                        ActivationIndicator.Foreground = Brushes.Green;
                        ActivationIndicator.Text = "Ihr Programm ist aktiviert!";
                        break;
                }
            } else {
                ActivationIndicator.Foreground = Brushes.Black;
                ActivationIndicator.Text = "Geben Sie einen gültigen Produktschlüssel ein";
            }
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

        private void useOwnServer_Checked(object sender, RoutedEventArgs e) {
            btn_configureServer.IsEnabled = true;
            GarbageCollector();
        }



        private void useGarbageCollector_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.useGarbageCollector = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void useGarbageCollector_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.useIDsWithoutPWD = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void blurPasswdBoxes_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.blurPasswdBoxes = true;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void blurPasswdBoxes_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.blurPasswdBoxes = false;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }
        private void btn_configureServer_Click(object sender, RoutedEventArgs e) {
            ServerConnection serverconfig = new();
            serverconfig.ShowDialog();
            GarbageCollector();
        }

        private void btn_goBackToProductkey_Click(object sender, RoutedEventArgs e) {
            activationSection.Visibility = Visibility.Visible;
            privacySection.Visibility = Visibility.Collapsed;
            GarbageCollector();
        }

        private void btn_goToPrivacySettings_Click(object sender, RoutedEventArgs e) {
            connectingIndicator.Visibility = Visibility.Visible;
            if (ActivationIndicator.Foreground == Brushes.Green) {
                activationSection.Visibility = Visibility.Collapsed;
                privacySection.Visibility = Visibility.Visible;
            } else {
                try {
                    using var con = new MySqlConnection(cs);
                    con.Open();
                    string sql = "SELECT * FROM activation WHERE prdkey ='" + productkey.Text + "';";
                    using var cmd = new MySqlCommand(sql, con);
                    using MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read()) {
                        Console.WriteLine("{0} {1}", rdr.GetString(0), rdr.GetString(1),
                        rdr.GetString(0));
                        string key = rdr.GetString(0);
                        string isThisKeyUsed = rdr.GetString(1);

                        if (isThisKeyUsed == "false") {
                            ActivationIndicator.Foreground = Brushes.Green;
                            ActivationIndicator.Text = "Ihr Programm ist aktiviert!";
                            connectingIndicator.Visibility = Visibility.Collapsed;
                        }

                    }
                }
                catch {
                    MessageBox.Show("Ein unerwarteter Fehler ist aufgetreten.Überprüfen Sie ihre Internetverbindung oder ihre Eingabe.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    connectingIndicator.Text = "Aktivierungsserver kann derzeit nicht kontakiert werden";
                }

            }
            GarbageCollector();
        }

        private void btn_finishSetup_Click(object sender, RoutedEventArgs e) {
            GarbageCollector();
            window.Close();
        }

        private void btn_goToFileSettings_Click(object sender, RoutedEventArgs e) {
            privacySection.Visibility = Visibility.Collapsed;
            fileSection.Visibility = Visibility.Visible;
            Properties.Settings.Default.setupDone = true;
            Properties.Settings.Default.Save();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/checknotiz";
            try {
                if (Directory.Exists(path)) { return; }
                DirectoryInfo di = Directory.CreateDirectory(path);
                FileStream fi = File.Create(path + "pronotizen.chnf");
            }  catch {
                MessageBox.Show("Achtug! Die Speicherdatei konnte nicht angelegt werden. Sie werden möglicherweise keine Notizen erstellen können.", "Achtung!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void importMyOldNotes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ConvertAssistant convert = new();
            convert.ShowDialog();
        }
    }

}