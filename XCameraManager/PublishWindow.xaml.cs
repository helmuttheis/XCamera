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

namespace XCameraManager
{
    /// <summary>
    /// Interaktionslogik für PublishWindow.xaml
    /// </summary>
    public partial class PublishWindow : Window
    {
        private string templateFolderPath;

        private Template selectedTemplate = null;

        private bool clicked = false;

        public PublishWindow()
        {
            InitializeComponent();
            this.Closing += PublishWindow_Closing;

            templateFolderPath = XCamera.Util.Config.current.szWordTemplateFolder;
            if(!Directory.Exists(templateFolderPath))
            {
                Directory.CreateDirectory(templateFolderPath);
            }

            tbTemplatedir.Text = templateFolderPath;

            cmbTemplate.ItemsSource = GetTemplates();

        }

        private List<Template> GetTemplates()
        {
            string[] templates = System.IO.Directory.GetFiles(templateFolderPath, "*.docx");
            List<Template> templateList = new List<Template>();

            foreach (var template in templates)
            {
                templateList.Add(new Template { filename = System.IO.Path.GetFileName(template), filePath = template });
            }
            return templateList;
        }

        private void btnSelectTemplatedir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            if (!string.IsNullOrWhiteSpace(tbTemplatedir.Text)) folderDlg.SelectedPath = tbTemplatedir.Text;
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tbTemplatedir.Text = folderDlg.SelectedPath;
                templateFolderPath = folderDlg.SelectedPath;
            }

            cmbTemplate.ItemsSource = null;
            cmbTemplate.ItemsSource = GetTemplates();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if(cmbTemplate.SelectedItem == null)
            {
                MessageBox.Show("Bitte ein Template auswählen!", "Kein Template gewählt", MessageBoxButton.OK);
                return;
            }

            selectedTemplate = cmbTemplate.SelectedItem as Template;

            XCamera.Util.Config.current.szWordTemplateFolder = tbTemplatedir.Text;

            clicked = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clicked = false;
            this.Close();
        }
        private void PublishWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!clicked)
            {
                selectedTemplate = null;
            }
        }

        public new string ShowDialog()
        {
            base.ShowDialog();
            if (selectedTemplate != null) return selectedTemplate.filePath;
            else return "";
        }
    }

    public class Template
    {
        public string filename { get; set; }
        public string filePath { get; set; }
    }
}
