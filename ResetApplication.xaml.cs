using System;
using System.Collections.Generic;
using System.Drawing;
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
using Point = System.Windows.Point;

namespace CheckNotiz_Pro
{
    /// <summary>
    /// Interaktionslogik für ResetApplication.xaml
    /// </summary>
    public partial class ResetApplication : Window
    {
        public ResetApplication()
        {
            InitializeComponent();
        }
        private void BlurBackground()
        {
            var positionTransform = dialog_box.TransformToAncestor(this);
            var areaPosition = positionTransform.Transform(new Point(0, 0));

            Bitmap Screenshot = new Bitmap(422, 190);
            Graphics G = Graphics.FromImage(Screenshot);
            G.CopyFromScreen((int)areaPosition.X - 17, (int)areaPosition.Y - 17, 0, 0, new System.Drawing.Size(422, 190), CopyPixelOperation.SourceCopy);
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\tempScrshot.png";
            try
            {
                System.IO.FileStream fs = System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate);
                Screenshot.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                fs.Close();
            }
            catch { }
            bkg.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\checknotiz\tempScrshot.png", UriKind.Absolute));
        }


        private void btn_resume_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_resume_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.useBackdropBlur == true)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                BlurBackground();
            }
            else
            {
                box.Background = System.Windows.Media.Brushes.LightGray;
                backBlur.Radius = 0;
            }
        }

        private void btn_resume_Copy_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.resetApplication = true;
            Properties.Settings.Default.Save();
            this.Hide();
            this.Close();
        }
    }
}
