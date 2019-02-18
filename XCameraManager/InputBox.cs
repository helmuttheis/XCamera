using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace XCameraManager
{
    public class InputBox
    {
        Window Box = new Window();//window for the inputbox
        FontFamily font = new FontFamily("Tahoma");//font for the whole inputbox
        int FontSize = 14;//fontsize for the input
        StackPanel sp1 = new StackPanel();// items container
        string title = "Neues Projekt";//title as heading
        string boxcontent;//title
        string defaulttext = "Projektname";//default textbox content
        string errormessage = "Fehler";//error messagebox content
        string errortitle = "Fehler";//error messagebox heading title
        string okbuttontext = "anlegen";//Ok button content
        string cancelbuttontext = "abbrechen";//Ok button content
        Brush BoxBackgroundColor = Brushes.White;// Window Background
        Brush InputBackgroundColor = Brushes.White;// Textbox Background
        bool clicked = false;
        TextBox input = new TextBox();
        Button ok = new Button();
        Button cancel = new Button();
        bool inputreset = false;

        public InputBox(string content)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            windowdef();
        }

        public InputBox(string content, string Htitle, string DefaultText)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            try
            {
                defaulttext = DefaultText;
            }
            catch
            {
                DefaultText = "Error!";
            }
            windowdef();
        }

        public InputBox(string content, string Htitle, string Font, int Fontsize)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                font = new FontFamily(Font);
            }
            catch { font = new FontFamily("Tahoma"); }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            if (Fontsize >= 1)
                FontSize = Fontsize;
            windowdef();
        }

        private void windowdef()// window building - check only for window size
        {
            Box.SizeToContent = SizeToContent.WidthAndHeight;
            Box.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Box.ResizeMode = ResizeMode.NoResize;
            Grid ButtonGrid = new Grid();
            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            colDef1.Width = new GridLength(1, GridUnitType.Star);
            colDef2.Width = new GridLength(1, GridUnitType.Star);
            ButtonGrid.ColumnDefinitions.Add(colDef1);
            ButtonGrid.ColumnDefinitions.Add(colDef2);

            Box.Background = BoxBackgroundColor;
            Box.Title = title;
            Box.Content = sp1;
            Box.Closing += Box_Closing;
            TextBlock content = new TextBlock();
            content.TextWrapping = TextWrapping.Wrap;
            content.Background = null;
            content.HorizontalAlignment = HorizontalAlignment.Left;
            content.Text = boxcontent;
            content.FontFamily = font;
            content.FontSize = FontSize;
            content.Margin = new Thickness(10, 10, 0, 0);
            sp1.Children.Add(content);

            input.Background = InputBackgroundColor;
            input.FontFamily = font;
            input.FontSize = FontSize;
            input.HorizontalAlignment = HorizontalAlignment.Center;
            input.Text = defaulttext;
            input.MinWidth = 200;
            input.MouseEnter += input_MouseDown;
            input.Margin = new Thickness(10, 5, 10, 0);
            input.Foreground = Brushes.LightGray;
            sp1.Children.Add(input);
            ok.Width = 70;
            ok.Height = 25;
            ok.Click += ok_Click;
            ok.Content = okbuttontext;
            ok.HorizontalAlignment = HorizontalAlignment.Center;
            sp1.Children.Add(ButtonGrid);
            ok.MaxWidth = 80;
            ok.HorizontalAlignment = HorizontalAlignment.Right;
            ok.Margin = new Thickness(0, 5, 2, 10);
            ok.VerticalContentAlignment = VerticalAlignment.Center;
            ButtonGrid.Children.Add(ok);
            cancel.Width = 70;
            cancel.Height = 25;
            cancel.Click += cancel_Click;
            cancel.Content = cancelbuttontext;
            cancel.HorizontalAlignment = HorizontalAlignment.Center;
            cancel.MaxWidth = 80;
            cancel.HorizontalAlignment = HorizontalAlignment.Left;
            cancel.Margin = new Thickness(2, 5, 0, 10);
            cancel.VerticalContentAlignment = VerticalAlignment.Center;
            ButtonGrid.Children.Add(cancel);
            Grid.SetColumn(cancel, 1);
        }

        void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!clicked)
            {
                //      e.Cancel = true;
                input.Text = "";
            }
        }

        private void input_MouseDown(object sender, MouseEventArgs e)
        {
            if ((sender as TextBox).Text == defaulttext && inputreset == false)
            {
                input.Foreground = Brushes.Black;
                (sender as TextBox).Text = null;
                inputreset = true;
            }
        }

        void ok_Click(object sender, RoutedEventArgs e)
        {
            if (input.Text == defaulttext || input.Text == "")
                MessageBox.Show(errormessage, errortitle);
            else
            {
                clicked = true;
                Box.Close();
                clicked = false;
            }
        }
        void cancel_Click(object sender, RoutedEventArgs e)
        {
            Box.Close();
        }
        public string ShowDialog()
        {
            Box.ShowDialog();
            return input.Text;
        }
    }
}
