using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace XCamera.UWP
{
    class Exif: XCamera.IExif
    {
        public async Task<string> GetComment(string filename)
        {
            string basedir = Path.GetDirectoryName(filename);
            string basename = Path.GetFileName(filename);

            var src = await StorageFile.GetFileFromPathAsync(filename);


            ImageProperties props = await src.Properties.GetImagePropertiesAsync();


            using (var stream = await src.OpenAsync(FileAccessMode.ReadWrite))
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);

                var encoder = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);


                // var props = await encoder.BitmapProperties.GetPropertiesAsync(new string[] { "/app1/ifd/exif/{ushort=315}" });
                
                 // var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                 var list = new List<KeyValuePair<string, BitmapTypedValue>>();

                var artist = new BitmapTypedValue("Hello World", Windows.Foundation.PropertyType.String);

                list.Add(new KeyValuePair<string, BitmapTypedValue>("/app1/ifd/{ushort=37510}", artist));

                await encoder.BitmapProperties.SetPropertiesAsync(list);

                await encoder.FlushAsync();

                await stream.FlushAsync();
            }

            return "";
        }
        public async Task<string> SetComment(string filename, string newComment)
        {
            string basename = Path.GetFileName(filename);

            var src = await KnownFolders
                        .PicturesLibrary
                        .GetFileAsync(filename);

            using (var stream = await src.OpenAsync(FileAccessMode.ReadWrite))
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);

                var encoder = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);

                // var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                var list = new List<KeyValuePair<string, BitmapTypedValue>>();

                var comment = new BitmapTypedValue(newComment, Windows.Foundation.PropertyType.String);

                list.Add(new KeyValuePair<string, BitmapTypedValue>("/app1/ifd/exif/{ushort=37510}", comment));

                await encoder.BitmapProperties.SetPropertiesAsync(list);

                await encoder.FlushAsync();

                await stream.FlushAsync();
            }
            return "";
        }
    }
}
