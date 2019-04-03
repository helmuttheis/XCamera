using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
    /// Interaktionslogik für ManagerWindow.xaml
    /// </summary>
    public partial class ManageWindow : Window
    {
        private ProjectSql projectSql;
        public ManageWindow(ProjectSql projectSql)
        {
            InitializeComponent();
            cmbTable.Items.Clear();
            cmbTable.Items.Add("Gebäude");
            cmbTable.Items.Add("Wohnung");
            cmbTable.Items.Add("Etage");
            cmbTable.Items.Add("Zimmer");
            cmbTable.Items.Add("Kommentar");
            this.projectSql = projectSql;

        }

        private void CmbTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                grdData.ItemsSource = null;
                string szSelected = cmbTable.SelectedItem.ToString();
                if (szSelected.Equals("Gebäude"))
                {
                    grdData.ItemsSource = projectSql.GetUsedGebaeude();
                }
                else if (szSelected.Equals("Etage"))
                {
                    grdData.ItemsSource = projectSql.GetUsedEtage();
                }
                else if (szSelected.Equals("Wohnung"))
                {
                    grdData.ItemsSource = projectSql.GetUsedWohnung();
                }
                else if (szSelected.Equals("Zimmer"))
                {
                    grdData.ItemsSource = projectSql.GetUsedZimmer();
                }
                else
                {
                    grdData.ItemsSource = projectSql.GetUsedKommentar();
                }

            }
            catch (Exception ex)
            {
                // ToDo log
            }
        }
    }
}
