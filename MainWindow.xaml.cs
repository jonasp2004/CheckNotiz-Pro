using MySqlConnector;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CheckNotiz_Pro {
    public partial class MainWindow : Window {

        private void GarbageCollector() {
            if (Properties.Settings.Default.useGarbageCollector == true) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public string cs = @"server=" + Properties.Settings.Default.ip +
            ";userid=" + Properties.Settings.Default.user +
            ";password=" + Properties.Settings.Default.passwrd +
            ";database=" + Properties.Settings.Default.database;

        public MainWindow() {
            if (Properties.Settings.Default.setupDone == false) {
                Setup setup = new();
                setup.ShowDialog();
            } else { 

            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/checknotiz";
            try {
                if (Directory.Exists(path)) { return; }
                DirectoryInfo di = Directory.CreateDirectory(path);
                FileStream fi = File.Create(path + "pronotizen.chnf");
                MessageBox.Show("Die Dateien wurden entpackt. Starten Sie bitte das Programm erneut, um fortzufahren.", "Alles fertig!", MessageBoxButton.OK, MessageBoxImage.Information);
            } finally { }
            GarbageCollector();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                window.DragMove();
                GarbageCollector();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void closeWIndow_MouseUp(object sender, MouseButtonEventArgs e) {
            GarbageCollector();
            Close();
        }

        private void minimizeWindow_MouseUp(object sender, MouseButtonEventArgs e) {
            WindowState = WindowState.Minimized;
            GarbageCollector();
        }

        private void btn_showNewNoteDialog_Click(object sender, RoutedEventArgs e) {
            dialog_newNote.Visibility = Visibility.Visible;
            GarbageCollector();
        }

        private void Button_cancelNoteCreation_Click(object sender, RoutedEventArgs e) {
            noteContent.Text = "";
            dialog_newNote.Visibility = Visibility.Collapsed;
            GarbageCollector();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e) {
            idOptions idoptions = new();
            idoptions.Show();
            GarbageCollector();
        }

        private void Button_createNote_Click(object sender, RoutedEventArgs e) {
            string tempTitle = noteContent.Text.Replace(' ', 'é');
            noteList.Items.Add(new Label() { Name = "tempTitle", Content = noteContent.Text });
            noteContent.Text = "";
            dialog_newNote.Visibility = Visibility.Collapsed;
            SaveList();
            GarbageCollector();
        }

        private void btn_UpdateNote_Click(object sender, RoutedEventArgs e) {
            noteList.Items.Clear();
            try {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\pronotizen.chnf")) {
                    while (!sr.EndOfStream) {
                        for (int i = 0; i < 4; i++) {
                            #pragma warning disable CS8600
                            string strListItem = sr.ReadLine();
                            if (!String.IsNullOrEmpty(strListItem))
                                noteList.Items.Add(new Label() { Name = "tempTitle", Content = strListItem.Replace("System.Windows.Controls.Label: ", "") });
                        }
                    }
                }
            } catch {
                MessageBox.Show("Ein Fehler ist aufgetreten!", "Achtung!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            GarbageCollector();
        }

        private void btn_DeleteNote_Click(object sender, RoutedEventArgs e) {
            if (noteList.SelectedIndex != -1) {
                noteList.Items.Remove(noteList.SelectedItems[0]);
            }
            SaveList();
            GarbageCollector();
        }

        private void SaveList() {
            StreamWriter SaveFile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\pronotizen.chnf");
            #pragma warning disable CS8602
            foreach (var item in noteList.Items) { SaveFile.WriteLine(item.ToString().Replace("System.Windows.Controls.Label: ", "")); }
            SaveFile.Close();
            GarbageCollector();
        }

        private void pw_textbox_GotFocus(object sender, RoutedEventArgs e) {
            blur_PasswordBox.Radius = 0;
            GarbageCollector();
        }

        private void pw_textbox_LostFocus(object sender, RoutedEventArgs e) {
            GarbageCollector();
            if (pw_textbox.Text != "" && Properties.Settings.Default.blurPasswdBoxes == true) {
                blur_PasswordBox.Radius = 15;
            }
        }

        private void maximizeButton_MouseUp(object sender, MouseButtonEventArgs e){
            if (window.WindowState == WindowState.Normal) {
                winborder.Margin = new Thickness(0, 0, 0, 50);
                window.WindowState = WindowState.Maximized;
            } else {
                winborder.Margin = new Thickness(10);
                window.WindowState = WindowState.Normal;
            }
            GarbageCollector();
        }

        private void btn_closeOnlineSyncDialog_Click(object sender, RoutedEventArgs e){
            dialog_onlineSync.Visibility = Visibility.Collapsed;
            GarbageCollector();
        }

        private void btn_viewOnlineConnectPopup_Click(object sender, RoutedEventArgs e) {
            dialog_onlineSync.Visibility = Visibility.Visible;
            GarbageCollector();
        }

        private void Button_createNote1_Click(object sender, RoutedEventArgs e) {
            if (Properties.Settings.Default.useIDsWithoutPWD == false && pw_textbox.Text == "") {
                MessageBox.Show("Bitte geben Sie ein Passwort ein.", "Achtung!", MessageBoxButton.OK, MessageBoxImage.Warning);
            } else { 
            try {
                using var con = new MySqlConnection(cs);
                con.Open();
                string sql = "SELECT * FROM " + Properties.Settings.Default.table + " WHERE id =" + id_textbox.Text + ";";
                using var cmd = new MySqlCommand(sql, con);
                using MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    Console.WriteLine("{0} {1} {2}", rdr.GetString(0), rdr.GetString(1),
                    rdr.GetString(2));
                    string loggedInId = rdr.GetString(0);
                    string loggedInPW = rdr.GetString(2);
#pragma warning disable SYSLIB0021
                    if (Properties.Settings.Default.hashPasswords == true) {
                        using SHA384 sha = new SHA384CryptoServiceProvider();
                        byte[] preHash = Encoding.ASCII.GetBytes(pw_textbox.Text);
                        byte[] hash = sha.ComputeHash(preHash);
                        string tempHashedPW = Convert.ToBase64String(hash);

                        if (tempHashedPW != loggedInPW) {
                            MessageBox.Show("Bitte überprüfen SIe Ihre Eingaben!");
                        } else {
                            conn_success_text.Visibility = Visibility.Visible;
                            id_textbox.IsReadOnly = true;
                            pw_textbox.IsReadOnly = true;
                            btn_onlineAufholen.Visibility = Visibility.Visible;
                            btn_onlineUpload.Visibility = Visibility.Visible;
                            link_idoptions.Visibility = Visibility.Collapsed;
                            btn_cancelOnlineConnection.Visibility = Visibility.Visible;
                            Button_createNote1.Visibility = Visibility.Collapsed;
                        }
                    } else {
                        if (pw_textbox.Text != loggedInPW) {
                            MessageBox.Show("Bitte überprüfen SIe Ihre Eingaben!");
                        } else {
                            conn_success_text.Visibility = Visibility.Visible;
                            id_textbox.IsReadOnly = true;
                            pw_textbox.IsReadOnly = true;
                            btn_onlineAufholen.Visibility = Visibility.Visible;
                            btn_onlineUpload.Visibility = Visibility.Visible;
                            link_idoptions.Visibility = Visibility.Collapsed;
                            btn_cancelOnlineConnection.Visibility = Visibility.Visible;
                            Button_createNote1.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            } catch {

            }
            }
            GarbageCollector();
        }

        private void btn_cancelOnlineConnection_Click(object sender, RoutedEventArgs e) {
            conn_success_text.Visibility = Visibility.Collapsed;
            id_textbox.IsReadOnly = false;
            pw_textbox.IsReadOnly = false;
            btn_onlineAufholen.Visibility = Visibility.Collapsed;
            btn_onlineUpload.Visibility = Visibility.Collapsed;
            link_idoptions.Visibility = Visibility.Visible;
            btn_cancelOnlineConnection.Visibility = Visibility.Collapsed;
            Button_createNote1.Visibility = Visibility.Visible;
            GarbageCollector();
        }

        private void resetSettings_MouseDown(object sender, MouseButtonEventArgs e) {
            GarbageCollector();
            Properties.Settings.Default.Reset();
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e) {
            GarbageCollector();
        }

        private void btn_onlineUpload_Click(object sender, RoutedEventArgs e) {
            if (conn_success_text.Visibility == Visibility.Visible) {
                try {
                    string filecontents = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\pronotizen.chnf");
                    using var con = new MySqlConnection(cs);
                    con.Open();
                    var sql = "UPDATE `" + Properties.Settings.Default.table + "` SET `" + Properties.Settings.Default.col2 + "`='" + filecontents + "' WHERE `id`=@id";
                    using var cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", id_textbox.Text);
                    string filecontent = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/checknotiz/pronotizen.chnf", Encoding.UTF8);
                    cmd.Parameters.AddWithValue("@text", filecontent);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Ihr Datensatz wurde hochgeladen.", "Erfolg");
                    con.Close();
                }
                catch {
                    MessageBox.Show("Verbindung zum Server nicht möglich!", "Warnung");
                }
            } else {
                MessageBox.Show("Bitte melden Sie sich zuerst mit ihrer ID an.", "Info");
            }
            GarbageCollector();
        }

        private void btn_onlineAufholen_Click(object sender, RoutedEventArgs e) {
            try {
                using var con = new MySqlConnection(cs);
                con.Open();
                var sql = "SELECT * FROM `" + Properties.Settings.Default.table + "` WHERE `id`=" + id_textbox.Text + "";
                using var cmd = new MySqlCommand(sql, con);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    string fileContent = rdr.GetString(1);
                    StreamWriter SaveFile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\pronotizen.chnf");
                    SaveFile.WriteLine(fileContent);
                    SaveFile.Close();
                }
            } catch {
                MessageBox.Show("Ihr Datensatz konnte nicht abgerufen werden, da die Verbindung zum Server scheiterte.", "Warnung");
            }
            GarbageCollector();
        }

        private void TitleBarSettingsButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void startSetup_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Setup setup = new Setup();
            setup.ShowDialog();
        }
    }
}