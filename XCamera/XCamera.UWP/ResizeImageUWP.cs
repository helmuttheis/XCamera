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
        public async Task<byte[]> ResizeImage(byte[] imageData, float width, float height)
        {
            return await ResizeImageWindows(imageData, width, height);
        }
        
        public async Task<byte[]> ResizeImageWindows(byte[] imageData, float width, float height)
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
                    encoder.BitmapTransform.ScaledHeight = (uint)height;
                    encoder.BitmapTransform.ScaledWidth = (uint)width;
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
