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
using System.IO;
using XCamera.Util;


namespace XCameraManager
{
    public partial class MoveDataWindow : Window
    {
        public MoveDataWindow()
        {
            InitializeComponent();
            tbOldDbSuffix.Text = Config.current.szDbSuffix;
            tbOldPicSuffix.Text = Config.current.szPicSuffix;
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if (tbNewDbSuffix.Text == tbOldDbSuffix.Text && tbNewPicSuffix.Text == tbOldPicSuffix.Text)
            {
                MessageBox.Show("Eingebene Suffixe sind identisch!");
            }
            else
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                var projekte = ProjectUtil.GetProjectList();
                foreach (var projekt in projekte)
                {
                    ((MainWindow)Application.Current.MainWindow).ToLog("moving " + projekt);
                    string oldDbPath = Path.Combine(ProjectUtil.szBasePath, projekt, tbOldDbSuffix.Text);
                    string newDbPath = Path.Combine(ProjectUtil.szBasePath, projekt, tbNewDbSuffix.Text);
                    if (File.Exists(Path.Combine(oldDbPath, projekt + ".db")))
                    {
                        if (!(tbNewDbSuffix.Text == tbOldDbSuffix.Text))
                        {
                            MoveFile(projekt + ".db", oldDbPath, newDbPath);
                        }
                        ProjectSql tmpProject = new ProjectSql(projekt, tbNewDbSuffix.Text);
                        if (!(tbNewPicSuffix.Text == tbOldPicSuffix.Text))
                        {
                            List<Bild> bildListe = tmpProject.GetBilder(null, null);
                            foreach (var bild in bildListe)
                            {
                                string oldPath = Path.Combine(Config.current.szBasedir, projekt, tbOldPicSuffix.Text);
                                string newPath = Path.Combine(Config.current.szBasedir, projekt, tbNewPicSuffix.Text);
                                MoveFile(bild.Name, oldPath, newPath);
                            }
                        }
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).ToLog("Database " + Path.Combine(oldDbPath, projekt + ".db") + " existiert nicht");
                    }
                }

                if (Config.current.szDbSuffix != tbNewDbSuffix.Text)
                {
                    Config.current.szDbSuffix = tbNewDbSuffix.Text;
                }
                if (Config.current.szPicSuffix != tbNewPicSuffix.Text)
                {
                    Config.current.szPicSuffix = tbNewPicSuffix.Text;
                }

                Mouse.OverrideCursor = null;
                if (MessageBox.Show("Daten erfolgreich verschoben.") == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }

        private void MoveFile(string fileName, string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);
            string sourceFile = Path.Combine(sourcePath, fileName);
            string destFile = Path.Combine(targetPath, fileName);
            if (File.Exists(sourceFile))
            {
                try
                {
                    File.Move(sourceFile, destFile);
                }
                catch(Exception ex)
                {
                    ((MainWindow)Application.Current.MainWindow).ToLog(ex.Message);
                }
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).ToLog("Datei " + sourceFile + " existiert nicht");
            }
        }
    }
}