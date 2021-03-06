using MySqlConnector;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CheckNotiz_Pro
{
    /// <summary>
    /// Interaktionslogik für clearID.xaml
    /// </summary>
    public partial class clearID : Window {

        public string cs = @"server=" + Properties.Settings.Default.ip +
                ";userid=" + Properties.Settings.Default.user +
                ";password=" + Properties.Settings.Default.passwrd +
                ";database=" + Properties.Settings.Default.database;

        private void GarbageCollector() {
            if (Properties.Settings.Default.useGarbageCollector == true) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public clearID() {
            InitializeComponent();
            GarbageCollector();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e) {
            Close();
            GarbageCollector();
        }

        private void btn_resume_Click(object sender, RoutedEventArgs e) {

            if (id_textbox.Text == "" && Properties.Settings.Default.useIDsWithoutPWD == false) {
                MessageBox.Show("Bitte geben Sie eine ID-Nummer ein!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            } else {
                DeletionRequest dialog = new();
                dialog.ShowDialog();
            }

            if (Properties.Settings.Default.clearID == true) {
                Properties.Settings.Default.clearID = false;
                Properties.Settings.Default.Save();
            }
            GarbageCollector();
        }

        private void titlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                window.DragMove();
            }
            GarbageCollector();
        }

        private void window_Activated(object sender, EventArgs e) {
            if (Properties.Settings.Default.clearID == true) {
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

                        string tempHashedPW = pw_textbox.Text;
                        if (Properties.Settings.Default.hashPasswords == true) {
                            #pragma warning disable SYSLIB0021
                            using SHA384 sha = new SHA384CryptoServiceProvider();
                            byte[] preHash = Encoding.ASCII.GetBytes(pw_textbox.Text);
                            byte[] hash = sha.ComputeHash(preHash);
                            tempHashedPW = Convert.ToBase64String(hash);
                        }
                            if (tempHashedPW != loggedInPW) {
                            MessageBox.Show("Bitte überprüfen SIe Ihre Eingaben!");
                        } else {
                            rdr.Close();
                            con.Close();
                            try {
                                con.Open();
                                sql = "UPDATE `" + Properties.Settings.Default.table + "` SET `" + Properties.Settings.Default.col2 + "`='' WHERE `id`=@id";
                                using var delcmd = new MySqlCommand(sql, con);
                                delcmd.Parameters.AddWithValue("@id", id_textbox.Text);
                                delcmd.Prepare();
                                delcmd.ExecuteNonQuery();
                                MessageBox.Show("Ihr ID-Inhalt wurde geleert.", "Erfolg");
                                Properties.Settings.Default.clearID = false;
                                con.Close();
                            } catch {
                                MessageBox.Show("Verbindung zum Server nicht möglich!", "Warnung");
                            }
                        }
                    }
                } catch {
                    Properties.Settings.Default.clearID = false;
                }
            }
            GarbageCollector();
        }

        private void pw_textbox_LostFocus(object sender, RoutedEventArgs e) { if (pw_textbox.Text != "" && Properties.Settings.Default.blurPasswdBoxes == true) { blur.Radius = 15; } GarbageCollector(); }
        private void pw_textbox_GotFocus(object sender, RoutedEventArgs e) { blur.Radius = 0; GarbageCollector(); }
    }
}
