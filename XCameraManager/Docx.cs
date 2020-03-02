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
        public static string szError { get; set; }
        public static Boolean FillTable(string fileName, Dictionary<string, string> varDict, Dictionary<string, BildMitKommentar> dictBilder)
        {
            Boolean bRet = false;
            szError = "";
            try
            {
                File.Copy(Config.current.szWordTemplate, fileName, true);
                using (WordprocessingDocument wordDocument
                    = WordprocessingDocument.Open(fileName, true))
                {
                    OpenXmlPowerTools.SimplifyMarkupSettings settings = new OpenXmlPowerTools.SimplifyMarkupSettings
                    {
                        RemoveComments = true,
                        RemoveContentControls = true,
                        RemoveEndAndFootNotes = true,
                        RemoveFieldCodes = false,
                        RemoveLastRenderedPageBreak = true,
                        RemovePermissions = true,
                        RemoveProof = true,
                        RemoveRsidInfo = true,
                        RemoveSmartTags = true,
                        RemoveSoftHyphens = true,
                        // ReplaceTabsWithSpaces = true,
                        RemoveGoBackBookmark=true,
                        RemoveBookmarks=true,
                        RemoveMarkupForDocumentComparison=true,
                        RemoveWebHidden=true
                        
                    };
                    OpenXmlPowerTools.MarkupSimplifier.SimplifyMarkup(wordDocument, settings);


                    MainDocumentPart mainPart = wordDocument.MainDocumentPart;
                    
                    var paraList = mainPart.Document.Descendants<Paragraph>();
                    foreach ( var para in paraList)
                    {
                        ReplaceVar(para, varDict);
                    }
                    var headerPart = mainPart.HeaderParts.FirstOrDefault();
                    paraList = headerPart.Header.Descendants<Paragraph>();
                    foreach (var para in paraList)
                    {
                        ReplaceVar(para, varDict);
                    }
                    var footerPart = mainPart.FooterParts.FirstOrDefault();
                    paraList = footerPart.Footer.Descendants<Paragraph>();
                    foreach (var para in paraList)
                    {
                        ReplaceVar(para, varDict);
                    }

                    var tableList = mainPart.Document.Descendants<Table>();
                    foreach(var table in tableList)
                    {
                        var rowList = table.Descendants<TableRow>();
                        foreach (var tr in rowList)
                        {
                            var tc = tr.Descendants<TableCell>().FirstOrDefault();
                            // get the first para
                            var p = tc.Descendants<Paragraph>().FirstOrDefault();
                            if (p.InnerText.Trim().Equals("${PictureTable}", StringComparison.InvariantCultureIgnoreCase))
                            {
                                AddInTable(mainPart, table, tr, dictBilder, varDict);
                            }
                        }

                            
                        
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
        private static void ReplaceVar(Paragraph para, Dictionary<string,string> varDict)
        {
            // OpenXmlPowerTools.MarkupSimplifier.MergeAdjacentSuperfluousRuns(para);

            var runList = para.Descendants<Run>();
            var textList = para.Descendants<Text>();
            foreach (var text in textList)
            {
                foreach(var kv in varDict)
                {
                    text.Text = text.Text.Replace("${" + kv.Key + "}", kv.Value);
                }
            }
        }
        private static void AddInTable(MainDocumentPart mainPart,Table table, TableRow trRef, Dictionary<string, BildMitKommentar> dictBilder,
            Dictionary<string, string> varDict)
        {
            TableRow trTemplate = trRef.NextSibling<TableRow>();
            trRef.Remove();

            if (trTemplate == null)
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
            //
            var tcList = trTemplate.Descendants<TableCell>();
            var image = trTemplate.Descendants<Drawing>().FirstOrDefault();
            Int64Value imageWidth = 2160000L;
            if (image != null)
            {
                imageWidth = image.Inline.Extent.Cx;
                image.Ancestors<Paragraph>().FirstOrDefault().Remove();
            }

            foreach (var kv in dictBilder)
            {
                Dictionary<string, string> localVarDict = new Dictionary<string, string>();
                localVarDict.Add("Building", kv.Value.BildInfo.GebaeudeBezeichnung);
                localVarDict.Add("Floor", kv.Value.BildInfo.EtageBezeichnung);
                localVarDict.Add("Flat", kv.Value.BildInfo.WohnungBezeichnung);
                localVarDict.Add("Room", kv.Value.BildInfo.ZimmerBezeichnung);
                localVarDict.Add("Comment", kv.Value.Kommentar);

                localVarDict.Add("PictureName", kv.Key);
                localVarDict.Add("PictureDate", kv.Value.CaptureDate);

                Dictionary<string, string> tmpVarDict = new Dictionary<string, string>(localVarDict);
                foreach (var kv2 in tmpVarDict)
                {
                    if (kv2.Value == null)
                    {
                        localVarDict[kv2.Key] = Config.current.szWordEmptyInfo;
                    }
                }

                System.Drawing.Image img = System.Drawing.Image.FromFile(kv.Key);
                Int64Value imageHeight = (imageWidth/img.Width)*img.Height;

                // Create a row.
                TableRow tr = new TableRow();

                foreach (var tc in tcList)
                {
                    var tcNew = tc.CloneNode(true) as TableCell;

                    // get the first para
                    var p = tcNew.Descendants<Paragraph>().FirstOrDefault();
                    var paraList = tcNew.Descendants<Paragraph>();
                    foreach (var para in paraList)
                    {
                        if (para.InnerText.Trim().Equals("${Picture}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            para.RemoveAllChildren();
                            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                            using (FileStream stream = new FileStream(kv.Key, FileMode.Open, FileAccess.Read))
                            {
                                imagePart.FeedData(stream);
                            }
                            AddImageToPara(para, mainPart.GetIdOfPart(imagePart), imageWidth, imageHeight);
                        }
                        else
                        {
                            ReplaceVar(para, localVarDict);
                        }
                    }

                    tr.Append(tcNew);
                }
                //// Create a cell.
                //TableCell tc1 = new TableCell();

                //// Specify the table cell content.
                //tc1.Append(new Paragraph(new Run(new Text(kv.Key))));

                //ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                //using (FileStream stream = new FileStream(kv.Key, FileMode.Open, FileAccess.Read))
                //{
                //    imagePart.FeedData(ResizeImageWindows(stream, 4));
                //}
                //AddImageToCell(tc1, mainPart.GetIdOfPart(imagePart));
                //tc1.Append(new Paragraph(RunWithFont(kv.Value.CaptureDate, 8)));

                //// Append the table cell to the table row.
                //tr.Append(tc1);

                //TableCell tc2 = new TableCell();
                //// Specify the table cell content.
                //tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.GebaeudeBezeichnung))));
                //tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.EtageBezeichnung))));
                //tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.WohnungBezeichnung))));
                //tc2.Append(new Paragraph(new Run(new Text(kv.Value.BildInfo.ZimmerBezeichnung))));
                //tc2.Append(new Paragraph(new Run(new Text(kv.Value.Kommentar))));

                //// Append the table cell to the table row.
                //tr.Append(tc2);

                // Append the table row to the table.
                table.Append(tr);
            }

            if (trTemplate != null)
            {
                trTemplate.Remove();
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
        private static  MemoryStream ResizeImageWindows(Stream imageStream, int scale = 1)
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
        private static void AddImageToPara(Paragraph para, string relationshipId, Int64Value imageWidth, Int64Value imageHeight)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = imageWidth, Cy = imageHeight },
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
                                             new A.Extents() { Cx = imageWidth, Cy = imageHeight }),
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
            para.RemoveAllChildren();
            para.Append(new Run(element));
        }
    }
    
}
