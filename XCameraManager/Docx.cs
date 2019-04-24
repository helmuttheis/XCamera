using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using XCamera.Util;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace XCameraManager
{
    public static class Docx
    {
        // Insert a table into a word processing document.
        public static void CreateTable(string fileName,string szTitle, Dictionary<string, BildMitKommentar> dictBilder)
        {
            // Use the file name and path passed in as an argument 
            // to open an existing Word 2007 document.

            using (WordprocessingDocument wordDocument
                = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());


                Paragraph h1 = new Paragraph(RunWithFont(szTitle,24,true));

                // Append the title to the document.
                body.Append(h1);
                // Create an empty table.
                Table table = new Table();

                // Create a TableProperties object and specify its border information.
                BorderValues borderValue = BorderValues.BasicThinLines;
                UInt32Value borderWidth = 10;
                TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(borderValue),
                            Size = borderWidth
                        },
                        new BottomBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(borderValue),
                            Size = borderWidth
                        },
                        new LeftBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(borderValue),
                            Size = borderWidth
                        },
                        new RightBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(borderValue),
                            Size = borderWidth
                        },
                        new InsideHorizontalBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(borderValue),
                            Size = borderWidth
                        },
                        new InsideVerticalBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(borderValue),
                            Size = borderWidth
                        }
                    )
                );
                AddInTable(mainPart, table, dictBilder);
                // Append the TableProperties object to the empty table.
                table.AppendChild<TableProperties>(tblProp);
                
                // Append the table to the document.
                body.Append(table);
            }
        }
        public static string szError { get; set; }
        public static Boolean FillTable(string fileName, string szTitle, Dictionary<string, BildMitKommentar> dictBilder)
        {
            Boolean bRet = false;
            szError = "";
            try
            {
                File.Copy(Config.current.szWordTemplate, fileName, true);
                using (WordprocessingDocument wordDocument
                    = WordprocessingDocument.Open(fileName, true))
                {
                    MainDocumentPart mainPart = wordDocument.MainDocumentPart;
                    var para = mainPart.Document.Descendants<Paragraph>().FirstOrDefault();
                    if (para != null)
                    {
                        var text = para.Descendants<Text>().FirstOrDefault();
                        text.Text = szTitle;
                    }

                    var table = mainPart.Document.Descendants<Table>().FirstOrDefault();
                    if (table != null)
                    {
                        AddInTable(mainPart, table, dictBilder);
                    }
                    wordDocument.Save();
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                szError = "FillTable: " + ex.ToString();
                Logging.AddError(szError);
            }
            return bRet;
        }
        private static void AddInTable(MainDocumentPart mainPart,Table table, Dictionary<string, BildMitKommentar> dictBilder)
        {
            TableRow trTemplate = table.Descendants<TableRow>().FirstOrDefault();
            if(trTemplate == null)
            {
                trTemplate = new TableRow();
                TableCell tc1 = new TableCell();

                // Specify the width property of the table cell.
                tc1.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }));
                // Append the table cell to the table row.
                trTemplate.Append(tc1);
                TableCell tc2 = new TableCell();
                trTemplate.Append(tc2);
            }
            foreach (var kv in dictBilder)
            {
                // Create a row.
                TableRow tr = new TableRow();

                // Create a cell.
                TableCell tc1 = new TableCell();

                // Specify the table cell content.
                tc1.Append(new Paragraph(new Run(new Text(kv.Key))));

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (FileStream stream = new FileStream(kv.Key, FileMode.Open, FileAccess.Read))
                {
                    imagePart.FeedData(ResizeImageWindows(stream, 4));
                }
                AddImageToCell(tc1, mainPart.GetIdOfPart(imagePart));
                tc1.Append(new Paragraph(RunWithFont(kv.Value.CaptureDate, 8)));

                // Append the table cell to the table row.
                tr.Append(tc1);

                TableCell tc2 = new TableCell();
                // Specify the table cell content.
                tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.GebaeudeBezeichnung))));
                tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.EtageBezeichnung))));
                tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.WohnungBezeichnung))));
                tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.ZimmerBezeichnung))));
                tc2.Append(new Paragraph(new Run(new Text(kv.Value.Kommentar))));

                // Append the table cell to the table row.
                tr.Append(tc2);

                // Append the table row to the table.
                table.Append(tr);
            }
        }
        private static Run RunWithFont(string szStr, int iFontSize,Boolean bBold=false)
        {
            Run run = new Run(new Text(szStr));

            var runProp = new RunProperties();

            var runFont = new RunFonts { Ascii = "Arial"};
            var size = new FontSize { Val = new StringValue(iFontSize.ToString()) };

            runProp.Append(runFont);
            runProp.Append(size);
            if (bBold)
            {
                runProp.Append(new Bold());
            }
            run.PrependChild(runProp);

            return run;
        }
        private static  MemoryStream ResizeImageWindows(Stream imageStream, int scale)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Image img = Image.FromStream(imageStream))
            {
                using (Bitmap bitmap = new Bitmap(img, (int)(img.Width / scale), (int)(img.Height / scale)))
                {
                    bitmap.Save(memoryStream, ImageFormat.Jpeg);
                }
            }
            memoryStream.Position = 0;
            return memoryStream;
        }


        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        private static void AddImageToCell(TableCell tc1, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            tc1.AppendChild(new Paragraph(new Run(element)));
        }
    }
    
}
