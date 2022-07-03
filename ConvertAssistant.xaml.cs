using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CheckNotiz_Pro {
    /// <summary>
    /// Interaktionslogik für ConvertAssistant.xaml
    /// </summary>
    public partial class ConvertAssistant : Window {
        public ConvertAssistant() {
            InitializeComponent();
        }

        private void titlebar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) { window.DragMove(); }
        }

        private void closeWIndow_MouseUp(object sender, MouseButtonEventArgs e) {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult cont = MessageBox.Show("Wollen Sie die neue Speicherdatei wirklich ÜBERSCHREIBEN ?","Frage", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (cont) {
                case MessageBoxResult.Yes:
                    try {
                        string oldFileContent = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\notizen.chnf", Encoding.UTF8);
                        StreamWriter SaveFile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\pronotizen.chnf");
                        SaveFile.Write("");
                        SaveFile.WriteLine(oldFileContent.Replace("System.Windows.Controls.Label: ", ""));
                        SaveFile.Close();
                        MessageBox.Show("Der Vorgang ist erfolgreich abgeschlossen.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    } catch {
                        MessageBox.Show("Ein Fehler ist aufgetreten. Möglicherweise existiert keine ältere Speicherdatei.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }
    }
}
