using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
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
        private FlowDocument doc;
        private Table table;

        private string szTitel;
        private Dictionary<string, string> dictBilder;

        public PublishWindow()
        {
            InitializeComponent();
            doc = new FlowDocument();

            fdViewer.Document = doc;
            dictBilder = new Dictionary<string, string>();

        }
        public void AddTitle(string szTitle)
        {
            this.szTitel = szTitle;
            Paragraph p = new Paragraph(new Run(szTitle));
            p.FontSize = 36;
            doc.Blocks.Add(p);
            
            // Create the Table...
            table = new Table();
            // ...and add it to the FlowDocument Blocks collection.
            doc.Blocks.Add(table);


            // Set some global formatting properties for the table.
            table.CellSpacing = 10;
            table.Background = Brushes.White;

            table.Columns.Add(new TableColumn());
            table.Columns.Add(new TableColumn());
            // Create and add an empty TableRowGroup to hold the table's Rows.
            table.RowGroups.Add(new TableRowGroup());

            // Add the first (title) row.
            table.RowGroups[0].Rows.Add(new TableRow());

        }
        public void AddBild(string szTitle, string szFilename)
        {
            if (!dictBilder.ContainsKey(szFilename))
            {
                dictBilder.Add(szFilename, szTitle);
            }
            table.RowGroups[0].Rows.Add(new TableRow());

            // Alias the current working row for easy reference.
            TableRow currentRow = table.RowGroups[0].Rows.Last();
            
            BlockUIContainer bc = new BlockUIContainer();

            Image myImg =  new Image();
            
            myImg.Source = new BitmapImage(new Uri(szFilename));
            
            myImg.Width = 500;
            myImg.Height = 500;
            myImg.Stretch = Stretch.Uniform;
            myImg.Margin = new Thickness(0, 0, 0, 0);

            Grid grid = new Grid();
            grid.Width = 500;
            grid.Height = 500;
            grid.Background = new SolidColorBrush(Colors.Green);
            grid.Children.Add(myImg);

            StackPanel sp = new StackPanel();
            sp.Width = 600;
            sp.Height = 600;
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            sp.Background = new SolidColorBrush(Colors.White);
            sp.Children.Add(grid);

            bc.Child = sp;
            currentRow.Cells.Add(new TableCell(bc));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(szFilename))));
        }
        public BitmapImage ConvertByteArrayToBitMapImage(byte[] imageByteArray)
        {
            BitmapImage img = new BitmapImage();
            img.StreamSource = new MemoryStream(imageByteArray);
            return img;
        }
        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        public void Save(string szFileName, string dataFormat)
        {
        }

        public void SaveHtml(string szFileName, string dataFormat)
        {
            using (StreamWriter sw = new StreamWriter(szFileName, false))
            {
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                sw.WriteLine("<h1>" + szTitel + "</h1");
                sw.WriteLine("<table>");
                foreach (var kv in dictBilder)
                {
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<td  style=\"width:250px; height:250px; background-color:red;text-align:center; vertical-align:middle\">");

                    sw.WriteLine("<img src=\"" + kv.Key.Replace(@"\","/") + "\" style=\"max-height:100%; max-width:100%\"/>");
                    sw.WriteLine("</td>");
                    sw.WriteLine("<td>");
                    sw.WriteLine("<p>" + kv.Value + "</p>");
                    sw.WriteLine("</td>");


                    sw.WriteLine("</tr>");
                }
                sw.WriteLine("</table>");

                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }
        }

        public void SaveFlowdocument(string szFilenName, string dataFormat)
        {
            var content = new TextRange(doc.ContentStart, doc.ContentEnd);

            if (content.CanSave(dataFormat) )
            {
                try
                {
                    using (var stream = new FileStream(szFilenName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        content.Save(stream, dataFormat, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.ToString());
                }
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".html";
            dlg.Filter = "HTML (*.html)|*.html|Word (*.rtf)|*.rtf|XAML (*.xaml)|*.xaml";
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string szDataformat = string.Empty;
                string szExt = System.IO.Path.GetExtension(dlg.FileName);
                if( szExt.Equals(".rtf",StringComparison.InvariantCultureIgnoreCase))
                {
                    szDataformat = DataFormats.Rtf;
                }
                else if (szExt.Equals(".html", StringComparison.InvariantCultureIgnoreCase))
                {
                    szDataformat = DataFormats.Html;
                }
                else if (szExt.Equals(".xaml", StringComparison.InvariantCultureIgnoreCase))
                {
                    szDataformat = DataFormats.Xaml;
                }
                if (!string.IsNullOrWhiteSpace(szDataformat))
                {
                    Save(dlg.FileName,szDataformat);
                }
            }

         //  PrintDialog pd = new PrintDialog();
         //  if ((pd.ShowDialog() == true))
         //  {
         //      //use either one of the below      
         //      pd.PrintVisual(fdViewer as Visual, "printing as visual");
         //      pd.PrintDocument((((IDocumentPaginatorSource)fdViewer.Document).DocumentPaginator), "printing as paginator");
         //  }
        }
    }
}
