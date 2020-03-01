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
using XCamera.Util;

namespace XCameraManager
{
    /// <summary>
    /// Interaktionslogik für ImageEditWindow.xaml
    /// </summary>
    public partial class ImageEditWindow : Window
    {
        BildMitKommentar bmk;
        ProjectSql projectSql;
        BitmapImage image;


        private bool newImage;

        private bool clicked = false;

        public ImageEditWindow()
        {
            InitializeComponent();
        }

        public ImageEditWindow(ProjectSql projectSql, BildMitKommentar bmk = null, bool newImage = false)
        {
            this.bmk = bmk;
            this.projectSql = projectSql;
            this.newImage = newImage;

            InitializeComponent();
            this.Closing += InputWindow_Closing;

            if(newImage == true)
            {
                this.Title = "Neues Bild hinzufügen";

                //imgNewImage.Source = new BitmapImage(new Uri(bmk.BildPath));

                image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(bmk.BildPath);
                image.EndInit();
                imgNewImage.Source = image;
            }
            else
            {
                this.Title = "Bilddaten editieren";
            }

            cmbGebaeude.ItemsSource = null;
            cmbGebaeude.ItemsSource = projectSql.sqlGebaeude.GetListe();

            cmbEtage.ItemsSource = null;
            cmbEtage.ItemsSource = projectSql.sqlEtage.GetListe();

            cmbWohnung.ItemsSource = null;
            cmbWohnung.ItemsSource = projectSql.sqlWohnung.GetListe();

            cmbZimmer.ItemsSource = null;
            cmbZimmer.ItemsSource = projectSql.sqlZimmer.GetListe();

            if (bmk != null && !newImage)
            {
                cmbGebaeude.SelectedValue = bmk.BildInfo.GebaeudeId;
                cmbEtage.SelectedValue = bmk.BildInfo.EtageId;
                cmbWohnung.SelectedValue = bmk.BildInfo.WohnungId;
                cmbZimmer.SelectedValue = bmk.BildInfo.ZimmerId;
                tbKommentar.Text = bmk.BildInfo.KommentarBezeichnung;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if(bmk != null)
            {
                if (bmk.BildInfo == null) bmk.BildInfo = new BildInfo();

                Gebaeude gebaeude = cmbGebaeude.SelectedItem as Gebaeude;
                if (gebaeude != null)
                {
                    bmk.BildInfo.GebaeudeId = gebaeude.ID;
                    bmk.BildInfo.GebaeudeBezeichnung = gebaeude.Bezeichnung;
                }

                Etage etage = cmbEtage.SelectedItem as Etage;
                if (etage != null)
                {
                    bmk.BildInfo.EtageId = etage.ID;
                    bmk.BildInfo.EtageBezeichnung = etage.Bezeichnung;
                }

                Wohnung wohnung = cmbWohnung.SelectedItem as Wohnung;
                if (wohnung != null)
                {
                    bmk.BildInfo.WohnungId = wohnung.ID;
                    bmk.BildInfo.WohnungBezeichnung = wohnung.Bezeichnung;
                }

                Zimmer zimmer = cmbZimmer.SelectedItem as Zimmer;
                if (zimmer != null) 
                {
                    bmk.BildInfo.ZimmerId = zimmer.ID;
                    bmk.BildInfo.ZimmerBezeichnung = zimmer.Bezeichnung;
                }

                bmk.BildInfo.KommentarBezeichnung = tbKommentar.Text;

                if(newImage)
                {
                    if (dpCaptureDate.SelectedDate == null)
                    {
                        MessageBoxResult MbResult = MessageBox.Show("Es wurde kein Aufnahmedatum eingetragen. Wenn kein Datum eingetragen wird, wird das aktuelle Datum verwendet.\n Möchten sie ein Datum eintragen?", " Kein Aufnahmedatum eingetragen", MessageBoxButton.YesNo);
                        if (MbResult == MessageBoxResult.Yes) return;
                    }
                    bmk.BildInfo.CaptureDate = dpCaptureDate.SelectedDate ?? DateTime.Now;
                }

                clicked = true;
                this.Close();
            }
        }
        private void InputWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!clicked)
            {
                bmk = null;
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            clicked = false;
            imgNewImage.Source = null;
            this.Close();
        }

        public new BildMitKommentar ShowDialog()
        {
            base.ShowDialog();
            return bmk;
        }
    }
}
