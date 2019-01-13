using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace XCamera.UWP
{
    class ExifUwp: XCamera.IExif
    {
        public async Task<string> GetComment(Stream stream)
        {
            using (IRandomAccessStream iRandomAccessStream = stream.AsRandomAccessStream())
            {
                var decoder = await BitmapDecoder.CreateAsync(iRandomAccessStream);

                var encoder = await BitmapEncoder.CreateForTranscodingAsync(iRandomAccessStream, decoder);

                // var props = await encoder.BitmapProperties.GetPropertiesAsync(new string[] { "/app1/ifd/exif/{ushort=315}" });
                
                 // var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                 var list = new List<KeyValuePair<string, BitmapTypedValue>>();

                var artist = new BitmapTypedValue("Hello World", Windows.Foundation.PropertyType.String);

                list.Add(new KeyValuePair<string, BitmapTypedValue>("/app1/ifd/{ushort=" + (int)ExifTag.XPComment + "}", artist));

                await encoder.BitmapProperties.SetPropertiesAsync(list);

                await encoder.FlushAsync();

                await stream.FlushAsync();
            }

            return "";
        }
        public async Task<string> SetComment(Stream stream, string newComment)
        {
            using (IRandomAccessStream iRandomAccessStream = stream.AsRandomAccessStream())
            {
                var decoder = await BitmapDecoder.CreateAsync(iRandomAccessStream);

                var encoder = await BitmapEncoder.CreateForTranscodingAsync(iRandomAccessStream, decoder);

                // var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                var list = new List<KeyValuePair<string, BitmapTypedValue>>();

                var comment = new BitmapTypedValue(newComment, Windows.Foundation.PropertyType.String);

                list.Add(new KeyValuePair<string, BitmapTypedValue>("/app1/ifd/exif/{ushort=" + (int)ExifTag.XPComment + "}" , comment));

                await encoder.BitmapProperties.SetPropertiesAsync(list);

                await encoder.FlushAsync();

                await stream.FlushAsync();
            }
            return "";
        }
    }
}
