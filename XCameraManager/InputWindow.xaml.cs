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

namespace XCameraManager
{
    /// <summary>
    /// Interaktionslogik für InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public string title { get; set; } = "";
        public string inputDefaultText { get; set; } = "";
        private bool clicked = false;


        public delegate bool IsValid(string szBezeichnung);

        public IsValid IsValidHandler;

        public InputWindow()
        {
            InitializeComponent();
            windowDef();
        }

        public InputWindow(string title, string inputDefaultText)
        {
            InitializeComponent();
            this.title = title;
            this.inputDefaultText = inputDefaultText;
            this.Closing += InputWindow_Closing;
            windowDef();
        }

        private void InputWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!clicked)
            {
                tbContent.Text = "";
            }
        }

        private void windowDef()
        {
            this.Title = title;
            tbContent.Text = inputDefaultText;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbContent.Text))
            {
                MessageBox.Show("Der neue Name darf nicht leer sein.");
                return;
            }
            if (IsValidHandler?.Invoke(tbContent.Text) == true)
            {
                clicked = true;
                this.Close();
                return;
            }
            MessageBox.Show("Der Name ist schon vergeben.");
            return;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbContent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public new string ShowDialog()
        {
            base.ShowDialog();
            return tbContent.Text;
        }
    }
}
