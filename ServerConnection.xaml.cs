using MySqlConnector;
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
    /// Interaktionslogik für ServerConnection.xaml
    /// </summary>
    public partial class ServerConnection : Window
    {
        public ServerConnection()
        {
            InitializeComponent();
        }

        private void btn_continue_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Bevor diese Einstellung übernommen werden kann, müssen die Einstellungen getestet werden.\nWollen Sie fortfahren?", "Verbindungstest", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result) {
                case MessageBoxResult.Yes:
                    try {
                        string cs = @"server=" + txt_ip.Text.Replace("Windows.Controls.TextBox: ", "") +
                            ";userid=" + txt_user.Text.Replace("Windows.Controls.TextBox: ", "") +
                            ";password=" + txt_password.Text.Replace("Windows.Controls.TextBox: ", "") +
                            ";database=" + txt_database.Text.Replace("Windows.Controls.TextBox: ", "");
                        using var con = new MySqlConnection(cs);
                        con.Open();
                        var sql = "INSERT INTO `" + 
                            txt_table.Text + "` (`" + 
                            txt_col1.Text + "`, `" +
                            txt_col2.Text + "`, `" +
                            txt_col3.Text + "`) " +
                            "VALUES ('test', 'testentry', 'testpasswd');";
                        using var cmd = new MySqlCommand(sql, con);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        MessageBoxResult connected = MessageBox.Show("Verbindung erfolgreich. Wollen Sie ihre Einstellungen speichern und fortfahren?", "Verbindung erfolgreich", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        switch (result) {
                            case MessageBoxResult.Yes:
                                Properties.Settings.Default.ip = txt_ip.Text.Replace("Windows.Controls.TextBox: ", "");
                                Properties.Settings.Default.database = txt_database.Text.Replace("Windows.Controls.TextBox: ", "");
                                Properties.Settings.Default.table = txt_table.Text;
                                Properties.Settings.Default.col1 = txt_col1.Text;
                                Properties.Settings.Default.col2 = txt_col2.Text;
                                Properties.Settings.Default.col3 = txt_col3.Text;
                                Properties.Settings.Default.user = txt_user.Text.Replace("Windows.Controls.TextBox: ", "");
                                Properties.Settings.Default.passwrd = txt_password.Text.Replace("Windows.Controls.TextBox: ", "");
                                Properties.Settings.Default.useOwnServer = true;
                                Properties.Settings.Default.Save();
                                break;
                        }

                    } catch(Exception ex) {
                        MessageBox.Show("Die Verbindung ist gescheitert. Stellen Sie sicher, dass Sie mit dem Internet verbunden sind und Ihre Eingaben richtig getätigt haben.\n\n" + ex.Message, "Verbindung gescheitert", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                window.DragMove();
            }
        }

        private void link_useStandardServer_MouseUp(object sender, MouseButtonEventArgs e)  {
            Properties.Settings.Default.ip = "10.33.156.250";
            Properties.Settings.Default.database = "checknotiz_pro";
            Properties.Settings.Default.table = "shared";
            Properties.Settings.Default.col1 = "id";
            Properties.Settings.Default.col2 = "note";
            Properties.Settings.Default.col3 = "passwrd";
            Properties.Settings.Default.user = "prouser";
            Properties.Settings.Default.passwrd = "st4rk3sP4s2w0rt";
            Properties.Settings.Default.useOwnServer = false;
            Properties.Settings.Default.Save();
            Close();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
