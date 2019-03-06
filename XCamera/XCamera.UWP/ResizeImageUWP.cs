using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System;

namespace XCamera.UWP
{
    public class ResizeImageUWP:IResizeImage
    {
        public async Task<byte[]> ResizeImage(byte[] imageData, int scale)
        {
            return await ResizeImageWindows(imageData, scale);
        }
        
        public async Task<byte[]> ResizeImageWindows(byte[] imageData, int scale)
        {
            byte[] resizedData;

            using (var streamIn = new MemoryStream(imageData))
            {
                using (IRandomAccessStream imageStream = streamIn.AsRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(imageStream);
                    var resizedStream = new InMemoryRandomAccessStream();
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                    
                    encoder.BitmapTransform.ScaledHeight = (uint)(decoder.OrientedPixelHeight / scale);
                    encoder.BitmapTransform.ScaledWidth = (uint)(decoder.OrientedPixelWidth / scale);
                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    resizedData = new byte[resizedStream.Size];
                    await resizedStream.ReadAsync(resizedData.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);
                }
            }

            return resizedData;
        }
    }
}
