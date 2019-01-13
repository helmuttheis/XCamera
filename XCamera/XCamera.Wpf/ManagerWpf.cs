using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace XCamera.Wpf
{
    public class ManagerWpf:IManager
    {
        public async Task Open()
        {
            // await Task.Delay(100);
            // get a picture
            // Image image = new Bitmap(@"c:\FakePhoto.jpg");
            await Task.Run(() =>
            {


                using (Image image = new Bitmap(@"E:\tmp\franzi\franzi002.21.jpg"))
                {
                    PropertyItem[] propItems = image.PropertyItems;
                    // For each PropertyItem in the array, display the ID, type, and 
                    // length.
                    int count = 0;
                    System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
                    foreach (PropertyItem propItem in propItems)
                    {
                        Console.WriteLine("Property Item " + count.ToString());

                        Console.WriteLine("   iD: 0x" + propItem.Id.ToString("x"));

                        Console.WriteLine("   type: " + propItem.Type.ToString());

                        Console.WriteLine("   length: " + propItem.Len.ToString() + " bytes");

                        string value = encoding.GetString(propItem.Value);

                        Console.WriteLine("   value: " + value);

                        if (propItem.Id == 40092) // XPComment
                        {
                            string szNewContent = "new/XPComment";
                            propItem.Value = encoding.GetBytes(szNewContent);
                            image.SetPropertyItem(propItem);
                        }
                        count++;
                    }


                    image.Save(@"E:\tmp\franzi\franzi002.21a.jpg"); // Automatic encoder selected based on extension.
                }

            });
        }
    }
}
