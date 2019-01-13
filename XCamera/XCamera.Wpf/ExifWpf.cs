using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCamera.Wpf
{
    class ExifWpf : XCamera.IExif
    {
        public async Task<string> GetComment(Stream stream)
        {
            string szValue = "";
            await Task.Run(() =>
            {
                using (Image image = new Bitmap(stream))
                {
                    PropertyItem[] propItems = image.PropertyItems;
                    // For each PropertyItem in the array, display the ID, type, and 
                    // length.
                    int count = 0;
                    System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
                    foreach (PropertyItem propItem in propItems)
                    {
                        if (propItem.Id == 40092) // XPComment
                        {
                            szValue = encoding.GetString(propItem.Value);
                        }
                        count++;
                    }

                    image.Save(@"E:\tmp\franzi\franzi002.21a.jpg"); // Automatic encoder selected based on extension.
                }
            });
            return szValue;
        }
        public async Task<string> SetComment(Stream stream, string newComment)
        {
            string szValue = "";
            await Task.Run(() =>
            {

                using (Image image = new Bitmap(stream))
                {
                    PropertyItem[] propItems = image.PropertyItems;
                    // For each PropertyItem in the array, display the ID, type, and 
                    // length.
                    int count = 0;
                    System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
                    foreach (PropertyItem propItem in propItems)
                    {
                        if (propItem.Id == (int)ExifTag.XPComment) // XPComment
                        {
                            szValue = encoding.GetString(propItem.Value);
                            string szNewContent = "new/XPComment";
                            propItem.Value = encoding.GetBytes(szNewContent);
                            image.SetPropertyItem(propItem);
                        }
                        count++;
                    }

                    // image.Save(@"E:\tmp\franzi\franzi002.21a.jpg"); // Automatic encoder selected based on extension.
                }
            });
            return szValue;
        }
    }
}
