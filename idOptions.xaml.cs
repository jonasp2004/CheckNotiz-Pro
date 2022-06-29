using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaktionslogik für idOptions.xaml
    /// </summary>
    public partial class idOptions : Window {
        public string cs = @"server=" + Properties.Settings.Default.ip + 
            ";userid=" + Properties.Settings.Default.user + 
            ";password=" + Properties.Settings.Default.passwrd + 
            ";database=" + Properties.Settings.Default.database;
        public idOptions() {
            InitializeComponent();
        }

        private void closeWIndow_MouseUp(object sender, MouseButtonEventArgs e) {
            Close();
        }

        private void btn_CopyTo_Clip_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(id_textbox.Text);
        }

        private void titlebar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                window.DragMove();
            }
        }

        private void btn_createID_Click(object sender, RoutedEventArgs e)
        {
            if (pw_textbox.Text == "" && Properties.Settings.Default.useIDsWithoutPWD == false) {
                MessageBox.Show("Ein Passwort muss angegeben werden!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            } else {
                pbar.Value = 0;
                Random Random = new Random();
                pbar.Value = 1;
                int ranID = Random.Next(0, 99999999);
                pbar.Value = 2;
                id_textbox.Text = ranID.ToString();
                pbar.Value = 3;
                string password = pw_textbox.Text.Replace("System.Windows.Controls.TextBox: ", "");
                if (Properties.Settings.Default.hashPasswords == true) { 
                #pragma warning disable SYSLIB0021
                using SHA384 sha = new SHA384CryptoServiceProvider();
                byte[] preHash = Encoding.ASCII.GetBytes( password);
                byte[] hash = sha.ComputeHash(preHash);
                password = Convert.ToBase64String(hash);
                }
                pbar.Value = 4;
                try {
                    using var con = new MySqlConnection(cs);
                    pbar.Value = 5;
                    con.Open();
                    pbar.Value = 6;
                    var sql = "INSERT INTO `" + 
                        Properties.Settings.Default.table + "` (`" + 
                        Properties.Settings.Default.col1 + "`, `" + 
                        Properties.Settings.Default.col2 + "`, `" + 
                        Properties.Settings.Default.col3 + "`) " +
                        "VALUES ('" + ranID + "', ' ', '" + password + "');";
                    pbar.Value = 7;
                    using var cmd = new MySqlCommand(sql, con);
                    pbar.Value = 8;
                    cmd.Prepare();
                    pbar.Value = 9;
                    cmd.ExecuteNonQuery();
                    pbar.Value = 10;
                    reminder.Visibility = Visibility.Visible;
                    pbar.Value = 11;
                    MessageBox.Show("Ihre ID wurde erstellt", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch {
                    MessageBox.Show("Ein Fehler ist aufgetreten", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    pbar.Value = 0;
                }
                id_textbox.Text = Convert.ToString(ranID);
                btn_createID.IsEnabled = false;
            }
        }

        private void btn_clearID_Click(object sender, RoutedEventArgs e) {
            delID del = new delID();
            del.Show();
        }

        private void btn_createID_Copy_Click(object sender, RoutedEventArgs e) {
            clearID clear = new();
            clear.Show();
        }

        private void window_Activated(object sender, EventArgs e) {
            
        }

        private void pw_textbox_GotFocus(object sender, RoutedEventArgs e) { blur.Radius = 0; }

        private void pw_textbox_LostFocus(object sender, RoutedEventArgs e) { if (pw_textbox.Text != "" && Properties.Settings.Default.blurPasswdBoxes == true) { blur.Radius = 15; } }
    }
}
